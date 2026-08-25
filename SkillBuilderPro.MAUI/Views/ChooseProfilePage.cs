using SkillBuilderPro.MAUI.Services;

namespace SkillBuilderPro.MAUI.Views;

public sealed class ChooseProfilePage : ContentPage
{
    private enum ChooseRoleLayoutState { PhonePortrait, PhoneLandscape, TabletPortrait, TabletLandscape, Desktop }

    private static readonly (string Role, string Description, string Monogram)[] Roles =
    [
        ("Athlete", "Train. Compete. Elevate.", "A"),
        ("Coach", "Lead. Develop. Win.", "C"),
        ("Parent", "Support. Guide. Empower.", "P"),
        ("Administrator", "Manage. Oversee. Optimize.", "AD")
    ];

    private readonly IAthleteApiService api;
    private readonly ISportVisualService visuals;
    private readonly Image backgroundImage;
    private readonly Border headerSurface;
    private readonly Grid roleGrid;
    private readonly List<Border> roleCards = [];
    private readonly Border demoSurface;
    private readonly VerticalStackLayout liveUi;
    private ChooseRoleLayoutState? currentState;

    public ChooseProfilePage(IAthleteApiService api, ISportVisualService? visuals = null)
    {
        this.api = api;
        this.visuals = visuals ?? new SportVisualService();
        Title = "Choose Your Experience";
        NavigationPage.SetHasNavigationBar(this, false);

        backgroundImage = new Image
        {
            Aspect = Aspect.AspectFill,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };
        SemanticProperties.SetDescription(backgroundImage, "Skill Builder Pro Performance Headquarters");

        var title = new Label
        {
            Text = "CHOOSE YOUR EXPERIENCE",
            FontSize = 25,
            FontAttributes = FontAttributes.Bold,
            CharacterSpacing = 1.4,
            TextColor = Colors.White,
            HorizontalTextAlignment = TextAlignment.Center
        };
        SemanticProperties.SetHeadingLevel(title, SemanticHeadingLevel.Level1);
        var subtitle = new Label
        {
            Text = "SELECT HOW YOU'LL ENTER SKILL BUILDER PRO",
            FontSize = 12,
            CharacterSpacing = .8,
            TextColor = Color.FromArgb("#C8D2DC"),
            HorizontalTextAlignment = TextAlignment.Center
        };
        SemanticProperties.SetHeadingLevel(subtitle, SemanticHeadingLevel.Level2);
        var headerStack = new VerticalStackLayout { Spacing = 3, Children = { title, subtitle } };
        headerSurface = new Border
        {
            BackgroundColor = Color.FromArgb("#B8141A22"),
            Stroke = Color.FromArgb("#886B849F"),
            StrokeThickness = 1,
            Padding = new Thickness(18, 12),
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 },
            Content = headerStack
        };

        roleGrid = new Grid { ColumnSpacing = 10, RowSpacing = 10 };
        foreach (var role in Roles)
            roleCards.Add(CreateRoleCard(role));

