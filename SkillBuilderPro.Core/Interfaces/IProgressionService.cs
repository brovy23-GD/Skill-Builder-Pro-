namespace SkillBuilderPro.Core.Interfaces;

public interface IProgressionService
{
    Task RecalculateAthleteAsync(int athleteUserId, CancellationToken cancellationToken = default);
    Task<AthleteProgressionView> GetAthleteProgressionAsync(int athleteUserId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<AthleteSkillProgressView>> GetAthleteSkillsAsync(int athleteUserId, CancellationToken cancellationToken = default);
}

public sealed record AthleteProgressionView(int AthleteUserId, int OverallRankNumber, string OverallRank, int ProgressionScore, int ProgressToNextRank, string? NextRank, int? NextRankThreshold, int? PointsToNextRank, int TotalQualifyingCompletions, int ActiveSkillCount, int CurrentStreak, int LongestStreak, DateTime? LastCompletedAtUtc, DateTime? UpdatedAtUtc);
public sealed record AthleteSkillProgressView(string Sport, string Category, string SubCategory, int CurrentLevel, string LevelName, int QualifyingCompletions, double? AverageRating, int CurrentStreak, int LongestStreak, DateTime? LastCompletedAtUtc, int ProgressToNextLevel, string? NextLevelName, int CompletionsNeededForNextLevel, DateTime UpdatedAtUtc);
