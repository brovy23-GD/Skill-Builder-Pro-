using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SkillBuilderPro.Core.Identity;

namespace SkillBuilderPro.Core.Models;

public sealed class DrillAssignmentRecipient
{
    public int AssignmentId { get; set; }
    public int AthleteUserId { get; set; }

    [Required, MaxLength(20)]
    public string Status { get; set; } = DrillAssignmentRecipientStatuses.Assigned;

    public DateTime? StartedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }

    [MaxLength(1000)]
    public string? AthleteNotes { get; set; }

    [Range(1, 5)]
    public int? Rating { get; set; }

    [JsonIgnore]
    public DrillAssignment Assignment { get; set; } = null!;

    [JsonIgnore]
    public ApplicationUser AthleteUser { get; set; } = null!;
}
