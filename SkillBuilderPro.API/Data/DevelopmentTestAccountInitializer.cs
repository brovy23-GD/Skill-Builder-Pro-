using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkillBuilderPro.Core.Data;
using SkillBuilderPro.Core.Identity;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.API.Data;

/// <summary>
/// Reconciles the configured Development test accounts without deleting users,
/// profiles, or domain relationships. Credentials are supplied by configuration
/// (normally .NET User Secrets) and are never stored in source.
/// </summary>
public static class DevelopmentTestAccountInitializer
{
    private sealed record AccountDefinition(string Section, string Role);

    private static readonly AccountDefinition[] Accounts =
    [
        new("DevelopmentAdmin", ApplicationRoles.Administrator),
        new("DevelopmentCoach", ApplicationRoles.Coach),
        new("DevelopmentParent", ApplicationRoles.Parent),
        new("DevelopmentAthlete1", ApplicationRoles.Athlete),
        new("DevelopmentAthlete2", ApplicationRoles.Athlete)
    ];

    public static async Task InitializeAsync(IServiceProvider services)
    {
        var environment = services.GetRequiredService<IHostEnvironment>();
        if (!environment.IsDevelopment())
        {
            return;
        }

        foreach (var account in Accounts)
        {
            await ReconcileAsync(services, account);
        }
    }

    private static async Task ReconcileAsync(
        IServiceProvider services,
        AccountDefinition account)
    {
        var configuration = services.GetRequiredService<IConfiguration>();
        var logger = services
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger(nameof(DevelopmentTestAccountInitializer));
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var dbContext = services.GetRequiredService<AppDbContext>();

        var email = configuration[$"{account.Section}:Email"]?.Trim();
        var password = configuration[$"{account.Section}:Password"];
        var displayName = configuration[$"{account.Section}:DisplayName"]?.Trim();
        var previousEmail = configuration[$"{account.Section}:PreviousEmail"]?.Trim();

        if (string.IsNullOrWhiteSpace(email)
            || string.IsNullOrWhiteSpace(password)
            || string.IsNullOrWhiteSpace(displayName))
        {
            throw new InvalidOperationException(
                $"{account.Section} requires Email, Password, and DisplayName configuration in Development.");
        }

        if (displayName.Length > 100)
        {
            throw new InvalidOperationException(
                $"{account.Section}:DisplayName must not exceed 100 characters.");
        }

        var user = await userManager.FindByEmailAsync(email);

        if (user is null && !string.IsNullOrWhiteSpace(previousEmail))
        {
            user = await userManager.FindByEmailAsync(previousEmail);
            if (user is not null)
            {
                EnsureSucceeded(
                    await userManager.SetEmailAsync(user, email),
                    $"set the {account.Section} email");
                EnsureSucceeded(
                    await userManager.SetUserNameAsync(user, email),
                    $"set the {account.Section} user name");

                logger.LogInformation(
                    "{Section} retained Identity user ID {UserId} while its legacy email was reconciled.",
                    account.Section,
                    user.Id);
            }
        }

        if (user is null)
        {
            user = new ApplicationUser
            {
                Email = email,
                UserName = email
            };

            EnsureSucceeded(
                await userManager.CreateAsync(user, password),
                $"create the {account.Section} account");
        }
        else
        {
            if (!string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
            {
                EnsureSucceeded(
                    await userManager.SetEmailAsync(user, email),
                    $"set the {account.Section} email");
            }

            if (!string.Equals(user.UserName, email, StringComparison.OrdinalIgnoreCase))
            {
                EnsureSucceeded(
                    await userManager.SetUserNameAsync(user, email),
                    $"set the {account.Section} user name");
            }

            if (!await userManager.CheckPasswordAsync(user, password))
            {
                var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
                EnsureSucceeded(
                    await userManager.ResetPasswordAsync(user, resetToken, password),
                    $"reconcile the {account.Section} password");
            }
        }

        var assignedRoles = await userManager.GetRolesAsync(user);
        var applicationRolesToRemove = assignedRoles
            .Where(role => ApplicationRoles.All.Contains(role)
                && !string.Equals(role, account.Role, StringComparison.Ordinal))
            .ToArray();

        if (applicationRolesToRemove.Length > 0)
        {
            EnsureSucceeded(
                await userManager.RemoveFromRolesAsync(user, applicationRolesToRemove),
                $"remove incorrect roles from {account.Section}");
        }

        if (!await userManager.IsInRoleAsync(user, account.Role))
        {
            EnsureSucceeded(
                await userManager.AddToRoleAsync(user, account.Role),
                $"assign the {account.Role} role to {account.Section}");
        }

        var profile = await dbContext.UserProfiles
            .SingleOrDefaultAsync(item => item.UserId == user.Id);

        if (profile is null)
        {
            dbContext.UserProfiles.Add(new UserProfile
            {
                UserId = user.Id,
                FullName = displayName,
                IsActive = true,
                DateCreated = DateTime.UtcNow
            });
            await dbContext.SaveChangesAsync();
        }
        else
        {
            var changed = false;
            if (!profile.IsActive)
            {
                profile.IsActive = true;
                changed = true;
            }

            if (string.IsNullOrWhiteSpace(profile.FullName))
            {
                profile.FullName = displayName;
                changed = true;
            }

            if (changed)
            {
                await dbContext.SaveChangesAsync();
            }
        }

        logger.LogInformation(
            "{Section} is reconciled as active {Role} user ID {UserId}.",
            account.Section,
            account.Role,
            user.Id);
    }

    private static void EnsureSucceeded(IdentityResult result, string operation)
    {
        if (result.Succeeded)
        {
            return;
        }

        var errors = string.Join(
            "; ",
            result.Errors.Select(error => $"{error.Code}: {error.Description}"));

        throw new InvalidOperationException($"Unable to {operation}. {errors}");
    }
}
