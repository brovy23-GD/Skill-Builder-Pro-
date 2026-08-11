using System;
using Microsoft.Maui.Controls;
using SkillBuilderPro.Core.Models;
using SkillBuilderPro.MAUI.ViewModels;
using SkillBuilderPro.MAUI.Views;

namespace SkillBuilderPro.MAUI.Views;

public partial class DrillListPage : ContentPage
{
    private readonly DrillsViewModel _viewModel;

    public DrillListPage(DrillsViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    private void OnCollectionViewSelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        _viewModel.SelectedDrills.Clear();

        foreach (var item in e.CurrentSelection)
        {
            if (item is Drill drill)
                _viewModel.SelectedDrills.Add(drill);
        }

        if (_viewModel.SelectedDrills.Count > 5)
        {
            DrillsCollection.SelectedItems = null;
            _viewModel.SelectedDrills.Clear();
            _viewModel.StatusMessage =
                "You can only select up to 5 drills.";

            return;
        }

        _viewModel.StatusMessage =
            $"Selected {_viewModel.SelectedDrills.Count} drills.";
    }

    private async void OnPlayDrillsClicked(
    object sender,
    EventArgs e)
    {
        if (_viewModel.SelectedDrills.Count == 0)
        {
            await DisplayAlert(
                "Selection Required",
                "Please select at least one drill.",
                "OK");

            return;
        }

        var selectedDrill = _viewModel.SelectedDrills[0];

        if (string.IsNullOrWhiteSpace(selectedDrill.VideoUrl))
        {
            await DisplayAlert(
                "Video Unavailable",
                $"The video for '{selectedDrill.Name}' is missing.",
                "OK");

            return;
        }

        string videoUrl =
            Uri.EscapeDataString(selectedDrill.VideoUrl);

        string drillName =
            Uri.EscapeDataString(selectedDrill.Name ?? "Drill");

        string route =
            $"{nameof(DrillLibraryPage)}" +
            $"?videoUrl={videoUrl}" +
            $"&drillName={drillName}";

        await Shell.Current.GoToAsync(route);
    }

    private void OnClearDrillsClicked(
        object sender,
        EventArgs e)
    {
        DrillsCollection.SelectedItems = null;
        _viewModel.SelectedDrills.Clear();
        _viewModel.StatusMessage = "Selection cleared.";
    }
}