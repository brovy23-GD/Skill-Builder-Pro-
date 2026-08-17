using SkillBuilderPro.MAUI.Services;

namespace SkillBuilderPro.MAUI.ViewModels;

public partial class NotificationsViewModel
{
    public string Background => visuals.GetTrainingBackground(
        api.IsDemoMode ? DemoDataService.Sport : api.User?.Sport);
}
