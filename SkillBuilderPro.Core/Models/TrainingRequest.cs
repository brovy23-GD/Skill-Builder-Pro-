using System.ComponentModel.DataAnnotations;
using SkillBuilderPro.Core.Identity;

namespace SkillBuilderPro.Core.Models;

public static class TrainingRequestStatuses { public const string Pending = "Pending"; public const string Approved = "Approved"; public const string Declined = "Declined"; public const string Cancelled = "Cancelled"; public static readonly string[] All = [Pending, Approved, Declined, Cancelled]; }

public sealed class TrainingRequest
{
    public int Id { get; set; }
    public int AthleteUserId { get; set; }
    public int RequestedRecipientUserId { get; set; }
    [MaxLength(20)] public string RequestedRecipientRole { get; set; } = string.Empty;
    public int? TeamId { get; set; }
    public int? RequestedDrillId { get; set; }
    [MaxLength(100)] public string? Sport { get; set; }
    [MaxLength(100)] public string? Category { get; set; }
    [MaxLength(100)] public string? SubCategory { get; set; }
    [MaxLength(1000)] public string? Message { get; set; }
    [MaxLength(20)] public string Status { get; set; } = TrainingRequestStatuses.Pending;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? RespondedAtUtc { get; set; }
    public DateTime? CancelledAtUtc { get; set; }
    public int? ApprovedAssignmentId { get; set; }
    public ApplicationUser AthleteUser { get; set; } = null!;
    public ApplicationUser RequestedRecipientUser { get; set; } = null!;
    public Team? Team { get; set; }
    public Drill? RequestedDrill { get; set; }
    public DrillAssignment? ApprovedAssignment { get; set; }
}
