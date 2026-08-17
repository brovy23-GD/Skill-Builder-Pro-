using Microsoft.EntityFrameworkCore;
using SkillBuilderPro.Core.Data;
using SkillBuilderPro.Core.Interfaces;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.API.Services;

public sealed class AssignmentService : IAssignmentService
{
    private readonly AppDbContext _dbContext;
    private readonly IRelationshipAccessService _relationshipAccess;

    public AssignmentService(AppDbContext dbContext, IRelationshipAccessService relationshipAccess)
    {
        _dbContext = dbContext;
        _relationshipAccess = relationshipAccess;
    }

    public async Task<AssignmentOperationResult<DrillAssignmentView>> CreateForParentAsync(
        int parentUserId,
        AssignmentCreateCommand command,
        CancellationToken cancellationToken = default)
    {
        var commonError = await ValidateCommonAsync(command, cancellationToken);
        if (commonError is not null) return AssignmentOperationResult<DrillAssignmentView>.Validation(commonError);

        var recipients = ValidateRecipientIds(command.AthleteUserIds, out var recipientError);
        if (recipientError is not null) return AssignmentOperationResult<DrillAssignmentView>.Validation(recipientError);

        foreach (var athleteUserId in recipients)
        {
            if (!await _relationshipAccess.CanParentAccessAthleteAsync(parentUserId, athleteUserId, cancellationToken)
                || !await HasActiveProfileAsync(athleteUserId, cancellationToken))
            {
                return AssignmentOperationResult<DrillAssignmentView>.NotFound();
            }
        }

        return await CreateAsync(parentUserId, null, recipients, command, cancellationToken);
    }

    public async Task<AssignmentOperationResult<DrillAssignmentView>> CreateForTeamAsync(
        int coachUserId,
        int teamId,
        AssignmentCreateCommand command,
        CancellationToken cancellationToken = default)
    {
        var commonError = await ValidateCommonAsync(command, cancellationToken);
        if (commonError is not null) return AssignmentOperationResult<DrillAssignmentView>.Validation(commonError);
        if (!await _relationshipAccess.CanCoachManageTeamAsync(coachUserId, teamId, cancellationToken))
            return AssignmentOperationResult<DrillAssignmentView>.NotFound();

        var authorizedIds = await _relationshipAccess.GetCoachTeamAthleteIdsAsync(coachUserId, teamId, cancellationToken);
        var recipients = await FilterActiveProfilesAsync(authorizedIds, cancellationToken);
        if (recipients.Count == 0)
            return AssignmentOperationResult<DrillAssignmentView>.Conflict("The team has no active Athlete recipients.");

        return await CreateAsync(coachUserId, teamId, recipients, command, cancellationToken);
    }

    public async Task<AssignmentOperationResult<DrillAssignmentView>> CreateForSelectedTeamAthletesAsync(
        int coachUserId,
        int teamId,
        AssignmentCreateCommand command,
        CancellationToken cancellationToken = default)
    {
        var commonError = await ValidateCommonAsync(command, cancellationToken);
        if (commonError is not null) return AssignmentOperationResult<DrillAssignmentView>.Validation(commonError);
        var recipients = ValidateRecipientIds(command.AthleteUserIds, out var recipientError);
        if (recipientError is not null) return AssignmentOperationResult<DrillAssignmentView>.Validation(recipientError);
        if (!await _relationshipAccess.CanCoachManageTeamAsync(coachUserId, teamId, cancellationToken))
            return AssignmentOperationResult<DrillAssignmentView>.NotFound();

        var authorizedIds = (await _relationshipAccess.GetCoachTeamAthleteIdsAsync(coachUserId, teamId, cancellationToken)).ToHashSet();
        foreach (var athleteUserId in recipients)
        {
            if (!authorizedIds.Contains(athleteUserId)
                || !await HasActiveProfileAsync(athleteUserId, cancellationToken))
                return AssignmentOperationResult<DrillAssignmentView>.NotFound();
        }

        return await CreateAsync(coachUserId, teamId, recipients, command, cancellationToken);
    }

    public async Task<IReadOnlyCollection<AthleteAssignmentView>> GetForAthleteAsync(
        int athleteUserId,
        string? recipientStatus,
        CancellationToken cancellationToken = default)
    {
        var query = AthleteAssignmentQuery(athleteUserId);
        if (recipientStatus is not null) query = query.Where(recipient => recipient.Status == recipientStatus);
        var recipients = await query.OrderByDescending(recipient => recipient.Assignment.CreatedAtUtc).ToListAsync(cancellationToken);
        return recipients.Select(ToAthleteView).ToList();
    }

