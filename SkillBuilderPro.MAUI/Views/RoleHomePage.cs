using System.Text.Json;
using SkillBuilderPro.MAUI.Models;
using SkillBuilderPro.MAUI.Services;

namespace SkillBuilderPro.MAUI.Views;

public sealed class RoleHomePage : ContentPage
{
    private static readonly string[] AdminModules =
    {
        "USER MANAGEMENT", "DRILL MANAGEMENT", "GOALS & PROGRESSION", "TRAINING WORKFLOWS",
        "ANALYTICS & REPORTS", "SYSTEM HEALTH", "AUDIT LOGS", "SETTINGS"
    };

    private readonly IAthleteApiService api;
    private readonly string role;
    private readonly Label primary = new() { FontSize = 34, FontAttributes = FontAttributes.Bold, TextColor = Colors.White };
    private readonly Label secondary = new() { FontSize = 20, TextColor = Color.FromArgb("#168CFF") };
    private readonly Label status = new() { TextColor = Color.FromArgb("#C8D6E3") };
    private readonly Label headingLabel = new() { FontSize = 28, FontAttributes = FontAttributes.Bold, TextColor = Colors.White, LineBreakMode = LineBreakMode.NoWrap, MaxLines = 1 };
    private Grid? adminGrid;
    private VerticalStackLayout? contentStack;

    public RoleHomePage(IAthleteApiService api, string role)
    {
        this.api = api;
        this.role = role;
        BackgroundImageSource = role switch
        {
            "Parent" => "parent_dashboard_approved.png",
            "Coach" => "coach_office.png",
            "Administrator" => "admin_command_center_approved.png",
            _ => "weight_room.png"
        };

        var logout = new Button { Text = "LOG OUT", Style = (Style)Application.Current!.Resources["GlassActionButtonStyle"] };
        logout.Clicked += Logout;

        string heading = role == "Administrator" ? "ADMIN COMMAND CENTER" : $"{role.ToUpperInvariant()} EXPERIENCE";
        string subtitle = role == "Administrator"
            ? "PLATFORM OPERATIONS • PERFORMANCE • OVERSIGHT"
            : $"Welcome, {api.User?.FullName}";
        headingLabel.Text = heading;
        var header = new Border
        {
            Style = (Style)Application.Current.Resources["GlassHeaderStyle"],
            Content = new VerticalStackLayout
            {
                Children =
                {
                    headingLabel,
                    new Label { Text = subtitle, TextColor = Color.FromArgb("#D6E2EC") }
                }
            }
        };
        var metrics = new Border
        {
            Style = (Style)Application.Current.Resources["GlassPanelStyle"],
            Content = new VerticalStackLayout { Spacing = 8, Children = { primary, secondary, status } }
        };

        View modules = role == "Administrator" ? BuildAdminModules() : new Border
        {
            Style = (Style)Application.Current.Resources["GlassCardStyle"],
            Content = new Label
            {
                Text = role switch
                {
                    "Parent" => "Linked Athletes • Assignments • Training Requests • Notifications",
                    "Coach" => "Teams • Rosters • Assignments • Training Requests • Notifications",
                    _ => "Secure role tools"
                },
                TextColor = Colors.White
            }
        };

        contentStack = new VerticalStackLayout
        {
            Padding = 24, Spacing = 16, MaximumWidthRequest = 900,
            HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center,
            Children = { header, metrics, modules, logout }
        };
        Content = new ScrollView
        {
            Content = contentStack
        };
        SizeChanged += RoleHomeSizeChanged;
    }

    private View BuildAdminModules()
    {
        var grid = new Grid { ColumnSpacing = 12, RowSpacing = 12 };
        adminGrid = grid;
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        for (int row = 0; row < 4; row++) grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

        for (int index = 0; index < AdminModules.Length; index++)
        {
            var button = new Button
            {
                Text = AdminModules[index],
                Style = (Style)Application.Current!.Resources["GlassSecondaryButtonStyle"],
                MinimumHeightRequest = 54
            };
            button.Clicked += AdminModuleClicked;
            grid.Add(button, index % 2, index / 2);
        }
        return grid;
    }

    private void RoleHomeSizeChanged(object? sender, EventArgs e)
    {
        var phone = Width > 0 && Width < 620;
        headingLabel.FontSize = phone ? 23 : 28;
        primary.FontSize = phone ? 24 : 34;
        if (contentStack is not null) contentStack.Padding = phone ? new Thickness(16, 20, 16, 26) : new Thickness(24);
        if (adminGrid is null) return;
        adminGrid.ColumnDefinitions.Clear();
        adminGrid.RowDefinitions.Clear();
        var columns = phone ? 1 : 2;
        for (var column = 0; column < columns; column++) adminGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        var rows = (int)Math.Ceiling(AdminModules.Length / (double)columns);
        for (var row = 0; row < rows; row++) adminGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        for (var index = 0; index < adminGrid.Children.Count; index++)
        {
            adminGrid.SetColumn(adminGrid.Children[index], index % columns);
            adminGrid.SetRow(adminGrid.Children[index], index / columns);
        }
    }

    private async void AdminModuleClicked(object? sender, EventArgs e)
    {
        string module = (sender as Button)?.Text ?? "This module";
        await DisplayAlert(module, "This dedicated administrator workspace is not implemented yet.", "COMMAND CENTER");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        status.Text = "Loading live workspace…";
        if (role == "Parent")
        {
            var athletes = await api.GetAsync<List<JsonElement>>("api/parent/athletes") ?? [];
            var assignments = await api.GetAsync<List<JsonElement>>("api/parent/assignments") ?? [];
            var unread = await api.GetAsync<UnreadCount>("api/parent/notifications/unread-count");
            primary.Text = $"{athletes.Count} linked athlete{(athletes.Count == 1 ? "" : "s")}";
            secondary.Text = $"{assignments.Count} created assignments • {unread?.UnreadCountValue ?? 0} unread";
        }
        else if (role == "Coach")
        {
            var teams = await api.GetAsync<List<JsonElement>>("api/coach/teams") ?? [];
            var assignments = await api.GetAsync<List<JsonElement>>("api/coach/assignments") ?? [];
            var unread = await api.GetAsync<UnreadCount>("api/coach/notifications/unread-count");
            primary.Text = $"{teams.Count} team{(teams.Count == 1 ? "" : "s")}";
            secondary.Text = $"{assignments.Count} assignments • {unread?.UnreadCountValue ?? 0} unread";
        }
        else
        {
            var teams = await api.GetAsync<List<JsonElement>>("api/admin/teams") ?? [];
            primary.Text = $"{teams.Count} team{(teams.Count == 1 ? "" : "s")} in the organization";
            secondary.Text = "Administrator access verified by JWT role";
        }
        status.Text = "Live workspace ready.";
    }

    private async void Logout(object? sender, EventArgs e)
    {
        await api.LogoutAsync();
        Application.Current!.Windows[0].Page = new NavigationPage(new ChooseProfilePage(api));
    }
}
