using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkillBuilderPro.MAUI.Models;
using SkillBuilderPro.MAUI.Services;
using CoreDrill = SkillBuilderPro.Core.Models.Drill;

namespace SkillBuilderPro.MAUI.ViewModels;

public partial class TrainingBuilderViewModel(IAthleteApiService api, ISportVisualService visuals) : LoadableViewModel
{
    private readonly List<CoreDrill> allDrills = [];
    private string? requestedSport;
    private double viewportWidth;
    private double viewportHeight;

    public TrainingSessionDraft Draft { get; } = new();
    public ObservableCollection<string> Sports { get; } = [];
    public ObservableCollection<string> Categories { get; } = [];
    public ObservableCollection<string> SubCategories { get; } = [];
    public ObservableCollection<CoreDrill> AvailableDrills { get; } = [];
    public ObservableCollection<TrainingSessionDraftItem> SessionItems => Draft.Items;

    [ObservableProperty] private string workoutName = string.Empty;
    [ObservableProperty] private string? selectedSport;
    [ObservableProperty] private string? selectedCategory;
    [ObservableProperty] private string? selectedSubCategory;
    [ObservableProperty] private CoreDrill? selectedAvailableDrill;
    [ObservableProperty] private string background = "training_builder_basketball_phone_portrait.png";

    public int TotalDurationMinutes => SessionItems.Sum(item => Math.Max(0, item.DurationMinutes));
    public string TotalDurationLabel => $"{TotalDurationMinutes} MIN";
    public bool HasSessionItems => SessionItems.Count > 0;
    public bool CanAddSelectedDrill => SelectedAvailableDrill is not null;
    public bool CanPreviewAvailableDrill => YouTubeUrl.IsValid(SelectedAvailableDrill?.VideoUrl);
    public bool IsDemoMode => api.IsDemoMode;
    public bool HasAvailableDrills => AvailableDrills.Count > 0;
    public bool NeedsFilterSelection => string.IsNullOrWhiteSpace(SelectedSport) ||
        string.IsNullOrWhiteSpace(SelectedCategory) || string.IsNullOrWhiteSpace(SelectedSubCategory);
    public bool HasNoDrillsForSelection => !IsBusy && !HasError && !NeedsFilterSelection && !HasAvailableDrills;
    public string DrillEmptyState => NeedsFilterSelection
        ? "SELECT A SPORT, CATEGORY, AND SKILL TO FIND DRILLS"
        : "NO DRILLS FOUND FOR THIS SELECTION";

    public void SetRequestedSport(string? sport)
    {
        requestedSport = visuals.SupportedSports.FirstOrDefault(value =>
            string.Equals(value, sport, StringComparison.OrdinalIgnoreCase));
        if (requestedSport is null) return;
        RefreshBackground(requestedSport);
        SelectedSport = requestedSport;
    }

    public void UpdateViewport(double width, double height)
    {
        if (width <= 0 || height <= 0) return;
        viewportWidth = width;
        viewportHeight = height;
        RefreshBackground(SelectedSport ?? requestedSport);
    }

    partial void OnWorkoutNameChanged(string value) => Draft.WorkoutName = value;
    partial void OnSelectedAvailableDrillChanged(CoreDrill? value)
    {
        OnPropertyChanged(nameof(CanAddSelectedDrill));
        OnPropertyChanged(nameof(CanPreviewAvailableDrill));
    }

    partial void OnSelectedSportChanged(string? value)
    {
        Draft.Sport = value;
        RefreshBackground(value);
        SelectedCategory = null;
        SelectedSubCategory = null;
        RefreshFilters();
    }

    partial void OnSelectedCategoryChanged(string? value)
    {
        SelectedSubCategory = null;
        RefreshFilters();
    }

    partial void OnSelectedSubCategoryChanged(string? value) => RefreshAvailableDrills();

    [RelayCommand]
    private async Task Load() => await LoadGuard(async () =>
    {
        allDrills.Clear();
        IEnumerable<CoreDrill> source;
        if (api.IsDemoMode)
        {
            source = DemoDataService.Drills;
            ErrorMessage = null;
        }
        else
        {
            var apiDrills = await api.GetAsync<List<CoreDrill>>("api/drills");
            source = apiDrills ?? [];
            ErrorMessage = apiDrills is not null
                ? null
                : api.ServiceStatusMessage ?? "Drills could not be loaded from the service. Try again.";
        }

        allDrills.AddRange(source);
        Sports.Reset(visuals.SupportedSports.Where(sport => !string.Equals(sport, "Strength", StringComparison.OrdinalIgnoreCase)));
        var preferredSport = requestedSport ?? SelectedSport ?? api.User?.Sport ?? (api.IsDemoMode ? DemoDataService.Sport : null);
        var supportedPreferredSport = visuals.SupportedSports.FirstOrDefault(sport =>
            string.Equals(sport, preferredSport, StringComparison.OrdinalIgnoreCase));
        SelectedSport = Sports.FirstOrDefault(sport =>
            string.Equals(sport, supportedPreferredSport ?? preferredSport, StringComparison.OrdinalIgnoreCase))
            ?? Sports.FirstOrDefault(sport => allDrills.Any(drill => string.Equals(drill.Sport, sport, StringComparison.OrdinalIgnoreCase)))
            ?? Sports.FirstOrDefault();
        RefreshBackground(SelectedSport);
        RefreshFilters();
        OnPropertyChanged(nameof(HasError));
    });

