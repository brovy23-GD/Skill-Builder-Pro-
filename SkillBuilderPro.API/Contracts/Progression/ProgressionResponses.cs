using SkillBuilderPro.Core.Interfaces;

namespace SkillBuilderPro.API.Contracts.Progression;

public sealed record AthleteProgressionResponse(int AthleteUserId, string OverallRank, int OverallRankNumber, int ProgressionScore, int ProgressToNextRank, string? NextRank, int? NextRankThreshold, int? PointsToNextRank, int TotalQualifyingCompletions, int ActiveSkillCount, int CurrentStreak, int LongestStreak, DateTime? LastCompletedAtUtc, DateTime? UpdatedAtUtc);
public sealed record AthleteSkillProgressResponse(string Sport, string Category, string SubCategory, int CurrentLevel, string LevelName, int QualifyingCompletions, double? AverageRating, int CurrentStreak, int LongestStreak, DateTime? LastCompletedAtUtc, int ProgressToNextLevel, string? NextLevelName, int CompletionsNeededForNextLevel, DateTime UpdatedAtUtc);

public static class ProgressionResponseMapper
{
    public static AthleteProgressionResponse ToResponse(this AthleteProgressionView view) => new(view.AthleteUserId, view.OverallRank, view.OverallRankNumber, view.ProgressionScore, view.ProgressToNextRank, view.NextRank, view.NextRankThreshold, view.PointsToNextRank, view.TotalQualifyingCompletions, view.ActiveSkillCount, view.CurrentStreak, view.LongestStreak, view.LastCompletedAtUtc, view.UpdatedAtUtc);
    public static AthleteSkillProgressResponse ToResponse(this AthleteSkillProgressView view) => new(view.Sport, view.Category, view.SubCategory, view.CurrentLevel, view.LevelName, view.QualifyingCompletions, view.AverageRating, view.CurrentStreak, view.LongestStreak, view.LastCompletedAtUtc, view.ProgressToNextLevel, view.NextLevelName, view.CompletionsNeededForNextLevel, view.UpdatedAtUtc);
}
