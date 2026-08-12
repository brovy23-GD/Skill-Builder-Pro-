using System.ComponentModel.DataAnnotations;

namespace SkillBuilderPro.API.Contracts.Progress;

public sealed class ProgressLogRequest
{
    [Range(1, int.MaxValue)]
    public int DrillId { get; init; }

    public DateTime? LogDate { get; init; }

    [Range(1, 5)]
    public int Rating { get; init; }

    [MaxLength(300)]
    public string Notes { get; init; } = string.Empty;
}
