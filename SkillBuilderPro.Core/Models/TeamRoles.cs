namespace SkillBuilderPro.Core.Models;

public static class TeamRoles
{
    public const string HeadCoach = "HeadCoach";
    public const string AssistantCoach = "AssistantCoach";
    public const string SkillsCoach = "SkillsCoach";

    public static readonly IReadOnlyCollection<string> All =
    [
        HeadCoach,
        AssistantCoach,
        SkillsCoach
    ];
}
