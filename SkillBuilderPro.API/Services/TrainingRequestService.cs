using Microsoft.EntityFrameworkCore;
using System.Data;
using SkillBuilderPro.API.Contracts.Goals;
using SkillBuilderPro.API.Contracts.TrainingRequests;
using SkillBuilderPro.Core.Data;
using SkillBuilderPro.Core.Identity;
using SkillBuilderPro.Core.Interfaces;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.API.Services;

public interface ITrainingRequestService
{
    Task<(TrainingRequestResponse? Value, string? Error)> CreateAsync(int athleteId, CreateTrainingRequest request, CancellationToken ct);
    Task<IReadOnlyCollection<TrainingRequestResponse>> ListForAthleteAsync(int athleteId, CancellationToken ct);
    Task<TrainingRequestResponse?> GetForAthleteAsync(int athleteId, int id, CancellationToken ct);
    Task<TrainingRequestResponse?> CancelAsync(int athleteId, int id, CancellationToken ct);
    Task<IReadOnlyCollection<TrainingRequestResponse>> InboxAsync(int recipientId, string role, CancellationToken ct);
    Task<TrainingRequestResponse?> InboxItemAsync(int recipientId, string role, int id, CancellationToken ct);
    Task<(TrainingRequestResponse? Value, string? Error)> ApproveAsync(int recipientId, string role, int id, ApproveTrainingRequest request, CancellationToken ct);
    Task<TrainingRequestResponse?> DeclineAsync(int recipientId, string role, int id, CancellationToken ct);
}

