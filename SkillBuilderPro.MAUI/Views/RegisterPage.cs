using SkillBuilderPro.MAUI.Services;

namespace SkillBuilderPro.MAUI.Views;

public partial class RegisterPage : ContentPage
{
    readonly IAthleteApiService api;
    readonly string role;
    string? selectedPhotoPath;

    public IReadOnlyList<string> FeetUnits { get; } = ["ft"];

    static readonly string[] Sports = ["Baseball", "Basketball", "Football", "Hockey", "Soccer", "Softball"];
    static readonly Dictionary<string, string[]> Positions = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Baseball"] = ["Pitcher", "Catcher", "Infield", "Outfield", "Utility"],
        ["Softball"] = ["Pitcher", "Catcher", "Infield", "Outfield", "Utility"],
        ["Basketball"] = ["Guard", "Wing", "Forward", "Center"],
        ["Football"] = ["Quarterback", "Running Back", "Receiver", "Line", "Linebacker", "Defensive Back"],
        ["Soccer"] = ["Goalkeeper", "Defender", "Midfielder", "Forward"],
        ["Hockey"] = ["Goalie", "Defense", "Center", "Wing"]
    };

    public RegisterPage(IAthleteApiService api, string role)
    {
        InitializeComponent();
        BindingContext = this;
        this.api = api;
        this.role = role;
        Title = $"Create {role} Profile";
        SportPicker.ItemsSource = Sports;
        DominantPicker.ItemsSource = new[] { "Left", "Right", "Both" };
    }

    void StageSizeChanged(object? sender, EventArgs e)
    {
        if (Stage.Width <= 0 || Stage.Height <= 0) return;
        var phone = Stage.Width < 600;
        var tablet = !phone && Stage.Width < 960;
        ContentStack.Padding = phone ? new Thickness(16, 18, 16, 28) : new Thickness(24, 22, 24, 30);

        FormGrid.ColumnDefinitions.Clear();
        FormGrid.RowDefinitions.Clear();
        if (phone)
        {
            FormGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            for (var i = 0; i < 11; i++) FormGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            Place(PhotoGroup, 0); Place(NameGroup, 1); Place(TeamGroup, 2); Place(HeightGroup, 3);
            Place(WeightGroup, 4); Place(SportGroup, 5); Place(PositionGroup, 6); Place(JerseyGroup, 7);
            Place(AgeGroup, 8); Place(DominantGroup, 9); Place(BioGroup, 10);
            PhotoFrame.HeightRequest = 190;
            ActionGrid.ColumnDefinitions.Clear();
            ActionGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            ActionGrid.RowDefinitions.Clear();
            for (var i = 0; i < 3; i++) ActionGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            for (var i = 0; i < ActionGrid.Children.Count; i++) { ActionGrid.SetColumn(ActionGrid.Children[i], 0); ActionGrid.SetRow(ActionGrid.Children[i], i); }
            ActionGrid.RowSpacing = 9;
        }
        else
        {
            var columns = tablet ? 2 : 4;
            for (var i = 0; i < columns; i++) FormGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            var rows = tablet ? 6 : 4;
            for (var i = 0; i < rows; i++) FormGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            if (tablet)
            {
                Place(PhotoGroup, 0, 0); FormGrid.SetRowSpan(PhotoGroup, 3);
                Place(NameGroup, 0, 1); Place(TeamGroup, 1, 1); Place(HeightGroup, 2, 1);
                Place(WeightGroup, 3, 0); Place(SportGroup, 3, 1); Place(PositionGroup, 4, 0);
                Place(JerseyGroup, 4, 1); Place(AgeGroup, 5, 0); Place(DominantGroup, 5, 1);
                Place(BioGroup, 6, 0); FormGrid.SetColumnSpan(BioGroup, 2);
                FormGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            }
            else
            {
                Place(PhotoGroup, 0, 0); FormGrid.SetRowSpan(PhotoGroup, 3);
                Place(NameGroup, 0, 1); Place(SportGroup, 0, 2); Place(JerseyGroup, 0, 3);
                Place(TeamGroup, 1, 1); Place(PositionGroup, 1, 2); Place(AgeGroup, 1, 3);
                Place(HeightGroup, 2, 1); Place(WeightGroup, 2, 2); Place(DominantGroup, 2, 3);
                Place(BioGroup, 3, 1); FormGrid.SetColumnSpan(BioGroup, 3);
            }
            PhotoFrame.HeightRequest = 210;
            ActionGrid.RowDefinitions.Clear(); ActionGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            ActionGrid.ColumnDefinitions.Clear();
            ActionGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            ActionGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1.35, GridUnitType.Star)));
            ActionGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            for (var i = 0; i < ActionGrid.Children.Count; i++) { ActionGrid.SetRow(ActionGrid.Children[i], 0); ActionGrid.SetColumn(ActionGrid.Children[i], i); }
            ActionGrid.RowSpacing = 0;
        }
    }

    void Place(IView view, int row, int column = 0)
    {
        FormGrid.SetRow(view, row); FormGrid.SetColumn(view, column); FormGrid.SetRowSpan(view, 1); FormGrid.SetColumnSpan(view, 1);
    }

    async void UploadPhotoClicked(object? sender, EventArgs e)
    {
        var result = await FilePicker.Default.PickAsync(new PickOptions { PickerTitle = "Choose athlete photo", FileTypes = FilePickerFileType.Images });
        if (result is null) return;
        selectedPhotoPath = result.FullPath;
        PhotoPreview.Source = ImageSource.FromFile(result.FullPath);
        PhotoPreview.IsVisible = true;
        PhotoPlaceholder.IsVisible = false;
    }

    void SportChanged(object? sender, EventArgs e)
    {
        PositionPicker.ItemsSource = SportPicker.SelectedItem is string sport && Positions.TryGetValue(sport, out var values) ? values : [];
        PositionPicker.SelectedIndex = -1;
    }

    void BioTextChanged(object? sender, TextChangedEventArgs e) => BioCounter.Text = $"{e.NewTextValue?.Length ?? 0} / 250";

    void ClearClicked(object? sender, EventArgs e)
    {
        foreach (var entry in new[] { NameEntry, TeamEntry, HeightFeetEntry, HeightInchesEntry, WeightEntry, JerseyEntry, AgeEntry }) entry.Text = string.Empty;
        BioEditor.Text = string.Empty;
        SportPicker.SelectedIndex = PositionPicker.SelectedIndex = DominantPicker.SelectedIndex = -1;
        selectedPhotoPath = null;
        PhotoPreview.Source = null;
        PhotoPreview.IsVisible = false;
        PhotoPlaceholder.IsVisible = true;
        ValidationLabel.IsVisible = false;
    }

    async void ContinueClicked(object? sender, EventArgs e)
    {
        var issues = new List<string>();
        if (string.IsNullOrWhiteSpace(NameEntry.Text)) issues.Add("Athlete name is required.");
        if (SportPicker.SelectedItem is null) issues.Add("Primary sport is required.");
        if (PositionPicker.SelectedItem is null) issues.Add("Position is required.");
        if (!TryOptionalInt(JerseyEntry.Text, 0, 999)) issues.Add("Jersey number must be 0–999.");
        if (!TryOptionalInt(AgeEntry.Text, 1, 120)) issues.Add("Age must be 1–120.");
        if (!TryOptionalInt(HeightFeetEntry.Text, 1, 8) || !TryOptionalInt(HeightInchesEntry.Text, 0, 11)) issues.Add("Height must use valid feet and inches.");
        if (!TryOptionalInt(WeightEntry.Text, 1, 999)) issues.Add("Weight must be a valid number.");
        if (issues.Count > 0) { ValidationLabel.Text = string.Join("\n", issues); ValidationLabel.IsVisible = true; return; }

        ValidationLabel.IsVisible = false;
        var email = await DisplayPromptAsync("Secure your account", "Email", keyboard: Keyboard.Email);
        if (string.IsNullOrWhiteSpace(email)) return;
        var passwordPage = new PasswordCapturePage();
        await Navigation.PushModalAsync(new NavigationPage(passwordPage));
        var password = await passwordPage.Result;
        if (string.IsNullOrWhiteSpace(password)) return;
        var result = await api.RegisterAsync(email.Trim(), password, NameEntry.Text!.Trim(), role);
        if (result.Ok) Application.Current!.Windows[0].Page = ShellFactory.Create(api);
        else { ValidationLabel.Text = result.Error; ValidationLabel.IsVisible = true; }
    }

    async void SignInClicked(object? sender, EventArgs e) => await Navigation.PopAsync();

    static bool TryOptionalInt(string? value, int min, int max) => string.IsNullOrWhiteSpace(value) || (int.TryParse(value, out var parsed) && parsed >= min && parsed <= max);
}
