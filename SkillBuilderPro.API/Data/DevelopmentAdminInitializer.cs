using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkillBuilderPro.Core.Data;
using SkillBuilderPro.Core.Identity;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.API.Data;

public static class DevelopmentAdminInitializer
{
    private const string EmailKey = "DevelopmentAdmin:Email";
    private const string PasswordKey = "DevelopmentAdmin:Password";
    private const string DisplayNameKey = "DevelopmentAdmin:DisplayName";

    public static async Task InitializeAsync(IServiceProvider services)
    {
        var environment = services.GetRequiredService<IHostEnvironment>();
        if (!environment.IsDevelopment())
        {
            return;
        }

        var configuration = services.GetRequiredService<IConfiguration>();
        var logger = services
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger(nameof(DevelopmentAdminInitializer));

        var email = configuration[EmailKey]?.Trim();
        var password = configuration[PasswordKey];
        var displayName = configuration[DisplayNameKey]?.Trim();

        if (string.IsNullOrWhiteSpace(email)
            || string.IsNullOrWhiteSpace(password)
            || string.IsNullOrWhiteSpace(displayName))
        {
            logger.LogInformation(
                "Development Administrator bootstrap skipped because configuration is incomplete.");
            return;
        }

        if (displayName.Length > 100)
        {
            throw new InvalidOperationException(
                "DevelopmentAdmin:DisplayName must not exceed 100 characters.");
        }

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var dbContext = services.GetRequiredService<AppDbContext>();

        var user = await userManager.FindByEmailAsync(email);
        var userWasCreated = false;
        var administratorRoleWasAdded = false;

        if (user is null)
        {
            user = new ApplicationUser
            {
                Email = email,
                UserName = email
            };

            var createResult = await userManager.CreateAsync(user, password);
            EnsureSucceeded(createResult, "create the Development Administrator account");
            userWasCreated = true;
        }

        try
        {
            if (!await userManager.IsInRoleAsync(user, ApplicationRoles.Administrator))
            {
                var roleResult = await userManager.AddToRoleAsync(
                    user,
                    ApplicationRoles.Administrator);
                EnsureSucceeded(roleResult, "assign the Administrator role");
                administratorRoleWasAdded = true;
            }

            var profile = await dbContext.UserProfiles
                .SingleOrDefaultAsync(item => item.UserId == user.Id);

            if (profile is null)
            {
                profile = new UserProfile
                {
                    UserId = user.Id,
                    FullName = displayName,
                    IsActive = true,
                    DateCreated = DateTime.UtcNow
                };

                dbContext.UserProfiles.Add(profile);
                await dbContext.SaveChangesAsync();
            }

            if (!profile.IsActive)
            {
                logger.LogWarning(
                    "Development Administrator account exists, but its profile is inactive.");
                return;
            }

            logger.LogInformation(
                "Development Administrator account is available.");
        }
        catch
        {
            if (userWasCreated)
            {
                dbContext.ChangeTracker.Clear();
                await userManager.DeleteAsync(user);
            }
            else if (administratorRoleWasAdded)
            {
                await userManager.RemoveFromRoleAsync(
                    user,
                    ApplicationRoles.Administrator);
            }

            throw;
        }
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

        throw new InvalidOperationException(
            $"Unable to {operation}. {errors}");
    }
}
