using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using SkillBuilderPro.Core.Data;
using SkillBuilderPro.Core.Interfaces;
using SkillBuilderPro.Core.Models;
using SkillBuilderPro.Core.Progression;

namespace SkillBuilderPro.API.Services;

public sealed class ProgressionService : IProgressionService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<ProgressionService> _logger;

    public ProgressionService(AppDbContext dbContext, ILogger<ProgressionService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task RecalculateAthleteAsync(int athleteUserId, CancellationToken cancellationToken = default)
    {
        try
        {
            await RecalculateCoreAsync(athleteUserId, cancellationToken);
        }
        catch (DbUpdateException)
        {
            _dbContext.ChangeTracker.Clear();
            await RecalculateCoreAsync(athleteUserId, cancellationToken);
        }
    }

    private async Task RecalculateCoreAsync(int athleteUserId, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var evidence = await LoadEvidenceAsync(athleteUserId, cancellationToken);
        var existingSkills = await _dbContext.AthleteSkillProgress
            .Where(progress => progress.AthleteUserId == athleteUserId)
            .ToListAsync(cancellationToken);
        var overall = await _dbContext.AthleteProgressions
            .FirstOrDefaultAsync(progress => progress.AthleteUserId == athleteUserId, cancellationToken);

        if (evidence.Count == 0)
        {
            _dbContext.AthleteSkillProgress.RemoveRange(existingSkills);
            if (overall is not null) _dbContext.AthleteProgressions.Remove(overall);
            if (existingSkills.Count > 0 || overall is not null) await _dbContext.SaveChangesAsync(cancellationToken);
            return;
        }

        var now = DateTime.UtcNow;
        var calculatedSkills = evidence
            .GroupBy(item => SkillKey.Create(item.Sport, item.Category, item.SubCategory))
            .Select(group => CalculateSkill(group.Key, group.ToList(), now))
            .OrderBy(skill => skill.Sport).ThenBy(skill => skill.Category).ThenBy(skill => skill.SubCategory)
            .ToList();
        var calculatedKeys = calculatedSkills.Select(skill => skill.Key).ToHashSet();

        foreach (var calculated in calculatedSkills)
        {
            var entity = existingSkills.FirstOrDefault(skill => SkillKey.Create(skill.Sport, skill.Category, skill.SubCategory) == calculated.Key);
            if (entity is null)
            {
                entity = new AthleteSkillProgress { AthleteUserId = athleteUserId };
                _dbContext.AthleteSkillProgress.Add(entity);
            }
            Apply(entity, calculated, now);
        }
        _dbContext.AthleteSkillProgress.RemoveRange(existingSkills.Where(skill => !calculatedKeys.Contains(SkillKey.Create(skill.Sport, skill.Category, skill.SubCategory))));

        var overallStreak = ProgressionRules.CalculateStreaks(evidence.Select(item => item.LogDate), now);
        var score = ProgressionRules.GetProgressionScore(evidence.Count, calculatedSkills.Select(skill => skill.Level), calculatedSkills.Count, overallStreak.Longest);
        var rank = ProgressionRules.GetRank(score, calculatedSkills.Count);
        overall ??= new AthleteProgression { AthleteUserId = athleteUserId };
        if (_dbContext.Entry(overall).State == EntityState.Detached) _dbContext.AthleteProgressions.Add(overall);
        overall.OverallRank = rank;
        overall.ProgressionScore = score;
        overall.TotalQualifyingCompletions = evidence.Count;
        overall.ActiveSkillCount = calculatedSkills.Count;
        overall.CurrentOverallStreak = overallStreak.Current;
        overall.LongestOverallStreak = overallStreak.Longest;
        overall.LastCompletedAtUtc = evidence.Max(item => item.LogDate);
        overall.ProgressToNextRank = ProgressionRules.GetRankProgressPercent(score, rank);
        overall.UpdatedAtUtc = now;

        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation(
            "Recalculated progression for Athlete {AthleteUserId} from {EvidenceCount} qualifying logs across {SkillCount} skills to rank {RankNumber} in {ElapsedMilliseconds} ms.",
            athleteUserId, evidence.Count, calculatedSkills.Count, rank, stopwatch.ElapsedMilliseconds);
    }

    public async Task<AthleteProgressionView> GetAthleteProgressionAsync(int athleteUserId, CancellationToken cancellationToken = default)
    {
        await EnsureCurrentAsync(athleteUserId, cancellationToken);
        var progress = await _dbContext.AthleteProgressions.AsNoTracking()
            .FirstOrDefaultAsync(item => item.AthleteUserId == athleteUserId, cancellationToken);
        if (progress is null) return DefaultOverall(athleteUserId);
        var nextRank = progress.OverallRank < ProgressionRules.RankNames.Length ? ProgressionRules.RankNames[progress.OverallRank] : null;
        int? nextThreshold = progress.OverallRank < ProgressionRules.RankThresholds.Length ? ProgressionRules.RankThresholds[progress.OverallRank] : null;
        return new AthleteProgressionView(
            athleteUserId, progress.OverallRank, ProgressionRules.RankNames[progress.OverallRank - 1], progress.ProgressionScore,
            progress.ProgressToNextRank, nextRank, nextThreshold, nextThreshold is int threshold ? Math.Max(0, threshold - progress.ProgressionScore) : null,
            progress.TotalQualifyingCompletions, progress.ActiveSkillCount, progress.CurrentOverallStreak,
            progress.LongestOverallStreak, progress.LastCompletedAtUtc, progress.UpdatedAtUtc);
    }

    public async Task<IReadOnlyCollection<AthleteSkillProgressView>> GetAthleteSkillsAsync(int athleteUserId, CancellationToken cancellationToken = default)
    {
        await EnsureCurrentAsync(athleteUserId, cancellationToken);
        var skills = await _dbContext.AthleteSkillProgress.AsNoTracking()
            .Where(progress => progress.AthleteUserId == athleteUserId)
            .OrderBy(progress => progress.Sport).ThenBy(progress => progress.Category).ThenBy(progress => progress.SubCategory)
            .ToListAsync(cancellationToken);
        return skills.Select(ToView).ToList();
    }

    private async Task EnsureCurrentAsync(int athleteUserId, CancellationToken cancellationToken)
    {
        var hasEvidence = await _dbContext.ProgressLogs.AsNoTracking()
            .AnyAsync(log => log.OwnerUserId == athleteUserId && log.AssignmentCompletionEventId != null, cancellationToken);
        var hasMaterializedState = await _dbContext.AthleteProgressions.AsNoTracking()
            .AnyAsync(progress => progress.AthleteUserId == athleteUserId, cancellationToken);
        if (!hasEvidence && !hasMaterializedState) return;
        await RecalculateAthleteAsync(athleteUserId, cancellationToken);
    }

    private Task<List<EvidenceRow>> LoadEvidenceAsync(int athleteUserId, CancellationToken cancellationToken) =>
        _dbContext.ProgressLogs.AsNoTracking()
            .Where(log => log.OwnerUserId == athleteUserId && log.AssignmentCompletionEventId != null)
            .Select(log => new EvidenceRow(log.LogDate, log.Rating, log.Drill!.Sport, log.Drill.Category, log.Drill.SubCategory))
            .ToListAsync(cancellationToken);

    private static CalculatedSkill CalculateSkill(SkillKey key, IReadOnlyCollection<EvidenceRow> evidence, DateTime now)
    {
        var display = evidence.First();
        var level = ProgressionRules.GetSkillLevel(evidence.Count);
        var streak = ProgressionRules.CalculateStreaks(evidence.Select(item => item.LogDate), now);
        var ratings = evidence.Where(item => item.Rating.HasValue).Select(item => item.Rating!.Value).ToList();
        return new CalculatedSkill(key, display.Sport.Trim(), display.Category.Trim(), (display.SubCategory ?? string.Empty).Trim(), level, evidence.Count,
            ratings.Count == 0 ? null : ratings.Average(), streak.Current, streak.Longest, evidence.Max(item => item.LogDate),
            ProgressionRules.GetSkillProgressPercent(evidence.Count, level));
    }

    private static void Apply(AthleteSkillProgress entity, CalculatedSkill value, DateTime now)
    {
        entity.Sport = value.Sport; entity.Category = value.Category; entity.SubCategory = value.SubCategory;
        entity.CurrentLevel = value.Level; entity.QualifyingCompletions = value.Completions; entity.AverageRating = value.AverageRating;
        entity.CurrentStreak = value.CurrentStreak; entity.LongestStreak = value.LongestStreak;
        entity.LastCompletedAtUtc = value.LastCompletedAtUtc; entity.ProgressToNextLevel = value.ProgressPercent; entity.UpdatedAtUtc = now;
    }

    private static AthleteSkillProgressView ToView(AthleteSkillProgress skill)
    {
        var nextLevel = skill.CurrentLevel < ProgressionRules.SkillLevelNames.Length ? ProgressionRules.SkillLevelNames[skill.CurrentLevel] : null;
        var needed = skill.CurrentLevel < ProgressionRules.SkillThresholds.Length
            ? Math.Max(0, ProgressionRules.SkillThresholds[skill.CurrentLevel] - skill.QualifyingCompletions) : 0;
        return new AthleteSkillProgressView(skill.Sport, skill.Category, skill.SubCategory, skill.CurrentLevel,
            ProgressionRules.SkillLevelNames[skill.CurrentLevel - 1], skill.QualifyingCompletions, skill.AverageRating,
            skill.CurrentStreak, skill.LongestStreak, skill.LastCompletedAtUtc, skill.ProgressToNextLevel, nextLevel, needed, skill.UpdatedAtUtc);
    }

    private static AthleteProgressionView DefaultOverall(int athleteUserId) => new(
        athleteUserId, 1, ProgressionRules.RankNames[0], 0, 0, ProgressionRules.RankNames[1], ProgressionRules.RankThresholds[1], ProgressionRules.RankThresholds[1], 0, 0, 0, 0, null, null);

    private sealed record EvidenceRow(DateTime LogDate, int? Rating, string Sport, string Category, string? SubCategory);
    private sealed record CalculatedSkill(SkillKey Key, string Sport, string Category, string SubCategory, int Level, int Completions, double? AverageRating, int CurrentStreak, int LongestStreak, DateTime LastCompletedAtUtc, int ProgressPercent);
    private sealed record SkillKey(string Sport, string Category, string SubCategory)
    {
        public static SkillKey Create(string? sport, string? category, string? subCategory) => new(Normalize(sport), Normalize(category), Normalize(subCategory));
        private static string Normalize(string? value) => (value ?? string.Empty).Trim().ToUpperInvariant();
    }
}
