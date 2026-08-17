using System.ComponentModel.DataAnnotations;
using SkillBuilderPro.Core.Identity;

namespace SkillBuilderPro.Core.Models;

public static class GoalTypes { public const string QualifyingCompletions = "QualifyingCompletions"; public const string SkillLevel = "SkillLevel"; public const string OverallRank = "OverallRank"; public const string TrainingStreak = "TrainingStreak"; public static readonly string[] All = [QualifyingCompletions, SkillLevel, OverallRank, TrainingStreak]; }
public static class GoalStatuses { public const string Active = "Active"; public const string Completed = "Completed"; public const string Cancelled = "Cancelled"; public static readonly string[] All = [Active, Completed, Cancelled]; }

public sealed class AthleteGoal
{
    public int Id { get; set; }
    public int AthleteUserId { get; set; }
    public int CreatedByUserId { get; set; }
    [MaxLength(32)] public string CreatedByRole { get; set; } = string.Empty;
    [MaxLength(32)] public string GoalType { get; set; } = string.Empty;
    [MaxLength(100)] public string? Sport { get; set; }
    [MaxLength(100)] public string? Category { get; set; }
    [MaxLength(100)] public string? SubCategory { get; set; }
    public int TargetValue { get; set; }
    [MaxLength(20)] public string Status { get; set; } = GoalStatuses.Active;
    [MaxLength(150)] public string Title { get; set; } = string.Empty;
    [MaxLength(1000)] public string? Description { get; set; }
    public DateTime? DueAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public DateTime? CancelledAtUtc { get; set; }
    public ApplicationUser AthleteUser { get; set; } = null!;
    public ApplicationUser CreatedByUser { get; set; } = null!;
}
