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
}