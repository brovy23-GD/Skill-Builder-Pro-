using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkillBuilderPro.MAUI.Models;
using SkillBuilderPro.MAUI.Services;
using System.Collections.ObjectModel;

namespace SkillBuilderPro.MAUI.ViewModels;

public partial class DrillListViewModel : ObservableObject
{
    private readonly ApiService _apiService;

    [ObservableProperty]
    public ObservableCollection<Drill> drills = new();

    [ObservableProperty]
    public bool isLoading = false;

    public DrillListViewModel()
    {
        _apiService = new ApiService();
    }

    [RelayCommand]
    public async Task LoadDrills()
    {
        IsLoading = true;

        var drillList = await _apiService.GetDrillsAsync();
        Drills = new ObservableCollection<Drill>(drillList);

        IsLoading = false;
    }

    [RelayCommand]
    public async Task SelectDrill(Drill drill)
    {
        if (drill == null) return;

        await Shell.Current.GoToAsync($"videoPlayer?drillId={drill.Id}&youtubeUrl={drill.YoutubeUrl}");
    }
}