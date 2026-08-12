using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SkillBuilderPro.Core.Identity;

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

    public int? OwnerUserId { get; set; }

    [JsonIgnore]
    public ApplicationUser? Owner { get; set; }
}
