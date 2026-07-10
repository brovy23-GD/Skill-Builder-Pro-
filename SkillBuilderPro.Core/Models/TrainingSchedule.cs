using System.ComponentModel.DataAnnotations;

namespace SkillBuilderPro.Core.Models;

public class TrainingSchedule
{
    public int Id { get; set; }

    [Required]
    public int DrillId { get; set; }
    public Drill? Drill { get; set; }

    [Required]
    public DateTime ScheduledDate { get; set; }

    [Range(5, 240)]
    public int DurationMinutes { get; set; } = 30;

    public bool IsCompleted { get; set; }

    [MaxLength(300)]
    public string Notes { get; set; } = string.Empty;
}
