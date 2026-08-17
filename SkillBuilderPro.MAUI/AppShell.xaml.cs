using SkillBuilderPro.MAUI.Views;

namespace SkillBuilderPro.MAUI;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(
            nameof(CategoryListPage),
            typeof(CategoryListPage));

        Routing.RegisterRoute(
            nameof(DrillListPage),
            typeof(DrillListPage));

        Routing.RegisterRoute(
            nameof(VideoPlayerPage),
            typeof(VideoPlayerPage));

        Routing.RegisterRoute(
            nameof(DrillLibraryPage),
            typeof(DrillLibraryPage));
        Routing.RegisterRoute(nameof(NotificationsPage),typeof(NotificationsPage));
        Routing.RegisterRoute(nameof(TrainingRequestsPage),typeof(TrainingRequestsPage));
    }
}