        var demoIcon = new Label
        {
            Text = "D",
            FontSize = 12,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#168CFF"),
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center
        };
        SemanticProperties.SetDescription(demoIcon, "Demo Mode icon");
        var demoButton = new Button
        {
            Text = "DEMO MODE",
            FontSize = 12,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            BackgroundColor = Colors.Transparent,
            BorderWidth = 0,
            MinimumHeightRequest = 48,
            Padding = new Thickness(4, 0)
        };
        SemanticProperties.SetDescription(demoButton, "Enter Demo Mode. Explore Skill Builder Pro. No sign-in required.");
        demoButton.Clicked += DemoClicked;
        var demoDescription = new Label
        {
            Text = "Explore Skill Builder Pro",
            FontSize = 11,
            TextColor = Color.FromArgb("#C8D2DC"),
            VerticalTextAlignment = TextAlignment.Center
        };
        var demoGrid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(34)),
                new ColumnDefinition(new GridLength(116)),
                new ColumnDefinition(GridLength.Star)
            },
            ColumnSpacing = 5
        };
        demoGrid.Add(demoIcon);
        demoGrid.Add(demoButton, 1);
        demoGrid.Add(demoDescription, 2);
        demoSurface = new Border
        {
            BackgroundColor = Color.FromArgb("#B5121821"),
            Stroke = Color.FromArgb("#9A168CFF"),
            StrokeThickness = 1,
            Padding = new Thickness(10, 2),
            MaximumWidthRequest = 370,
            MinimumHeightRequest = 52,
            HorizontalOptions = LayoutOptions.Center,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
            Content = demoGrid
        };

        liveUi = new VerticalStackLayout
        {
            Spacing = 10,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.End,
            Children = { headerSurface, roleGrid, demoSurface }
        };

        var stage = new Grid();
        stage.Add(backgroundImage);
        stage.Add(new BoxView { Color = Color.FromArgb("#24040910"), InputTransparent = true });
        stage.Add(new ScrollView
        {
            Content = liveUi,
            Padding = 0,
            VerticalScrollBarVisibility = ScrollBarVisibility.Never
        });
        Content = stage;
        SizeChanged += LayoutForViewport;
    }

    private Border CreateRoleCard((string Role, string Description, string Monogram) role)
    {
        var select = new Button
        {
            Text = role.Role.ToUpperInvariant(),
            CommandParameter = role.Role,
            FontSize = 14,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            BackgroundColor = Colors.Transparent,
            BorderWidth = 0,
            MinimumHeightRequest = 48,
            Padding = new Thickness(4, 2)
        };
        SemanticProperties.SetDescription(select, $"Continue as {role.Role}. {role.Description}");
        select.Clicked += RoleClicked;

        var iconLabel = new Label
        {
            Text = role.Monogram,
            FontSize = role.Monogram.Length > 1 ? 10 : 14,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#168CFF"),
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center
        };
        SemanticProperties.SetDescription(iconLabel, $"{role.Role} role icon");
        var icon = new Border
        {
            WidthRequest = 34,
            HeightRequest = 34,
            BackgroundColor = Color.FromArgb("#32168CFF"),
            Stroke = Color.FromArgb("#B8168CFF"),
            StrokeThickness = 1,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 17 },
            Content = iconLabel,
            HorizontalOptions = LayoutOptions.Center
        };
        var description = new Label
        {
            Text = role.Description,
            FontSize = 11,
            TextColor = Color.FromArgb("#C8D2DC"),
            HorizontalTextAlignment = TextAlignment.Center,
            MaxLines = 2
        };
        var cardStack = new VerticalStackLayout
        {
            Spacing = 1,
            VerticalOptions = LayoutOptions.Center,
            Children = { icon, select, description }
        };
        return new Border
        {
            BackgroundColor = Color.FromArgb("#C0121821"),
            Stroke = Color.FromArgb("#706B849F"),
            StrokeThickness = 1,
            Padding = new Thickness(10, 9),
            MinimumHeightRequest = 116,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 },
            Content = cardStack
        };
    }

    private async void RoleClicked(object? sender, EventArgs e)
    {
        var role = (string)((Button)sender!).CommandParameter;
        api.SelectRole(role);
        await Navigation.PushAsync(new LoginPage(api, role));
    }

    private void DemoClicked(object? sender, EventArgs e)
    {
        api.EnterDemoMode();
        Application.Current!.Windows[0].Page = new AppShell();
    }

    private void LayoutForViewport(object? sender, EventArgs e)
    {
        if (Width <= 0 || Height <= 0) return;
        var (deviceClass, orientation) = SportVisualService.ClassifyViewport(Width, Height);
        backgroundImage.Source = visuals.GetChooseRoleBackground(deviceClass, orientation);
        var state = ToLayoutState(deviceClass, orientation);
        if (currentState == state) return;
        currentState = state;
        ConfigureRoleGrid(state);
    }

    private void ConfigureRoleGrid(ChooseRoleLayoutState state)
    {
        roleGrid.Children.Clear();
        roleGrid.RowDefinitions.Clear();
        roleGrid.ColumnDefinitions.Clear();
        var fourAcross = state is ChooseRoleLayoutState.TabletLandscape or ChooseRoleLayoutState.Desktop;
        var columns = fourAcross ? 4 : 2;
        var rows = fourAcross ? 1 : 2;
        for (var i = 0; i < columns; i++) roleGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        for (var i = 0; i < rows; i++) roleGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        for (var i = 0; i < roleCards.Count; i++)
        {
            Grid.SetColumn(roleCards[i], i % columns);
            Grid.SetRow(roleCards[i], i / columns);
            roleGrid.Add(roleCards[i]);
        }

        var phone = state is ChooseRoleLayoutState.PhonePortrait or ChooseRoleLayoutState.PhoneLandscape;
        var portrait = state is ChooseRoleLayoutState.PhonePortrait or ChooseRoleLayoutState.TabletPortrait;
        liveUi.WidthRequest = state switch
        {
            ChooseRoleLayoutState.Desktop => Math.Min(1100, Math.Max(760, Width * .66)),
            ChooseRoleLayoutState.TabletLandscape => Math.Min(1050, Width * .82),
            ChooseRoleLayoutState.TabletPortrait => Math.Min(760, Width * .88),
            ChooseRoleLayoutState.PhoneLandscape => Math.Min(760, Width * .78),
            _ => Math.Max(300, Width * .91)
        };
        liveUi.Padding = new Thickness(Math.Max(12, Width * .03), Math.Max(12, Height * .02),
            Math.Max(12, Width * .03), Math.Max(14, Height * .025));
        liveUi.Spacing = phone ? 7 : 10;
        headerSurface.Padding = phone ? new Thickness(12, 8) : new Thickness(18, 12);
        roleGrid.ColumnSpacing = phone ? 7 : 10;
        roleGrid.RowSpacing = phone ? 7 : 10;
        demoSurface.MinimumHeightRequest = phone ? 48 : 52;
        demoSurface.Padding = phone ? new Thickness(7, 0) : new Thickness(10, 2);
        foreach (var card in roleCards)
        {
            card.MinimumHeightRequest = phone ? (portrait ? 108 : 100) : 124;
            card.Padding = phone ? new Thickness(7, 6) : new Thickness(10, 9);
        }
    }

    private static ChooseRoleLayoutState ToLayoutState(VisualDeviceClass deviceClass, VisualOrientation orientation) =>
        deviceClass switch
        {
            VisualDeviceClass.Desktop => ChooseRoleLayoutState.Desktop,
            VisualDeviceClass.Tablet when orientation == VisualOrientation.Portrait => ChooseRoleLayoutState.TabletPortrait,
            VisualDeviceClass.Tablet => ChooseRoleLayoutState.TabletLandscape,
            VisualDeviceClass.Phone when orientation == VisualOrientation.Portrait => ChooseRoleLayoutState.PhonePortrait,
            _ => ChooseRoleLayoutState.PhoneLandscape
        };
}
