using SkillBuilderPro.MAUI.ViewModels;

namespace SkillBuilderPro.MAUI.Views;

public partial class CategoryListPage : ContentPage
{
    private readonly DrillsViewModel _viewModel;

    public CategoryListPage(DrillsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    private async void BackClicked(object sender, EventArgs e) => await Shell.Current.GoToAsync("..");
    private async void ExitClicked(object sender, EventArgs e) => await Shell.Current.GoToAsync("//Home");
}
