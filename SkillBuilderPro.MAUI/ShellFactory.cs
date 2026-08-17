using SkillBuilderPro.MAUI.Services;
using SkillBuilderPro.MAUI.Views;
namespace SkillBuilderPro.MAUI;
public static class ShellFactory
{
 public static Page Create(IAthleteApiService api){var role=api.IsDemoMode?"Athlete":api.User?.Roles.FirstOrDefault()??api.SelectedRole;if(string.Equals(role,"Athlete",StringComparison.OrdinalIgnoreCase))return new AppShell();return new RoleHomePage(api,role??"User");}
}