public sealed class TrainingRequestService(AppDbContext db, IRelationshipAccessService access, IAssignmentService assignments) : ITrainingRequestService
{
    public async Task<(TrainingRequestResponse? Value, string? Error)> CreateAsync(int athleteId, CreateTrainingRequest r, CancellationToken ct)
    {
        var recipientRole = await access.IsUserInRoleAsync(r.RequestedRecipientUserId, ApplicationRoles.Parent, ct) ? ApplicationRoles.Parent : await access.IsUserInRoleAsync(r.RequestedRecipientUserId, ApplicationRoles.Coach, ct) ? ApplicationRoles.Coach : null;
        if (recipientRole is null) return (null, "Recipient is not an authorized Parent or Coach.");
        if (recipientRole == ApplicationRoles.Parent && !await access.CanParentAccessAthleteAsync(r.RequestedRecipientUserId, athleteId, ct)) return (null, "Recipient relationship was not found.");
        if (recipientRole == ApplicationRoles.Coach && (r.TeamId is null || !await access.CanCoachManageTeamAsync(r.RequestedRecipientUserId, r.TeamId.Value, ct) || !(await access.GetCoachTeamAthleteIdsAsync(r.RequestedRecipientUserId, r.TeamId.Value, ct)).Contains(athleteId))) return (null, "Recipient relationship was not found.");
        if (recipientRole == ApplicationRoles.Parent && r.TeamId is not null) return (null, "TeamId is only valid for Coach requests.");
        var drill = r.RequestedDrillId is int drillId ? await db.Drills.AsNoTracking().FirstOrDefaultAsync(x => x.Id == drillId, ct) : null;
        if (r.RequestedDrillId is not null && drill is null) return (null, "Requested Drill was not found.");
        var sport = N(r.Sport); var category = N(r.Category); var sub = N(r.SubCategory); var message = T(r.Message);
        if (drill is null && sport is null && category is null && sub is null && message is null) return (null, "Provide a Drill, taxonomy, or message.");
        if (new[] { sport, category, sub }.Any(x => x is not null) && new[] { sport, category, sub }.Any(x => x is null)) return (null, "Provide complete taxonomy or none.");
        if (sport is not null && !await db.Drills.AnyAsync(d => d.Sport.Trim().ToUpper() == sport && d.Category.Trim().ToUpper() == category && d.SubCategory != null && d.SubCategory.Trim().ToUpper() == sub, ct)) return (null, "Unknown taxonomy.");
        var entity = new TrainingRequest { AthleteUserId = athleteId, RequestedRecipientUserId = r.RequestedRecipientUserId, RequestedRecipientRole = recipientRole, TeamId = r.TeamId, RequestedDrillId = r.RequestedDrillId, Sport = sport, Category = category, SubCategory = sub, Message = message, CreatedAtUtc = DateTime.UtcNow };
        db.TrainingRequests.Add(entity); await db.SaveChangesAsync(ct);
        db.NotificationEvents.Add(NotificationEventFactory.Create(NotificationTypes.TrainingRequestReceived,$"TrainingRequest:{entity.Id}:Received",entity.RequestedRecipientUserId,athleteId,"New Training Request","An athlete requested more training.","TrainingRequest",entity.Id,$"/training-requests/{entity.Id}",entity.CreatedAtUtc));
        await db.SaveChangesAsync(ct); return (await GetForAthleteAsync(athleteId, entity.Id, ct), null);
    }
    public async Task<IReadOnlyCollection<TrainingRequestResponse>> ListForAthleteAsync(int athleteId, CancellationToken ct) => (await Q().Where(x => x.AthleteUserId == athleteId).OrderByDescending(x => x.CreatedAtUtc).ToListAsync(ct)).Select(Map).ToList();
    public async Task<TrainingRequestResponse?> GetForAthleteAsync(int athleteId, int id, CancellationToken ct) { var x = await Q().FirstOrDefaultAsync(x => x.AthleteUserId == athleteId && x.Id == id, ct); return x is null ? null : Map(x); }
    public async Task<TrainingRequestResponse?> CancelAsync(int athleteId, int id, CancellationToken ct) { var x = await db.TrainingRequests.FirstOrDefaultAsync(x => x.AthleteUserId == athleteId && x.Id == id && x.Status == TrainingRequestStatuses.Pending, ct); if (x is null) return null; x.Status = TrainingRequestStatuses.Cancelled; x.CancelledAtUtc = DateTime.UtcNow; db.NotificationEvents.Add(NotificationEventFactory.Create(NotificationTypes.TrainingRequestCancelled,$"TrainingRequest:{x.Id}:Cancelled",x.RequestedRecipientUserId,athleteId,"Training Request Cancelled","A training request was cancelled.","TrainingRequest",x.Id,$"/training-requests/{x.Id}",x.CancelledAtUtc.Value)); await db.SaveChangesAsync(ct); return await GetForAthleteAsync(athleteId, id, ct); }
    public async Task<IReadOnlyCollection<TrainingRequestResponse>> InboxAsync(int recipientId, string role, CancellationToken ct) => (await Q().Where(x => x.RequestedRecipientUserId == recipientId && x.RequestedRecipientRole == role).OrderByDescending(x => x.CreatedAtUtc).ToListAsync(ct)).Select(Map).ToList();
    public async Task<TrainingRequestResponse?> InboxItemAsync(int recipientId, string role, int id, CancellationToken ct) { var x = await Q().FirstOrDefaultAsync(x => x.Id == id && x.RequestedRecipientUserId == recipientId && x.RequestedRecipientRole == role, ct); return x is null ? null : Map(x); }
    public async Task<(TrainingRequestResponse? Value, string? Error)> ApproveAsync(int recipientId, string role, int id, ApproveTrainingRequest r, CancellationToken ct)
    {
        var existing = await db.TrainingRequests.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.RequestedRecipientUserId == recipientId && x.RequestedRecipientRole == role, ct);
        if (existing is null) return (null, null); if (existing.Status == TrainingRequestStatuses.Approved) return (await InboxItemAsync(recipientId, role, id, ct), null); if (existing.Status != TrainingRequestStatuses.Pending) return (null, "Request is not pending.");
        if (role == ApplicationRoles.Parent && !await access.CanParentAccessAthleteAsync(recipientId, existing.AthleteUserId, ct)) return (null, null);
        if (role == ApplicationRoles.Coach && (existing.TeamId is null || !await access.CanCoachManageTeamAsync(recipientId, existing.TeamId.Value, ct) || !(await access.GetCoachTeamAthleteIdsAsync(recipientId, existing.TeamId.Value, ct)).Contains(existing.AthleteUserId))) return (null, null);
        string? error = null;
        await db.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
        {
            await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
            var current = await db.TrainingRequests.FirstAsync(x => x.Id == id, ct);
            if (current.Status != TrainingRequestStatuses.Pending) { await tx.RollbackAsync(ct); return; }
            var command = new AssignmentCreateCommand(r.DrillId, [current.AthleteUserId], r.ScheduledForUtc, r.DueAtUtc, r.Instructions, r.CountsTowardProgression);
            var result = role == ApplicationRoles.Parent ? await assignments.CreateForParentAsync(recipientId, command, ct) : await assignments.CreateForSelectedTeamAthletesAsync(recipientId, current.TeamId!.Value, command, ct);
            if (result.Value is null) { error = result.Error ?? "Assignment creation failed."; await tx.RollbackAsync(ct); return; }
            current.Status = TrainingRequestStatuses.Approved; current.RespondedAtUtc = DateTime.UtcNow; current.ApprovedAssignmentId = result.Value.AssignmentId;
            db.NotificationEvents.Add(NotificationEventFactory.Create(NotificationTypes.TrainingRequestApproved,$"TrainingRequest:{current.Id}:Approved",current.AthleteUserId,recipientId,"Training Request Approved","Your training request was approved.","TrainingRequest",current.Id,$"/training-requests/{current.Id}",current.RespondedAtUtc.Value));
            try { await db.SaveChangesAsync(ct); await tx.CommitAsync(ct); } catch (DbUpdateException) { error = "Request approval could not be persisted."; await tx.RollbackAsync(ct); }
        });
        return error is null ? (await InboxItemAsync(recipientId, role, id, ct), null) : (null, error);
    }
    public async Task<TrainingRequestResponse?> DeclineAsync(int recipientId, string role, int id, CancellationToken ct) { var x = await db.TrainingRequests.FirstOrDefaultAsync(x => x.Id == id && x.RequestedRecipientUserId == recipientId && x.RequestedRecipientRole == role, ct); if (x is null) return null; if (x.Status == TrainingRequestStatuses.Declined) return await InboxItemAsync(recipientId, role, id, ct); if (x.Status != TrainingRequestStatuses.Pending) return null; x.Status = TrainingRequestStatuses.Declined; x.RespondedAtUtc = DateTime.UtcNow; db.NotificationEvents.Add(NotificationEventFactory.Create(NotificationTypes.TrainingRequestDeclined,$"TrainingRequest:{x.Id}:Declined",x.AthleteUserId,recipientId,"Training Request Update","Your training request was declined.","TrainingRequest",x.Id,$"/training-requests/{x.Id}",x.RespondedAtUtc.Value)); await db.SaveChangesAsync(ct); return await InboxItemAsync(recipientId, role, id, ct); }
    private IQueryable<TrainingRequest> Q() => db.TrainingRequests.AsNoTracking().Include(x => x.AthleteUser).ThenInclude(x => x.Profile).Include(x => x.RequestedRecipientUser).ThenInclude(x => x.Profile).Include(x => x.Team).Include(x => x.RequestedDrill);
    private static TrainingRequestResponse Map(TrainingRequest x) => new(x.Id, new UserSummary(x.AthleteUserId, x.AthleteUser.Profile?.FullName ?? "Athlete", ApplicationRoles.Athlete), new UserSummary(x.RequestedRecipientUserId, x.RequestedRecipientUser.Profile?.FullName ?? x.RequestedRecipientRole, x.RequestedRecipientRole), x.RequestedRecipientRole, x.TeamId, x.Team?.Name, x.RequestedDrill is null ? null : new(x.RequestedDrill.Id, x.RequestedDrill.Name, x.RequestedDrill.Sport, x.RequestedDrill.Category, x.RequestedDrill.SubCategory), x.Sport, x.Category, x.SubCategory, x.Message, x.Status, x.CreatedAtUtc, x.RespondedAtUtc, x.CancelledAtUtc, x.ApprovedAssignmentId);
    private static string? N(string? x) => string.IsNullOrWhiteSpace(x) ? null : x.Trim().ToUpperInvariant();
    private static string? T(string? x) => string.IsNullOrWhiteSpace(x) ? null : x.Trim();
}
