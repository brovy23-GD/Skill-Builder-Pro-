using Microsoft.AspNetCore.Identity;
using SkillBuilderPro.Core.Identity;

namespace SkillBuilderPro.API.Data;

public static class IdentityRoleInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();

        foreach (var roleName in ApplicationRoles.All)
        {
            if (await roleManager.RoleExistsAsync(roleName))
            {
                continue;
            }

            var result = await roleManager.CreateAsync(new IdentityRole<int>(roleName));
            if (!result.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    result.Errors.Select(error => $"{error.Code}: {error.Description}"));

                throw new InvalidOperationException(
                    $"Unable to create application role '{roleName}'. {errors}");
            }
        }
    }
}
