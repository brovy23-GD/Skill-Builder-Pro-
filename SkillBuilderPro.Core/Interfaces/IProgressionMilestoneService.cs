namespace SkillBuilderPro.Core.Interfaces;

public interface IProgressionMilestoneService
{
    Task SyncMilestonesAsync(int athleteUserId, CancellationToken cancellationToken = default);
    Task<TrophyRoomView> GetTrophyRoomAsync(int athleteUserId, CancellationToken cancellationToken = default);
}

public sealed record RankHistoryView(int RankNumber, string RankName, DateTime EarnedAtUtc, int ProgressionScoreAtEarned, int TotalQualifyingCompletionsAtEarned, int ActiveSkillCountAtEarned);
public sealed record SkillLevelHistoryView(string Sport, string Category, string SubCategory, int Level, string LevelName, DateTime EarnedAtUtc, int QualifyingCompletionsAtEarned, double? AverageRatingAtEarned);
public sealed record AchievementView(string Code, string Name, string Description, string Category, string Tier, bool IsEarned, DateTime? EarnedAtUtc, int SortOrder);
public sealed record TrophyRoomView(AthleteProgressionView CurrentProgression, IReadOnlyCollection<RankHistoryView> RankHistory, IReadOnlyCollection<SkillLevelHistoryView> SkillMilestones, IReadOnlyCollection<AchievementView> Achievements);
