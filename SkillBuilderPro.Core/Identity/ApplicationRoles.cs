namespace SkillBuilderPro.Core.Identity;

public static class ApplicationRoles
{
    public const string Athlete = "Athlete";
    public const string Parent = "Parent";
    public const string Coach = "Coach";
    public const string Administrator = "Administrator";

    public static readonly IReadOnlyCollection<string> All =
    [
        Athlete,
        Parent,
        Coach,
        Administrator
    ];
}
