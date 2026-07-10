using System.ComponentModel.DataAnnotations;

namespace SkillBuilderPro.Core.Models;

public class ProgressLog
{
    public int Id { get; set; }

    [Required]
    public int DrillId { get; set; }
    public Drill? Drill { get; set; }

    [Required]
    public DateTime LogDate { get; set; } = DateTime.UtcNow;

    [Range(1, 5)]
    public int Rating { get; set; }                          // 1 = Struggled, 5 = Mastered

    [MaxLength(300)]
    public string Notes { get; set; } = string.Empty;
}
