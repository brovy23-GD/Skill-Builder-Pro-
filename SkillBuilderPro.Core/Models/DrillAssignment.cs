using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SkillBuilderPro.Core.Identity;

namespace SkillBuilderPro.Core.Models;

public sealed class DrillAssignment
{
    public int Id { get; set; }
    public int DrillId { get; set; }
    public int AssignedByUserId { get; set; }
    public int? SourceTeamId { get; set; }
    public DateTime? ScheduledForUtc { get; set; }
    public DateTime? DueAtUtc { get; set; }

    [MaxLength(1000)]
    public string? Instructions { get; set; }

    [Required, MaxLength(20)]
    public string Status { get; set; } = DrillAssignmentStatuses.Active;

    public bool CountsTowardProgression { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? CancelledAtUtc { get; set; }

    [JsonIgnore]
    public Drill Drill { get; set; } = null!;

    [JsonIgnore]
    public ApplicationUser AssignedByUser { get; set; } = null!;

    [JsonIgnore]
    public Team? SourceTeam { get; set; }

    [JsonIgnore]
    public ICollection<DrillAssignmentRecipient> Recipients { get; set; } = [];
}
