using System.ComponentModel.DataAnnotations;

namespace SkillBuilderPro.API.Contracts.Assignments;

public sealed class ParentAssignmentRequest
{
    [Range(1, int.MaxValue)]
    public int DrillId { get; set; }

    [Required, MinLength(1)]
    public List<int> AthleteUserIds { get; set; } = [];

    public DateTime? ScheduledForUtc { get; set; }
    public DateTime? DueAtUtc { get; set; }

    [MaxLength(1000)]
    public string? Instructions { get; set; }

    public bool CountsTowardProgression { get; set; } = true;
}

public sealed class TeamAssignmentRequest
{
    [Range(1, int.MaxValue)]
    public int DrillId { get; set; }
    public DateTime? ScheduledForUtc { get; set; }
    public DateTime? DueAtUtc { get; set; }

    [MaxLength(1000)]
    public string? Instructions { get; set; }

    public bool CountsTowardProgression { get; set; } = true;
}

public sealed class SelectedTeamAssignmentRequest
{
    [Range(1, int.MaxValue)]
    public int DrillId { get; set; }

    [Required, MinLength(1)]
    public List<int> AthleteUserIds { get; set; } = [];

    public DateTime? ScheduledForUtc { get; set; }
    public DateTime? DueAtUtc { get; set; }

    [MaxLength(1000)]
    public string? Instructions { get; set; }

    public bool CountsTowardProgression { get; set; } = true;
}

public sealed class CompleteAssignmentRequest
{
    [MaxLength(1000)]
    public string? AthleteNotes { get; set; }

    [Range(1, 5)]
    public int? Rating { get; set; }
}
