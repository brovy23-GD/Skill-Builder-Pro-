namespace SkillBuilderPro.MAUI.Services;

public interface ISportVisualService
{
    IReadOnlyList<string> SupportedSports { get; }
    string GetTrainingBackground(string? sport);
}

public sealed class SportVisualService : ISportVisualService
{
    private static readonly IReadOnlyDictionary<string,string> Backgrounds =
        new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Basketball"]="basketball_training.png",
            ["Football"]="football_training.png",
            ["Baseball"]="baseball_training.png",
            ["Softball"]="softball_training.png",
            ["Soccer"]="soccer_training.png",
            ["Hockey"]="hockey_training.png",
            ["Strength"]="strength_training.png"
        };
    public IReadOnlyList<string> SupportedSports { get; } = Backgrounds.Keys.ToArray();
    public string GetTrainingBackground(string? sport) =>
        sport is not null && Backgrounds.TryGetValue(sport,out var asset) ? asset : "strength_training.png";
}
