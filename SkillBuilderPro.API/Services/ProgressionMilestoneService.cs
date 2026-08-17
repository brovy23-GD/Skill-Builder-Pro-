using Microsoft.EntityFrameworkCore;
using SkillBuilderPro.Core.Data;
using SkillBuilderPro.Core.Interfaces;
using SkillBuilderPro.Core.Models;
using SkillBuilderPro.Core.Progression;

namespace SkillBuilderPro.API.Services;

public sealed class ProgressionMilestoneService : IProgressionMilestoneService
{
    private readonly AppDbContext _db;
    private readonly IProgressionService _progression;
    public ProgressionMilestoneService(AppDbContext db, IProgressionService progression) => (_db, _progression) = (db, progression);

    public async Task SyncMilestonesAsync(int athleteUserId, CancellationToken cancellationToken = default)
    {
        try { await SyncCoreAsync(athleteUserId, cancellationToken); }
        catch (DbUpdateException) { _db.ChangeTracker.Clear(); await SyncCoreAsync(athleteUserId, cancellationToken); }
    }

    private async Task SyncCoreAsync(int athleteUserId, CancellationToken cancellationToken)
    {
        var evidence = await _db.ProgressLogs.AsNoTracking()
            .Where(x => x.OwnerUserId == athleteUserId && x.AssignmentCompletionEventId != null)
            .OrderBy(x => x.LogDate).ThenBy(x => x.Id)
            .Select(x => new Evidence(x.Id, x.LogDate, x.Rating, x.Drill!.Sport, x.Drill.Category, x.Drill.SubCategory))
            .ToListAsync(cancellationToken);
        if (evidence.Count == 0) return;

        var existingRanks = (await _db.AthleteRankHistories.Where(x => x.AthleteUserId == athleteUserId).Select(x => x.RankNumber).ToListAsync(cancellationToken)).ToHashSet();
        var existingSkills = (await _db.AthleteSkillLevelHistories.Where(x => x.AthleteUserId == athleteUserId)
            .Select(x => new { x.Sport, x.Category, x.SubCategory, x.Level }).ToListAsync(cancellationToken))
            .Select(x => Key(x.Sport, x.Category, x.SubCategory, x.Level)).ToHashSet();
        var now = DateTime.UtcNow;

        for (var index = 0; index < evidence.Count; index++)
        {
            var prefix = evidence.Take(index + 1).ToList();
            var groups = prefix.GroupBy(x => SkillKey(x.Sport, x.Category, x.SubCategory)).ToList();
            var levels = groups.Select(g => ProgressionRules.GetSkillLevel(g.Count())).ToList();
            var streak = ProgressionRules.CalculateStreaks(prefix.Select(x => x.LogDate), evidence[index].LogDate);
            var score = ProgressionRules.GetProgressionScore(prefix.Count, levels, groups.Count, streak.Longest);
            var rank = ProgressionRules.GetRank(score, groups.Count);
            for (var earnedRank = 2; earnedRank <= rank; earnedRank++)
                if (existingRanks.Add(earnedRank)) { _db.AthleteRankHistories.Add(new AthleteRankHistory
                {
                    AthleteUserId = athleteUserId, RankNumber = earnedRank, ProgressionScoreAtEarned = score,
                    TotalQualifyingCompletionsAtEarned = prefix.Count, ActiveSkillCountAtEarned = groups.Count,
                    CurrentStreakAtEarned = streak.Current, EarnedAtUtc = evidence[index].LogDate, CreatedAtUtc = now
                }); _db.NotificationEvents.Add(NotificationEventFactory.Create(NotificationTypes.RankEarned,$"Athlete:{athleteUserId}:Rank:{earnedRank}",athleteUserId,null,"Rank Up!",$"You reached {ProgressionRules.RankNames[earnedRank-1]}.","TrophyRoom",athleteUserId,"/trophy-room",evidence[index].LogDate)); }

            var current = evidence[index];
            var skillEvidence = prefix.Where(x => SkillKey(x.Sport, x.Category, x.SubCategory) == SkillKey(current.Sport, current.Category, current.SubCategory)).ToList();
            var level = ProgressionRules.GetSkillLevel(skillEvidence.Count);
            for (var earnedLevel = 2; earnedLevel <= level; earnedLevel++)
            {
                var key = Key(current.Sport, current.Category, current.SubCategory, earnedLevel);
                if (!existingSkills.Add(key)) continue;
                var ratings = skillEvidence.Where(x => x.Rating.HasValue).Select(x => x.Rating!.Value).ToList();
                _db.AthleteSkillLevelHistories.Add(new AthleteSkillLevelHistory
                {
                    AthleteUserId = athleteUserId, Sport = current.Sport.Trim(), Category = current.Category.Trim(),
                    SubCategory = (current.SubCategory ?? string.Empty).Trim(), Level = earnedLevel,
                    QualifyingCompletionsAtEarned = skillEvidence.Count, AverageRatingAtEarned = ratings.Count == 0 ? null : ratings.Average(),
                    EarnedAtUtc = current.LogDate, CreatedAtUtc = now
                });
                _db.NotificationEvents.Add(NotificationEventFactory.Create(NotificationTypes.SkillLevelEarned,$"Athlete:{athleteUserId}:{SkillKey(current.Sport,current.Category,current.SubCategory)}:Level:{earnedLevel}",athleteUserId,null,"Skill Level Up",$"You reached {ProgressionRules.SkillLevelNames[earnedLevel-1]} in {current.Sport} {(current.SubCategory??string.Empty).Trim()}.".Trim(),"TrophyRoom",athleteUserId,"/trophy-room",current.LogDate));
            }
        }
        if (_db.ChangeTracker.HasChanges()) await _db.SaveChangesAsync(cancellationToken);
        await EvaluateAchievementsAsync(athleteUserId, evidence, cancellationToken);
    }

