using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SkillBuilderPro.Core.Identity;

namespace SkillBuilderPro.Core.Models;

public sealed class AthleteRankHistory
{
    public int Id { get; set; }
    public int AthleteUserId { get; set; }
    public int RankNumber { get; set; }
    public int ProgressionScoreAtEarned { get; set; }
    public int TotalQualifyingCompletionsAtEarned { get; set; }
    public int ActiveSkillCountAtEarned { get; set; }
    public int CurrentStreakAtEarned { get; set; }
    public DateTime EarnedAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    [JsonIgnore] public ApplicationUser AthleteUser { get; set; } = null!;
}

public sealed class AthleteSkillLevelHistory
{
    public int Id { get; set; }
    public int AthleteUserId { get; set; }
    [Required, MaxLength(100)] public string Sport { get; set; } = string.Empty;
    [Required, MaxLength(100)] public string Category { get; set; } = string.Empty;
    [Required, MaxLength(100)] public string SubCategory { get; set; } = string.Empty;
    public int Level { get; set; }
    public int QualifyingCompletionsAtEarned { get; set; }
    public double? AverageRatingAtEarned { get; set; }
    public DateTime EarnedAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    [JsonIgnore] public ApplicationUser AthleteUser { get; set; } = null!;
}

public static class AchievementCategories
{
    public const string Training = "Training";
    public const string Skill = "Skill";
    public const string Rank = "Rank";
    public const string Consistency = "Consistency";
}

public static class AchievementTiers
{
    public const string Bronze = "Bronze";
    public const string Silver = "Silver";
    public const string Gold = "Gold";
    public const string Platinum = "Platinum";
}

public static class AchievementCodes
{
    public const string FirstCompletion = "FIRST_COMPLETION";
    public const string FirstSkillDeveloping = "FIRST_SKILL_DEVELOPING";
    public const string RankRisingStar = "RANK_RISING_STAR";
    public const string TenQualifyingCompletions = "TEN_QUALIFYING_COMPLETIONS";
    public const string ThreeDayStreak = "THREE_DAY_STREAK";
}

public sealed class AchievementDefinition
{
    public int Id { get; set; }
    [Required, MaxLength(80)] public string Code { get; set; } = string.Empty;
    [Required, MaxLength(120)] public string Name { get; set; } = string.Empty;
    [Required, MaxLength(500)] public string Description { get; set; } = string.Empty;
    [Required, MaxLength(30)] public string Category { get; set; } = string.Empty;
    [Required, MaxLength(20)] public string Tier { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

public sealed class AthleteAchievement
{
    public int Id { get; set; }
    public int AthleteUserId { get; set; }
    public int AchievementDefinitionId { get; set; }
    public DateTime EarnedAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    [MaxLength(40)] public string? SourceType { get; set; }
    [MaxLength(160)] public string? SourceKey { get; set; }
    [JsonIgnore] public ApplicationUser AthleteUser { get; set; } = null!;
    [JsonIgnore] public AchievementDefinition AchievementDefinition { get; set; } = null!;
}
