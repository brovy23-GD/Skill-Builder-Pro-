using CommunityToolkit.Mvvm.ComponentModel;
using SkillBuilderPro.MAUI.Models;

namespace SkillBuilderPro.MAUI.ViewModels;

public partial class DrillItemViewModel : ObservableObject
{
    private readonly Drill _drill;

    [ObservableProperty]
    private bool isSelected = false;

    public Drill Drill => _drill;

    public DrillItemViewModel(Drill drill)
    {
        _drill = drill;
    }
}