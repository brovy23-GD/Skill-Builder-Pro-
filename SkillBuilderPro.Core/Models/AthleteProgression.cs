using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SkillBuilderPro.Core.Identity;

namespace SkillBuilderPro.Core.Models;

public sealed class AthleteProgression
{
    [Key]
    public int AthleteUserId { get; set; }
    public int OverallRank { get; set; } = 1;
    public int ProgressionScore { get; set; }
    public int TotalQualifyingCompletions { get; set; }
    public int ActiveSkillCount { get; set; }
    public int CurrentOverallStreak { get; set; }
    public int LongestOverallStreak { get; set; }
    public DateTime? LastCompletedAtUtc { get; set; }
    public int ProgressToNextRank { get; set; }
    public DateTime UpdatedAtUtc { get; set; }

    [JsonIgnore]
    public ApplicationUser AthleteUser { get; set; } = null!;
}
