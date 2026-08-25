using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CoreDrill = SkillBuilderPro.Core.Models.Drill;

namespace SkillBuilderPro.MAUI.Models;

public sealed class TrainingSessionDraft
{
    public string WorkoutName { get; set; } = string.Empty;
    public string? Sport { get; set; }
    public ObservableCollection<TrainingSessionDraftItem> Items { get; } = [];
}

public partial class TrainingSessionDraftItem : ObservableObject
{
    public required CoreDrill Drill { get; init; }
    public int DrillId => Drill.Id;
    public string Name => Drill.Name;

    [ObservableProperty]
    private int reps = 1;

    [ObservableProperty]
    private int durationMinutes = 1;

    [ObservableProperty]
    private int order;
}
