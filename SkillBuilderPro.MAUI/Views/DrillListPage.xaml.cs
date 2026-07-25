using SkillBuilderPro.MAUI.ViewModels;

namespace SkillBuilderPro.MAUI.Views;

public partial class DrillListPage : ContentPage
{
    public DrillListPage()
    {
        InitializeComponent();
        BindingContext = new DrillListViewModel();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var viewModel = (DrillListViewModel)BindingContext;
        await viewModel.LoadDrillsCommand.ExecuteAsync(null);
    }
}