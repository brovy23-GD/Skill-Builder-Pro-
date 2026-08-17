using Microsoft.EntityFrameworkCore;
using SkillBuilderPro.API.Contracts.Goals;
using SkillBuilderPro.Core.Data;
using SkillBuilderPro.Core.Models;
using SkillBuilderPro.Core.Progression;

namespace SkillBuilderPro.API.Services;

public interface IGoalService
{
    Task<(GoalResponse? Value, string? Error)> CreateAsync(int athleteId, int creatorId, string role, CreateGoalRequest request, CancellationToken ct);
    Task<IReadOnlyCollection<GoalResponse>> ListAsync(int athleteId, CancellationToken ct);
    Task<GoalResponse?> GetAsync(int athleteId, int goalId, CancellationToken ct);
    Task<GoalResponse?> CancelAsync(int athleteId, int goalId, int creatorId, CancellationToken ct);
    Task SynchronizeAthleteGoalsAsync(int athleteId, CancellationToken ct);
}

public sealed class GoalService(AppDbContext db) : IGoalService
{
    public async Task<(GoalResponse? Value, string? Error)> CreateAsync(int athleteId, int creatorId, string role, CreateGoalRequest r, CancellationToken ct)
    {
        var type = GoalTypes.All.FirstOrDefault(x => x.Equals(r.GoalType.Trim(), StringComparison.OrdinalIgnoreCase));
        if (type is null || string.IsNullOrWhiteSpace(r.Title)) return (null, "Invalid goal type or title.");
        if (r.DueAtUtc is not null && r.DueAtUtc <= DateTime.UtcNow) return (null, "DueAtUtc must be in the future.");
        var sport = Normalize(r.Sport); var category = Normalize(r.Category); var sub = Normalize(r.SubCategory);
        if (type == GoalTypes.SkillLevel && (sport is null || category is null || sub is null || r.TargetValue is < 2 or > 5)) return (null, "SkillLevel requires known taxonomy and target 2-5.");
        if (type == GoalTypes.OverallRank && r.TargetValue is < 2 or > 8) return (null, "OverallRank target must be 2-8.");
        if (type == GoalTypes.TrainingStreak && r.TargetValue > 3650) return (null, "TrainingStreak target is too large.");
        if (type == GoalTypes.QualifyingCompletions && new[] { sport, category, sub }.Any(x => x is not null) && new[] { sport, category, sub }.Any(x => x is null)) return (null, "Provide complete taxonomy or none.");
        if (type is GoalTypes.SkillLevel or GoalTypes.QualifyingCompletions && sport is not null && !await db.Drills.AnyAsync(d => d.Sport.Trim().ToUpper() == sport && d.Category.Trim().ToUpper() == category && d.SubCategory != null && d.SubCategory.Trim().ToUpper() == sub, ct)) return (null, "Unknown skill taxonomy.");
        var now = DateTime.UtcNow;
        var goal = new AthleteGoal { AthleteUserId = athleteId, CreatedByUserId = creatorId, CreatedByRole = role, GoalType = type, Sport = sport, Category = category, SubCategory = sub, TargetValue = r.TargetValue, Title = r.Title.Trim(), Description = NormalizeText(r.Description), DueAtUtc = r.DueAtUtc, CreatedAtUtc = now, UpdatedAtUtc = now };
        db.AthleteGoals.Add(goal); await db.SaveChangesAsync(ct); await SynchronizeAthleteGoalsAsync(athleteId, ct);
        return (await GetAsync(athleteId, goal.Id, ct), null);
    }