    public async Task<AthleteAssignmentView?> GetForAthleteAsync(
        int athleteUserId,
        int assignmentId,
        CancellationToken cancellationToken = default)
    {
        var recipient = await AthleteAssignmentQuery(athleteUserId)
            .FirstOrDefaultAsync(recipient => recipient.AssignmentId == assignmentId, cancellationToken);
        return recipient is null ? null : ToAthleteView(recipient);
    }

    public async Task<AssignmentOperationResult<AthleteAssignmentView>> StartAsync(
        int athleteUserId,
        int assignmentId,
        CancellationToken cancellationToken = default)
    {
        var recipient = await MutableRecipientQuery(athleteUserId, assignmentId).FirstOrDefaultAsync(cancellationToken);
        if (recipient is null) return AssignmentOperationResult<AthleteAssignmentView>.NotFound();
        if (recipient.Assignment.Status == DrillAssignmentStatuses.Cancelled)
            return AssignmentOperationResult<AthleteAssignmentView>.Conflict("A cancelled assignment cannot be started.");
        if (recipient.Assignment.ScheduledForUtc is DateTime scheduled && scheduled > DateTime.UtcNow)
            return AssignmentOperationResult<AthleteAssignmentView>.Conflict("The assignment is not available to start yet.");
        if (recipient.Status == DrillAssignmentRecipientStatuses.InProgress)
            return AssignmentOperationResult<AthleteAssignmentView>.Success(ToAthleteView(recipient));
        if (recipient.Status != DrillAssignmentRecipientStatuses.Assigned)
            return AssignmentOperationResult<AthleteAssignmentView>.Conflict("The assignment cannot be started from its current state.");

        recipient.Status = DrillAssignmentRecipientStatuses.InProgress;
        recipient.StartedAtUtc = DateTime.UtcNow;
        if (recipient.Assignment.Status == DrillAssignmentStatuses.Scheduled)
            recipient.Assignment.Status = DrillAssignmentStatuses.Active;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return AssignmentOperationResult<AthleteAssignmentView>.Success(ToAthleteView(recipient));
    }

