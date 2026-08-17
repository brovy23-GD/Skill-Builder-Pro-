using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SkillBuilderPro.Core.Identity;

namespace SkillBuilderPro.Core.Models;

public sealed class AthleteSkillProgress
{
    public int Id { get; set; }
    public int AthleteUserId { get; set; }

    [Required, MaxLength(100)]
    public string Sport { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string SubCategory { get; set; } = string.Empty;

    public int CurrentLevel { get; set; } = 1;
    public int QualifyingCompletions { get; set; }
    public double? AverageRating { get; set; }
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public DateTime? LastCompletedAtUtc { get; set; }
    public int ProgressToNextLevel { get; set; }
    public DateTime UpdatedAtUtc { get; set; }

    [JsonIgnore]
    public ApplicationUser AthleteUser { get; set; } = null!;
}
