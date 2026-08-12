using System.ComponentModel.DataAnnotations;

namespace SkillBuilderPro.API.Contracts.Schedules;

public sealed class TrainingScheduleRequest
{
    [Range(1, int.MaxValue)]
    public int DrillId { get; init; }

    [Required]
    public string Title { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    [Required]
    public string Status { get; init; } = string.Empty;
}
