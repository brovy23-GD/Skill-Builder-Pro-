namespace SkillBuilderPro.Core.Models;

public static class DrillAssignmentStatuses
{
    public const string Scheduled = "Scheduled";
    public const string Active = "Active";
    public const string Cancelled = "Cancelled";
    public const string Closed = "Closed";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal)
    {
        Scheduled,
        Active,
        Cancelled,
        Closed
    };
}

public static class DrillAssignmentRecipientStatuses
{
    public const string Assigned = "Assigned";
    public const string InProgress = "InProgress";
    public const string Completed = "Completed";
    public const string Missed = "Missed";
    public const string Excused = "Excused";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal)
    {
        Assigned,
        InProgress,
        Completed,
        Missed,
        Excused
    };
}