    public async Task<IReadOnlyCollection<GoalResponse>> ListAsync(int athleteId, CancellationToken ct) { await SynchronizeAthleteGoalsAsync(athleteId, ct); var goals = await Query().Where(x => x.AthleteUserId == athleteId).OrderByDescending(x => x.CreatedAtUtc).ToListAsync(ct); return await MapAll(goals, ct); }
    public async Task<GoalResponse?> GetAsync(int athleteId, int goalId, CancellationToken ct) { await SynchronizeAthleteGoalsAsync(athleteId, ct); var goal = await Query().FirstOrDefaultAsync(x => x.AthleteUserId == athleteId && x.Id == goalId, ct); return goal is null ? null : await Map(goal, ct); }
    public async Task<GoalResponse?> CancelAsync(int athleteId, int goalId, int creatorId, CancellationToken ct) { var goal = await db.AthleteGoals.FirstOrDefaultAsync(x => x.Id == goalId && x.AthleteUserId == athleteId && x.CreatedByUserId == creatorId, ct); if (goal is null || goal.Status != GoalStatuses.Active) return null; goal.Status = GoalStatuses.Cancelled; goal.CancelledAtUtc = goal.UpdatedAtUtc = DateTime.UtcNow; await db.SaveChangesAsync(ct); return await GetAsync(athleteId, goalId, ct); }
    public async Task SynchronizeAthleteGoalsAsync(int athleteId, CancellationToken ct)
    {
        var goals = await db.AthleteGoals.Where(x => x.AthleteUserId == athleteId && x.Status == GoalStatuses.Active).ToListAsync(ct); if (goals.Count == 0) return;
        foreach (var g in goals) { var value = await Current(g, ct); if (value >= g.TargetValue) { g.Status = GoalStatuses.Completed; g.CompletedAtUtc = await CompletionTime(g, ct) ?? DateTime.UtcNow; g.UpdatedAtUtc = DateTime.UtcNow; db.NotificationEvents.Add(NotificationEventFactory.Create(NotificationTypes.GoalCompleted,$"Goal:{g.Id}:Completed:Athlete",g.AthleteUserId,g.CreatedByUserId,"Goal Completed",$"You completed your goal: {g.Title}.","AthleteGoal",g.Id,$"/goals/{g.Id}",g.CompletedAtUtc.Value)); if(g.CreatedByUserId!=g.AthleteUserId)db.NotificationEvents.Add(NotificationEventFactory.Create(NotificationTypes.GoalCompleted,$"Goal:{g.Id}:Completed:Creator",g.CreatedByUserId,g.AthleteUserId,"Goal Completed",$"An athlete completed the goal: {g.Title}.","AthleteGoal",g.Id,$"/goals/{g.Id}",g.CompletedAtUtc.Value)); } }
        if (db.ChangeTracker.HasChanges()) await db.SaveChangesAsync(ct);
    }
    private IQueryable<AthleteGoal> Query() => db.AthleteGoals.AsNoTracking().Include(x => x.CreatedByUser).ThenInclude(x => x.Profile);
    private async Task<int> Current(AthleteGoal g, CancellationToken ct) => g.GoalType switch { GoalTypes.SkillLevel => await db.AthleteSkillProgress.Where(x => x.AthleteUserId == g.AthleteUserId && x.Sport == g.Sport && x.Category == g.Category && x.SubCategory == g.SubCategory).Select(x => (int?)x.CurrentLevel).FirstOrDefaultAsync(ct) ?? 1, GoalTypes.OverallRank => await db.AthleteProgressions.Where(x => x.AthleteUserId == g.AthleteUserId).Select(x => (int?)x.OverallRank).FirstOrDefaultAsync(ct) ?? 1, GoalTypes.TrainingStreak => await db.AthleteProgressions.Where(x => x.AthleteUserId == g.AthleteUserId).Select(x => (int?)x.LongestOverallStreak).FirstOrDefaultAsync(ct) ?? 0, _ when g.Sport is not null => await db.AthleteSkillProgress.Where(x => x.AthleteUserId == g.AthleteUserId && x.Sport == g.Sport && x.Category == g.Category && x.SubCategory == g.SubCategory).Select(x => (int?)x.QualifyingCompletions).FirstOrDefaultAsync(ct) ?? 0, _ => await db.AthleteProgressions.Where(x => x.AthleteUserId == g.AthleteUserId).Select(x => (int?)x.TotalQualifyingCompletions).FirstOrDefaultAsync(ct) ?? 0 };
    private async Task<DateTime?> CompletionTime(AthleteGoal g, CancellationToken ct) => g.GoalType switch { GoalTypes.SkillLevel => await db.AthleteSkillLevelHistories.Where(x => x.AthleteUserId == g.AthleteUserId && x.Sport == g.Sport && x.Category == g.Category && x.SubCategory == g.SubCategory && x.Level >= g.TargetValue).OrderBy(x => x.EarnedAtUtc).Select(x => (DateTime?)x.EarnedAtUtc).FirstOrDefaultAsync(ct), GoalTypes.OverallRank => await db.AthleteRankHistories.Where(x => x.AthleteUserId == g.AthleteUserId && x.RankNumber >= g.TargetValue).OrderBy(x => x.EarnedAtUtc).Select(x => (DateTime?)x.EarnedAtUtc).FirstOrDefaultAsync(ct), _ => null };
    private async Task<GoalResponse> Map(AthleteGoal g, CancellationToken ct) { var current = await Current(g, ct); var percent = Math.Clamp((int)Math.Floor(current * 100d / g.TargetValue), 0, 100); var targetName = g.GoalType == GoalTypes.SkillLevel ? ProgressionRules.SkillLevelNames[g.TargetValue - 1] : g.GoalType == GoalTypes.OverallRank ? ProgressionRules.RankNames[g.TargetValue - 1] : null; return new(g.Id, g.AthleteUserId, new(g.CreatedByUserId, g.CreatedByUser.Profile?.FullName ?? "User", g.CreatedByRole), g.GoalType, g.Sport, g.Category, g.SubCategory, g.Title, g.Description, g.TargetValue, targetName, current, percent, g.Status, g.Status == GoalStatuses.Completed, g.Status == GoalStatuses.Active && g.DueAtUtc < DateTime.UtcNow, g.DueAtUtc, g.CreatedAtUtc, g.UpdatedAtUtc, g.CompletedAtUtc, g.CancelledAtUtc); }
    private async Task<IReadOnlyCollection<GoalResponse>> MapAll(List<AthleteGoal> goals, CancellationToken ct) { var list = new List<GoalResponse>(); foreach (var g in goals) list.Add(await Map(g, ct)); return list; }
    private static string? Normalize(string? x) => string.IsNullOrWhiteSpace(x) ? null : x.Trim().ToUpperInvariant();
    private static string? NormalizeText(string? x) => string.IsNullOrWhiteSpace(x) ? null : x.Trim();
}
