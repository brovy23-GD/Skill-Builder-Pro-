using System;
using SkillBuilderPro.MAUI.Views;

namespace SkillBuilderPro.MAUI;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnBrowseSportsClicked(
        object sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync("//SportListPage");
    }
}