    [RelayCommand]
    private void AddSelectedDrill(CoreDrill? drill)
    {
        drill ??= SelectedAvailableDrill;
        if (drill is null) return;
        var item = new TrainingSessionDraftItem
        {
            Drill = drill,
            Reps = 1,
            DurationMinutes = ParseDurationMinutes(drill.Duration),
            Order = SessionItems.Count + 1
        };
        item.PropertyChanged += SessionItemChanged;
        SessionItems.Add(item);
        if (SelectedAvailableDrill?.Id == drill.Id) SelectedAvailableDrill = null;
        NotifySessionChanged();
    }

    [RelayCommand]
    private void RemoveDrill(TrainingSessionDraftItem? item)
    {
        if (item is null || !SessionItems.Remove(item)) return;
        item.PropertyChanged -= SessionItemChanged;
        NormalizeOrder();
    }

    [RelayCommand]
    private void MoveDrillUp(TrainingSessionDraftItem? item)
    {
        if (item is null) return;
        var index = SessionItems.IndexOf(item);
        if (index <= 0) return;
        SessionItems.Move(index, index - 1);
        NormalizeOrder();
    }

    [RelayCommand]
    private void MoveDrillDown(TrainingSessionDraftItem? item)
    {
        if (item is null) return;
        var index = SessionItems.IndexOf(item);
        if (index < 0 || index >= SessionItems.Count - 1) return;
        SessionItems.Move(index, index + 1);
        NormalizeOrder();
    }

    [RelayCommand]
    private async Task PreviewAvailableDrill(CoreDrill? drill)
    {
        drill ??= SelectedAvailableDrill;
        if (drill is null || !YouTubeUrl.IsValid(drill.VideoUrl)) return;
        await OpenDrillPreviewAsync(drill.Id);
    }

    [RelayCommand]
    private async Task PreviewDrill(TrainingSessionDraftItem? item)
    {
        if (item is null) return;
        await OpenDrillPreviewAsync(item.DrillId);
    }

    [RelayCommand]
    private async Task ExitDemo()
    {
        if (!api.IsDemoMode) return;
        await api.LogoutAsync();
        Application.Current!.Windows[0].Page = new NavigationPage(new Views.ChooseProfilePage(api));
    }

    private static Task OpenDrillPreviewAsync(int drillId) =>
        Shell.Current.GoToAsync($"{nameof(Views.DrillLibraryPage)}?drillId={drillId}&fromTraining=true");

    private void RefreshFilters()
    {
        var sportRows = allDrills.Where(drill => string.IsNullOrWhiteSpace(SelectedSport) ||
            string.Equals(drill.Sport, SelectedSport, StringComparison.OrdinalIgnoreCase)).ToList();
        Categories.Reset(sportRows.Select(drill => drill.Category)
            .Where(category => !string.IsNullOrWhiteSpace(category))
            .Distinct(StringComparer.OrdinalIgnoreCase).Order());
        var categoryRows = sportRows.Where(drill => string.IsNullOrWhiteSpace(SelectedCategory) ||
            string.Equals(drill.Category, SelectedCategory, StringComparison.OrdinalIgnoreCase));
        SubCategories.Reset(categoryRows.Select(drill => drill.SubCategory)
            .Where(subCategory => !string.IsNullOrWhiteSpace(subCategory)).Select(subCategory => subCategory!)
            .Distinct(StringComparer.OrdinalIgnoreCase).Order());
        RefreshAvailableDrills();
    }

    private void RefreshAvailableDrills()
    {
        if (NeedsFilterSelection)
        {
            AvailableDrills.Clear();
        }
        else AvailableDrills.Reset(allDrills.Where(drill =>
            (string.IsNullOrWhiteSpace(SelectedSport) || string.Equals(drill.Sport, SelectedSport, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrWhiteSpace(SelectedCategory) || string.Equals(drill.Category, SelectedCategory, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrWhiteSpace(SelectedSubCategory) || string.Equals(drill.SubCategory, SelectedSubCategory, StringComparison.OrdinalIgnoreCase)))
            .OrderBy(drill => drill.Name));
        if (SelectedAvailableDrill is not null && !AvailableDrills.Any(drill => drill.Id == SelectedAvailableDrill.Id))
            SelectedAvailableDrill = null;
        OnPropertyChanged(nameof(HasAvailableDrills));
        OnPropertyChanged(nameof(NeedsFilterSelection));
        OnPropertyChanged(nameof(HasNoDrillsForSelection));
        OnPropertyChanged(nameof(DrillEmptyState));
    }

    private void SessionItemChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TrainingSessionDraftItem.DurationMinutes)) NotifySessionChanged();
    }

    private void NormalizeOrder()
    {
        for (var index = 0; index < SessionItems.Count; index++) SessionItems[index].Order = index + 1;
        NotifySessionChanged();
    }

    private void NotifySessionChanged()
    {
        OnPropertyChanged(nameof(TotalDurationMinutes));
        OnPropertyChanged(nameof(TotalDurationLabel));
        OnPropertyChanged(nameof(HasSessionItems));
    }

    private void RefreshBackground(string? sport)
    {
        Background = viewportWidth > 0 && viewportHeight > 0
            ? visuals.GetTrainingBuilderBackground(sport, viewportWidth, viewportHeight)
            : visuals.GetTrainingBuilderBackground(sport);
    }

    private static int ParseDurationMinutes(string? duration)
    {
        var match = Regex.Match(duration ?? string.Empty, @"\d+");
        return match.Success && int.TryParse(match.Value, out var minutes) ? Math.Max(1, minutes) : 1;
    }
}
