using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SkillBuilderPro.Core.Identity;

namespace SkillBuilderPro.Core.Models;

public static class AssignmentEventTypes
{
    public const string RecipientCompleted = "AssignmentRecipientCompleted";
}

public sealed class AssignmentCompletionEvent
{
    public long Id { get; set; }
    public int AssignmentId { get; set; }
    public int AthleteUserId { get; set; }
    public int DrillId { get; set; }

    [Required, MaxLength(50)]
    public string EventType { get; set; } = AssignmentEventTypes.RecipientCompleted;

    public DateTime OccurredAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? ProcessedAtUtc { get; set; }
    public int ProcessingAttempts { get; set; }

    [MaxLength(2000)]
    public string? LastError { get; set; }

    [JsonIgnore]
    public DrillAssignment Assignment { get; set; } = null!;

    [JsonIgnore]
    public ApplicationUser AthleteUser { get; set; } = null!;

    [JsonIgnore]
    public Drill Drill { get; set; } = null!;
}
