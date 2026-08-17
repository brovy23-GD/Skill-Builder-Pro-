using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkillBuilderPro.Core.Data;
using SkillBuilderPro.Core.Identity;

namespace SkillBuilderPro.API.Data;

/// <summary>Development-only recovery for existing test accounts. It never creates users or stores passwords in source.</summary>
public static class DevelopmentExistingAccountResetInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        var environment = services.GetRequiredService<IHostEnvironment>();
        if (!environment.IsDevelopment()) return;
        await ResetAsync(services, "DevelopmentAthlete", ApplicationRoles.Athlete);
        await ResetAsync(services, "DevelopmentParent", ApplicationRoles.Parent);
    }

    private static async Task ResetAsync(IServiceProvider services, string section, string requiredRole)
    {
        var configuration = services.GetRequiredService<IConfiguration>();
        var email = configuration[$"{section}:Email"]?.Trim();
        var password = configuration[$"{section}:Password"];
        var enabled = configuration.GetValue<bool>($"{section}:ResetPassword");
        if (!enabled || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) return;

        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(DevelopmentExistingAccountResetInitializer));
        var users = services.GetRequiredService<UserManager<ApplicationUser>>();
        var db = services.GetRequiredService<AppDbContext>();
        var user = await users.FindByEmailAsync(email) ?? throw new InvalidOperationException($"Configured {section} account does not exist.");
        if (!await users.IsInRoleAsync(user, requiredRole)) throw new InvalidOperationException($"Configured {section} account does not have the required {requiredRole} role.");
        var active = await db.UserProfiles.AsNoTracking().AnyAsync(x => x.UserId == user.Id && x.IsActive);
        if (!active) throw new InvalidOperationException($"Configured {section} account is inactive or missing its profile.");

        var token = await users.GeneratePasswordResetTokenAsync(user);
        var result = await users.ResetPasswordAsync(user, token, password);
        if (!result.Succeeded) throw new InvalidOperationException($"Unable to reset {section} password: {string.Join("; ", result.Errors.Select(x => x.Code))}");
        logger.LogInformation("{Section} password reset completed for an existing active {Role} account.", section, requiredRole);
    }
}
