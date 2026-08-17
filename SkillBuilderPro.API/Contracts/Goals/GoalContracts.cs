using System.ComponentModel.DataAnnotations;

namespace SkillBuilderPro.API.Contracts.Goals;

public sealed class CreateGoalRequest
{
    [Required, MaxLength(32)] public string GoalType { get; set; } = string.Empty;
    [MaxLength(100)] public string? Sport { get; set; }
    [MaxLength(100)] public string? Category { get; set; }
    [MaxLength(100)] public string? SubCategory { get; set; }
    [Range(1, 100000)] public int TargetValue { get; set; }
    [Required, MaxLength(150)] public string Title { get; set; } = string.Empty;
    [MaxLength(1000)] public string? Description { get; set; }
    public DateTime? DueAtUtc { get; set; }
}

public sealed record UserSummary(int UserId, string DisplayName, string Role);
public sealed record GoalResponse(int GoalId, int AthleteUserId, UserSummary CreatedBy, string GoalType, string? Sport, string? Category, string? SubCategory, string Title, string? Description, int TargetValue, string? TargetDisplayName, int CurrentValue, int ProgressPercent, string Status, bool IsComplete, bool IsOverdue, DateTime? DueAtUtc, DateTime CreatedAtUtc, DateTime UpdatedAtUtc, DateTime? CompletedAtUtc, DateTime? CancelledAtUtc);
