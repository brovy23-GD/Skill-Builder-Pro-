using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkillBuilderPro.Core.Data;
using SkillBuilderPro.Core.Identity;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.API.Data;

public static class DevelopmentCoachInitializer
{
    private const string EmailKey = "DevelopmentCoach:Email";
    private const string PasswordKey = "DevelopmentCoach:Password";
    private const string DisplayNameKey = "DevelopmentCoach:DisplayName";
    private const string ResetPasswordKey = "DevelopmentCoach:ResetPassword";

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
            .CreateLogger(nameof(DevelopmentCoachInitializer));

        var email = configuration[EmailKey]?.Trim();
        var password = configuration[PasswordKey];
        var displayName = configuration[DisplayNameKey]?.Trim();
        var resetPassword = configuration.GetValue<bool>(ResetPasswordKey);

        if (string.IsNullOrWhiteSpace(email)
            || string.IsNullOrWhiteSpace(password)
            || string.IsNullOrWhiteSpace(displayName))
        {
            logger.LogInformation(
                "Development Coach bootstrap skipped because configuration is incomplete.");
            return;
        }

        if (displayName.Length > 100)
        {
            throw new InvalidOperationException(
                "DevelopmentCoach:DisplayName must not exceed 100 characters.");
        }

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var dbContext = services.GetRequiredService<AppDbContext>();

        var user = await userManager.FindByEmailAsync(email);
        var userWasCreated = false;
        var coachRoleWasAdded = false;

        if (user is null)
        {
            user = new ApplicationUser
            {
                Email = email,
                UserName = email
            };

            var createResult = await userManager.CreateAsync(user, password);
            EnsureSucceeded(createResult, "create the Development Coach account");
            userWasCreated = true;
        }

        try
        {
            if (!await userManager.IsInRoleAsync(user, ApplicationRoles.Coach))
            {
                var roleResult = await userManager.AddToRoleAsync(
                    user,
                    ApplicationRoles.Coach);
                EnsureSucceeded(roleResult, "assign the Coach role");
                coachRoleWasAdded = true;
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
                    "Development Coach account exists, but its profile is inactive.");
                return;
            }

            if (resetPassword)
            {
                if (!await userManager.IsInRoleAsync(user, ApplicationRoles.Coach))
                {
                    throw new InvalidOperationException(
                        "Development Coach password reset requires the configured user to have the Coach role.");
                }

                var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
                var resetResult = await userManager.ResetPasswordAsync(
                    user,
                    resetToken,
                    password);
                EnsureSucceeded(resetResult, "reset the Development Coach password");

                logger.LogInformation(
                    "Development Coach password reset completed.");
            }

            logger.LogInformation(
                "Development Coach account is available.");
        }
        catch
        {
            if (userWasCreated)
            {
                dbContext.ChangeTracker.Clear();
                await userManager.DeleteAsync(user);
            }
            else if (coachRoleWasAdded)
            {
                await userManager.RemoveFromRoleAsync(
                    user,
                    ApplicationRoles.Coach);
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