    public async Task<AssignmentOperationResult<AthleteAssignmentView>> CompleteAsync(
        int athleteUserId,
        int assignmentId,
        string? athleteNotes,
        int? rating,
        CancellationToken cancellationToken = default)
    {
        if (rating is < 1 or > 5)
            return AssignmentOperationResult<AthleteAssignmentView>.Validation("Rating must be between 1 and 5.");
        if (athleteNotes?.Length > 1000)
            return AssignmentOperationResult<AthleteAssignmentView>.Validation("Athlete notes cannot exceed 1000 characters.");

        var recipient = await MutableRecipientQuery(athleteUserId, assignmentId).FirstOrDefaultAsync(cancellationToken);
        if (recipient is null) return AssignmentOperationResult<AthleteAssignmentView>.NotFound();
        if (recipient.Assignment.Status == DrillAssignmentStatuses.Cancelled)
            return AssignmentOperationResult<AthleteAssignmentView>.Conflict("A cancelled assignment cannot be completed.");
        if (recipient.Assignment.ScheduledForUtc is DateTime scheduled && scheduled > DateTime.UtcNow)
            return AssignmentOperationResult<AthleteAssignmentView>.Conflict("The assignment is not available to complete yet.");
        if (recipient.Status == DrillAssignmentRecipientStatuses.Completed)
            return AssignmentOperationResult<AthleteAssignmentView>.Success(ToAthleteView(recipient));
        if (recipient.Status is not (DrillAssignmentRecipientStatuses.Assigned or DrillAssignmentRecipientStatuses.InProgress))
            return AssignmentOperationResult<AthleteAssignmentView>.Conflict("The assignment cannot be completed from its current state.");

        var completedAtUtc = DateTime.UtcNow;
        recipient.StartedAtUtc ??= completedAtUtc;
        recipient.CompletedAtUtc = completedAtUtc;
        recipient.Status = DrillAssignmentRecipientStatuses.Completed;
        recipient.AthleteNotes = string.IsNullOrWhiteSpace(athleteNotes) ? null : athleteNotes.Trim();
        recipient.Rating = rating;
        if (recipient.Assignment.Status == DrillAssignmentStatuses.Scheduled)
            recipient.Assignment.Status = DrillAssignmentStatuses.Active;

        _dbContext.AssignmentCompletionEvents.Add(new AssignmentCompletionEvent
        {
            AssignmentId = assignmentId,
            AthleteUserId = athleteUserId,
            DrillId = recipient.Assignment.DrillId,
            EventType = AssignmentEventTypes.RecipientCompleted,
            OccurredAtUtc = completedAtUtc,
            CreatedAtUtc = completedAtUtc,
            ProcessingAttempts = 0
        });
        _dbContext.NotificationEvents.Add(NotificationEventFactory.Create(NotificationTypes.AssignmentCompleted,$"Assignment:{assignmentId}:Athlete:{athleteUserId}:Completed",recipient.Assignment.AssignedByUserId,athleteUserId,"Assignment Completed",$"An athlete completed {recipient.Assignment.Drill.Name}.","DrillAssignment",assignmentId,$"/assignments/{assignmentId}",completedAtUtc));

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return AssignmentOperationResult<AthleteAssignmentView>.Conflict(
                "The assignment completion could not be recorded. Try again.");
        }
        return AssignmentOperationResult<AthleteAssignmentView>.Success(ToAthleteView(recipient));
    }

    public async Task<IReadOnlyCollection<CreatorAssignmentSummaryView>> GetCreatedAssignmentsAsync(
        int creatorUserId,
        CancellationToken cancellationToken = default) =>
        await _dbContext.DrillAssignments.AsNoTracking()
            .Where(assignment => assignment.AssignedByUserId == creatorUserId)
            .OrderByDescending(assignment => assignment.CreatedAtUtc)
            .Select(assignment => new CreatorAssignmentSummaryView(
                assignment.Id,
                assignment.DrillId,
                assignment.Drill.Name,
                assignment.Drill.Sport,
                assignment.Drill.Category,
                assignment.SourceTeamId,
                assignment.SourceTeam == null ? null : assignment.SourceTeam.Name,
                assignment.ScheduledForUtc,
                assignment.DueAtUtc,
                assignment.Instructions,
                assignment.Status,
                assignment.CountsTowardProgression,
                assignment.CreatedAtUtc,
                assignment.CancelledAtUtc,
                assignment.Recipients.Count,
                assignment.Recipients.Count(recipient => recipient.Status == DrillAssignmentRecipientStatuses.Assigned),
                assignment.Recipients.Count(recipient => recipient.Status == DrillAssignmentRecipientStatuses.InProgress),
                assignment.Recipients.Count(recipient => recipient.Status == DrillAssignmentRecipientStatuses.Completed),
                assignment.Recipients.Count(recipient => recipient.Status == DrillAssignmentRecipientStatuses.Missed),
                assignment.Recipients.Count(recipient => recipient.Status == DrillAssignmentRecipientStatuses.Excused)))
            .ToListAsync(cancellationToken);

    public async Task<DrillAssignmentView?> GetCreatedAssignmentAsync(
        int creatorUserId,
        int assignmentId,
        CancellationToken cancellationToken = default)
    {
        var assignment = await CreatorAssignmentQuery(creatorUserId)
            .FirstOrDefaultAsync(assignment => assignment.Id == assignmentId, cancellationToken);
        return assignment is null ? null : ToAssignmentView(assignment);
    }

    public async Task<AssignmentOperationResult<DrillAssignmentView>> CancelCreatedAssignmentAsync(
        int creatorUserId,
        int assignmentId,
        CancellationToken cancellationToken = default)
    {
        var assignment = await CreatorAssignmentQuery(creatorUserId, tracking: true)
            .FirstOrDefaultAsync(assignment => assignment.Id == assignmentId, cancellationToken);
        if (assignment is null) return AssignmentOperationResult<DrillAssignmentView>.NotFound();
        if (assignment.Status == DrillAssignmentStatuses.Cancelled)
            return AssignmentOperationResult<DrillAssignmentView>.Success(ToAssignmentView(assignment));
        if (assignment.Status == DrillAssignmentStatuses.Closed)
            return AssignmentOperationResult<DrillAssignmentView>.Conflict("A closed assignment cannot be cancelled.");

        assignment.Status = DrillAssignmentStatuses.Cancelled;
        assignment.CancelledAtUtc = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return AssignmentOperationResult<DrillAssignmentView>.Success(ToAssignmentView(assignment));
    }

    private async Task<AssignmentOperationResult<DrillAssignmentView>> CreateAsync(
        int actorUserId,
        int? sourceTeamId,
        IReadOnlyCollection<int> athleteUserIds,
        AssignmentCreateCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var now = DateTime.UtcNow;
            var assignment = new DrillAssignment
            {
                DrillId = command.DrillId,
                AssignedByUserId = actorUserId,
                SourceTeamId = sourceTeamId,
                ScheduledForUtc = command.ScheduledForUtc,
                DueAtUtc = command.DueAtUtc,
                Instructions = string.IsNullOrWhiteSpace(command.Instructions) ? null : command.Instructions.Trim(),
                Status = command.ScheduledForUtc > now ? DrillAssignmentStatuses.Scheduled : DrillAssignmentStatuses.Active,
                CountsTowardProgression = command.CountsTowardProgression,
                CreatedAtUtc = now,
                Recipients = athleteUserIds.Select(athleteUserId => new DrillAssignmentRecipient
                {
                    AthleteUserId = athleteUserId,
                    Status = DrillAssignmentRecipientStatuses.Assigned
                }).ToList()
            };

            _dbContext.DrillAssignments.Add(assignment);
            await _dbContext.SaveChangesAsync(cancellationToken);
            foreach(var athleteUserId in athleteUserIds)_dbContext.NotificationEvents.Add(NotificationEventFactory.Create(NotificationTypes.AssignmentCreated,$"Assignment:{assignment.Id}:Athlete:{athleteUserId}",athleteUserId,actorUserId,"New Training Assignment","You have a new training assignment.","DrillAssignment",assignment.Id,$"/assignments/{assignment.Id}",now));
            await _dbContext.SaveChangesAsync(cancellationToken);
            var created = await LoadAssignmentAsync(assignment.Id, cancellationToken);
            return AssignmentOperationResult<DrillAssignmentView>.Created(ToAssignmentView(created!));
        }
        catch (DbUpdateException)
        {
            return AssignmentOperationResult<DrillAssignmentView>.Conflict("The assignment could not be created because its recipients changed. Try again.");
        }
    }

    private async Task<string?> ValidateCommonAsync(AssignmentCreateCommand command, CancellationToken cancellationToken)
    {
        if (command.DrillId <= 0 || !await _dbContext.Drills.AsNoTracking().AnyAsync(drill => drill.Id == command.DrillId, cancellationToken))
            return "The selected Drill does not exist.";
        if (command.Instructions?.Length > 1000) return "Instructions cannot exceed 1000 characters.";
        if (command.ScheduledForUtc is DateTime scheduled && scheduled.Kind != DateTimeKind.Utc) return "ScheduledForUtc must use UTC.";
        if (command.DueAtUtc is DateTime due && due.Kind != DateTimeKind.Utc) return "DueAtUtc must use UTC.";
        if (command.ScheduledForUtc is DateTime scheduledUtc && command.DueAtUtc is DateTime dueUtc && dueUtc < scheduledUtc)
            return "DueAtUtc cannot be before ScheduledForUtc.";
        return null;
    }

    private static IReadOnlyCollection<int> ValidateRecipientIds(IReadOnlyCollection<int>? ids, out string? error)
    {
        error = null;
        if (ids is null || ids.Count == 0) { error = "At least one Athlete recipient is required."; return []; }
        if (ids.Any(id => id <= 0)) { error = "Athlete recipient IDs must be positive."; return []; }
        if (ids.Count != ids.Distinct().Count()) { error = "Duplicate Athlete recipients are not allowed."; return []; }
        return ids.ToList();
    }

    private Task<bool> HasActiveProfileAsync(int athleteUserId, CancellationToken cancellationToken) =>
        _dbContext.UserProfiles.AsNoTracking().AnyAsync(profile => profile.UserId == athleteUserId && profile.IsActive, cancellationToken);

    private async Task<IReadOnlyCollection<int>> FilterActiveProfilesAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken) =>
        await _dbContext.UserProfiles.AsNoTracking().Where(profile => ids.Contains(profile.UserId) && profile.IsActive).Select(profile => profile.UserId).ToListAsync(cancellationToken);

    private IQueryable<DrillAssignmentRecipient> AthleteAssignmentQuery(int athleteUserId) =>
        _dbContext.DrillAssignmentRecipients.AsNoTracking()
            .Where(recipient => recipient.AthleteUserId == athleteUserId)
            .Include(recipient => recipient.Assignment).ThenInclude(assignment => assignment.Drill)
            .Include(recipient => recipient.Assignment).ThenInclude(assignment => assignment.AssignedByUser).ThenInclude(user => user.Profile)
            .Include(recipient => recipient.Assignment).ThenInclude(assignment => assignment.SourceTeam);

    private IQueryable<DrillAssignmentRecipient> MutableRecipientQuery(int athleteUserId, int assignmentId) =>
        _dbContext.DrillAssignmentRecipients
            .Where(recipient => recipient.AthleteUserId == athleteUserId && recipient.AssignmentId == assignmentId)
            .Include(recipient => recipient.Assignment).ThenInclude(assignment => assignment.Drill)
            .Include(recipient => recipient.Assignment).ThenInclude(assignment => assignment.AssignedByUser).ThenInclude(user => user.Profile)
            .Include(recipient => recipient.Assignment).ThenInclude(assignment => assignment.SourceTeam);

    private Task<DrillAssignment?> LoadAssignmentAsync(int assignmentId, CancellationToken cancellationToken) =>
        _dbContext.DrillAssignments.AsNoTracking()
            .Include(assignment => assignment.Drill)
            .Include(assignment => assignment.AssignedByUser).ThenInclude(user => user.Profile)
            .Include(assignment => assignment.SourceTeam)
            .Include(assignment => assignment.Recipients).ThenInclude(recipient => recipient.AthleteUser).ThenInclude(user => user.Profile)
            .FirstOrDefaultAsync(assignment => assignment.Id == assignmentId, cancellationToken);

    private IQueryable<DrillAssignment> CreatorAssignmentQuery(int creatorUserId, bool tracking = false)
    {
        var query = tracking ? _dbContext.DrillAssignments : _dbContext.DrillAssignments.AsNoTracking();
        return query
            .Where(assignment => assignment.AssignedByUserId == creatorUserId)
            .Include(assignment => assignment.Drill)
            .Include(assignment => assignment.AssignedByUser).ThenInclude(user => user.Profile)
            .Include(assignment => assignment.SourceTeam)
            .Include(assignment => assignment.Recipients).ThenInclude(recipient => recipient.AthleteUser).ThenInclude(user => user.Profile);
    }

    private static DrillAssignmentView ToAssignmentView(DrillAssignment assignment) => new(
        assignment.Id, assignment.DrillId, assignment.Drill.Name, assignment.Drill.Sport, assignment.Drill.Category, assignment.AssignedByUserId,
        DisplayName(assignment.AssignedByUser), assignment.SourceTeamId, assignment.SourceTeam?.Name,
        assignment.ScheduledForUtc, assignment.DueAtUtc, assignment.Instructions, assignment.Status,
        assignment.CountsTowardProgression, assignment.CreatedAtUtc, assignment.CancelledAtUtc,
        assignment.Recipients.OrderBy(recipient => recipient.AthleteUserId).Select(recipient => new AssignmentRecipientView(
            recipient.AthleteUserId, DisplayName(recipient.AthleteUser), recipient.Status, recipient.StartedAtUtc,
            recipient.CompletedAtUtc, recipient.AthleteNotes, recipient.Rating)).ToList());

    private static AthleteAssignmentView ToAthleteView(DrillAssignmentRecipient recipient) => new(
        recipient.AssignmentId, recipient.Assignment.DrillId, recipient.Assignment.Drill.Name,
        recipient.Assignment.Drill.Sport, recipient.Assignment.Drill.Category,
        recipient.Assignment.AssignedByUserId, DisplayName(recipient.Assignment.AssignedByUser),
        recipient.Assignment.SourceTeamId, recipient.Assignment.SourceTeam?.Name,
        recipient.Assignment.ScheduledForUtc, recipient.Assignment.DueAtUtc, recipient.Assignment.Instructions,
        recipient.Assignment.Status, recipient.Assignment.CountsTowardProgression, recipient.Assignment.CreatedAtUtc,
        recipient.Status, recipient.StartedAtUtc, recipient.CompletedAtUtc, recipient.AthleteNotes, recipient.Rating);

    private static string DisplayName(Core.Identity.ApplicationUser user) =>
        string.IsNullOrWhiteSpace(user.Profile?.FullName) ? user.UserName ?? $"User {user.Id}" : user.Profile.FullName;
}