    private async Task EvaluateAchievementsAsync(int athleteUserId, IReadOnlyList<Evidence> evidence, CancellationToken cancellationToken)
    {
        var definitions = await _db.AchievementDefinitions.Where(x => x.IsActive).ToDictionaryAsync(x => x.Code, cancellationToken);
        var earnedIds = (await _db.AthleteAchievements.Where(x => x.AthleteUserId == athleteUserId).Select(x => x.AchievementDefinitionId).ToListAsync(cancellationToken)).ToHashSet();
        var rank = await _db.AthleteRankHistories.Where(x => x.AthleteUserId == athleteUserId && x.RankNumber == 2).Select(x => (DateTime?)x.EarnedAtUtc).FirstOrDefaultAsync(cancellationToken);
        var developing = await _db.AthleteSkillLevelHistories.Where(x => x.AthleteUserId == athleteUserId && x.Level == 2).OrderBy(x => x.EarnedAtUtc).Select(x => (DateTime?)x.EarnedAtUtc).FirstOrDefaultAsync(cancellationToken);
        var threeDay = FirstStreakDate(evidence.Select(x => x.LogDate));
        var candidates = new[]
        {
            (AchievementCodes.FirstCompletion, (DateTime?)evidence[0].LogDate, "ProgressEvidence"),
            (AchievementCodes.FirstSkillDeveloping, developing, "SkillLevelHistory"),
            (AchievementCodes.RankRisingStar, rank, "RankHistory"),
            (AchievementCodes.TenQualifyingCompletions, evidence.Count >= 10 ? evidence[9].LogDate : (DateTime?)null, "ProgressEvidence"),
            (AchievementCodes.ThreeDayStreak, threeDay, "StreakEvidence")
        };
        foreach (var candidate in candidates)
            if (candidate.Item2 is DateTime earned && definitions.TryGetValue(candidate.Item1, out var definition) && earnedIds.Add(definition.Id))
                { _db.AthleteAchievements.Add(new AthleteAchievement { AthleteUserId = athleteUserId, AchievementDefinitionId = definition.Id, EarnedAtUtc = earned, CreatedAtUtc = DateTime.UtcNow, SourceType = candidate.Item3, SourceKey = candidate.Item1 }); _db.NotificationEvents.Add(NotificationEventFactory.Create(NotificationTypes.AchievementEarned,$"Athlete:{athleteUserId}:Achievement:{definition.Id}",athleteUserId,null,"Achievement Unlocked",$"You earned {definition.Name}.","TrophyRoom",athleteUserId,"/trophy-room",earned)); }
        if (_db.ChangeTracker.HasChanges()) await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<TrophyRoomView> GetTrophyRoomAsync(int athleteUserId, CancellationToken cancellationToken = default)
    {
        await _progression.RecalculateAthleteAsync(athleteUserId, cancellationToken);
        await SyncMilestonesAsync(athleteUserId, cancellationToken);
        var current = await _progression.GetAthleteProgressionAsync(athleteUserId, cancellationToken);
        var rankEntities = await _db.AthleteRankHistories.AsNoTracking().Where(x => x.AthleteUserId == athleteUserId).OrderBy(x => x.RankNumber).ToListAsync(cancellationToken);
        var ranks = rankEntities.Select(x => new RankHistoryView(x.RankNumber, ProgressionRules.RankNames[x.RankNumber - 1], x.EarnedAtUtc, x.ProgressionScoreAtEarned, x.TotalQualifyingCompletionsAtEarned, x.ActiveSkillCountAtEarned)).ToList();
        var skillEntities = await _db.AthleteSkillLevelHistories.AsNoTracking().Where(x => x.AthleteUserId == athleteUserId).OrderByDescending(x => x.EarnedAtUtc).ThenBy(x => x.Id).ToListAsync(cancellationToken);
        var skills = skillEntities.Select(x => new SkillLevelHistoryView(x.Sport, x.Category, x.SubCategory, x.Level, ProgressionRules.SkillLevelNames[x.Level - 1], x.EarnedAtUtc, x.QualifyingCompletionsAtEarned, x.AverageRatingAtEarned)).ToList();
        var earned = await _db.AthleteAchievements.AsNoTracking().Where(x => x.AthleteUserId == athleteUserId).ToDictionaryAsync(x => x.AchievementDefinitionId, x => x.EarnedAtUtc, cancellationToken);
        var definitions = await _db.AchievementDefinitions.AsNoTracking().Where(x => x.IsActive || earned.Keys.Contains(x.Id)).OrderBy(x => x.SortOrder).ThenBy(x => x.Name).ToListAsync(cancellationToken);
        var achievements = definitions.Select(x =>
        {
            var isEarned = earned.TryGetValue(x.Id, out var earnedAt);
            return new AchievementView(x.Code, x.Name, x.Description, x.Category, x.Tier, isEarned, isEarned ? earnedAt : null, x.SortOrder);
        }).ToList();
        return new TrophyRoomView(current, ranks, skills, achievements);
    }

    private static DateTime? FirstStreakDate(IEnumerable<DateTime> timestamps)
    {
        var days = timestamps.Select(x => x.Date).Distinct().OrderBy(x => x).ToList(); var run = 1;
        for (var i = 1; i < days.Count; i++) { run = days[i] == days[i - 1].AddDays(1) ? run + 1 : 1; if (run >= 3) return days[i]; }
        return null;
    }
    private static string SkillKey(string s, string c, string? sub) => $"{s.Trim().ToUpperInvariant()}|{c.Trim().ToUpperInvariant()}|{(sub ?? string.Empty).Trim().ToUpperInvariant()}";
    private static string Key(string s, string c, string? sub, int level) => $"{SkillKey(s, c, sub)}|{level}";
    private sealed record Evidence(int Id, DateTime LogDate, int? Rating, string Sport, string Category, string? SubCategory);
}
