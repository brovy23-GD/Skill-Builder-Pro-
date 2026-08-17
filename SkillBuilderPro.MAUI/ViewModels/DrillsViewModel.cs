using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkillBuilderPro.Client.ApiClients;
using SkillBuilderPro.Core.Models;
using SkillBuilderPro.MAUI.Views;

namespace SkillBuilderPro.MAUI.ViewModels;

public partial class DrillsViewModel : ObservableObject
{
    private readonly DrillApiClient _drillApiClient;
    private List<Drill> _allDrills = new();

    public ObservableCollection<string> Sports { get; } = new();
    public ObservableCollection<string> Categories { get; } = new();
    public ObservableCollection<Drill> FilteredDrills { get; } = new();

    // ? TYPE ALIGNMENT: Strongly typed collection to match your DrillListPage cast logic exactly
    public ObservableCollection<Drill> SelectedDrills { get; } = new();

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string statusMessage = "Ready";

    [ObservableProperty]
    private string selectedSport = string.Empty;

    [ObservableProperty]
    private string selectedCategory = string.Empty;

    public DrillsViewModel(DrillApiClient drillApiClient)
    {
        _drillApiClient = drillApiClient;
    }

    [RelayCommand]
    public async Task LoadAllDrillsAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            StatusMessage = "Loading training library...";

            var items = await _drillApiClient.GetAllAsync();

            _allDrills.Clear();
            Sports.Clear();
            Categories.Clear();
            FilteredDrills.Clear();
            SelectedDrills.Clear();

            if (items is null || items.Count == 0)
            {
                StatusMessage = "No drills found.";
                return;
            }

            _allDrills = items.ToList();

            var sports = _allDrills
                .Select(d => d.Sport)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(s => s)
                .ToList();

            foreach (var sport in sports)
                Sports.Add(sport);

            StatusMessage = $"Loaded {Sports.Count} sports.";
        }
        catch (Exception ex)
        {
            StatusMessage = "Failed to load drill data.";
            System.Diagnostics.Debug.WriteLine($"LoadAllDrillsAsync error: {ex}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task SelectSportAsync(string sportName)
    {
        if (string.IsNullOrWhiteSpace(sportName)) return;

        SelectedSport = sportName;
        SelectedCategory = string.Empty;

        Categories.Clear();
        FilteredDrills.Clear();
        SelectedDrills.Clear();

        var categories = _allDrills
            .Where(d => string.Equals(d.Sport, sportName, StringComparison.OrdinalIgnoreCase))
            .Select(d => d.Category)
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(c => c)
            .ToList();

        foreach (var category in categories)
            Categories.Add(category);

        StatusMessage = $"{SelectedSport}: {Categories.Count} categories.";
        await Shell.Current.GoToAsync(nameof(CategoryListPage));
    }

    [RelayCommand]
    public async Task SelectCategoryAsync(string categoryName)
    {
        if (string.IsNullOrWhiteSpace(categoryName)) return;

        SelectedCategory = categoryName;
        FilteredDrills.Clear();
        SelectedDrills.Clear();

        List<Drill> drills;

        // ? FIXED CATCH-ALL LOOKUP:
        // If a user clicks a generic folder like "Offense", pull anything that isn't a dummy test row!
        if (categoryName.Equals("Offense", StringComparison.OrdinalIgnoreCase) ||
            categoryName.Equals("Defense", StringComparison.OrdinalIgnoreCase))
        {
            drills = _allDrills
                .Where(d => string.Equals(d.Sport, SelectedSport, StringComparison.OrdinalIgnoreCase) &&
                           !d.SubCategory.Contains("System Integration Testing")) // Hides the dummy placeholding blocks!
                .OrderBy(d => d.Category)
                .ThenBy(d => d.Name)
                .ToList();
        }
        else
        {
            // Standard narrow category filtering matching your spreadsheet parameters explicitly
            drills = _allDrills
                .Where(d => string.Equals(d.Sport, SelectedSport, StringComparison.OrdinalIgnoreCase) &&
                            string.Equals(d.Category, categoryName, StringComparison.OrdinalIgnoreCase))
                .OrderBy(d => d.Name)
                .ToList();
        }

        foreach (var drill in drills)
            FilteredDrills.Add(drill);

        StatusMessage = $"{SelectedSport} Library: {FilteredDrills.Count} authentic training items loaded.";
        await Shell.Current.GoToAsync(nameof(Views.DrillListPage));
    }

    // ? ADDED NAVIGATION SUBMISSION: Maps your selected items data directly to your target screen
    [RelayCommand]
    public async Task PlaySelectedDrillAsync()
    {
        var chosenDrill = SelectedDrills.FirstOrDefault();

        if (chosenDrill == null)
        {
            await App.Current?.MainPage?.DisplayAlert("Selection Required", "Please select a drill from the list first.", "OK")!;
            return;
        }

        if (string.IsNullOrWhiteSpace(chosenDrill.VideoUrl))
        {
            await App.Current?.MainPage?.DisplayAlert("Video Unloaded", $"The video asset for '{chosenDrill.Name}' is missing from the data resource.", "OK")!;
            return;
        }

        // Builds URL string encoding parameters matching your secure cross-platform route targets
        string route = $"{nameof(DrillLibraryPage)}?drillId={chosenDrill.Id}";
        await Shell.Current.GoToAsync(route);
    }

    [RelayCommand]
    public void ClearSelection()
    {
        SelectedDrills.Clear();
        StatusMessage = "Selection cleared.";
    }
}
