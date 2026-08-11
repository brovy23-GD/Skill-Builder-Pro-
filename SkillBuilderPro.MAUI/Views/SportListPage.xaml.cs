using SkillBuilderPro.MAUI.ViewModels;

namespace SkillBuilderPro.MAUI.Views;

public partial class SportListPage : ContentPage
{
    private readonly DrillsViewModel _viewModel;

    public SportListPage(DrillsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_viewModel.Sports.Count == 0)
            await _viewModel.LoadAllDrillsAsync();
    }
}