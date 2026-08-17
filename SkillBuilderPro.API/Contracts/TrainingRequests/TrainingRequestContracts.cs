using System.ComponentModel.DataAnnotations;
using SkillBuilderPro.API.Contracts.Goals;

namespace SkillBuilderPro.API.Contracts.TrainingRequests;

public sealed class CreateTrainingRequest
{
    [Range(1, int.MaxValue)] public int RequestedRecipientUserId { get; set; }
    public int? TeamId { get; set; }
    public int? RequestedDrillId { get; set; }
    [MaxLength(100)] public string? Sport { get; set; }
    [MaxLength(100)] public string? Category { get; set; }
    [MaxLength(100)] public string? SubCategory { get; set; }
    [MaxLength(1000)] public string? Message { get; set; }
}
public sealed class ApproveTrainingRequest
{
    [Range(1, int.MaxValue)] public int DrillId { get; set; }
    public DateTime? ScheduledForUtc { get; set; }
    public DateTime? DueAtUtc { get; set; }
    [MaxLength(1000)] public string? Instructions { get; set; }
    public bool CountsTowardProgression { get; set; } = true;
}
public sealed record DrillSummary(int DrillId, string Name, string Sport, string? Category, string? SubCategory);
public sealed record TrainingRequestResponse(int RequestId, UserSummary Athlete, UserSummary Recipient, string RecipientRole, int? TeamId, string? TeamName, DrillSummary? RequestedDrill, string? Sport, string? Category, string? SubCategory, string? Message, string Status, DateTime CreatedAtUtc, DateTime? RespondedAtUtc, DateTime? CancelledAtUtc, int? ApprovedAssignmentId);
