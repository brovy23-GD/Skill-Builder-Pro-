using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SkillBuilderPro.Core.Data;
using SkillBuilderPro.Core.Interfaces;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.API.Services;

public sealed class AssignmentCompletionEventProcessor : IAssignmentCompletionEventProcessor
{
    private readonly AppDbContext _dbContext;
    private readonly AssignmentCompletionProcessingOptions _options;
    private readonly ILogger<AssignmentCompletionEventProcessor> _logger;
    private readonly IProgressionService _progressionService;
    private readonly IProgressionMilestoneService _milestoneService;
    private readonly IGoalService _goalService;

    public AssignmentCompletionEventProcessor(
        AppDbContext dbContext,
        IOptions<AssignmentCompletionProcessingOptions> options,
        ILogger<AssignmentCompletionEventProcessor> logger,
        IProgressionService progressionService,
        IProgressionMilestoneService milestoneService,
        IGoalService goalService)
    {
        _dbContext = dbContext;
        _options = options.Value;
        _logger = logger;
        _progressionService = progressionService;
        _milestoneService = milestoneService;
        _goalService = goalService;
    }

    public async Task<int> ProcessPendingBatchAsync(CancellationToken cancellationToken = default)
    {
        var eventIds = await _dbContext.AssignmentCompletionEvents
            .AsNoTracking()
            .Where(completionEvent => completionEvent.ProcessedAtUtc == null
                && completionEvent.ProcessingAttempts < _options.MaxAttempts)
            .OrderBy(completionEvent => completionEvent.CreatedAtUtc)
            .ThenBy(completionEvent => completionEvent.Id)
            .Select(completionEvent => completionEvent.Id)
            .Take(_options.BatchSize)
            .ToListAsync(cancellationToken);

        if (eventIds.Count > 0)
        {
            _logger.LogInformation(
                "Discovered {EventCount} pending assignment completion events.",
                eventIds.Count);
        }

        foreach (var eventId in eventIds)
        {
            await ProcessOneAsync(eventId, cancellationToken);
        }

        return eventIds.Count;
    }

    private async Task ProcessOneAsync(long eventId, CancellationToken cancellationToken)
    {
        var completionEvent = await _dbContext.AssignmentCompletionEvents
            .Include(current => current.Assignment)
            .ThenInclude(assignment => assignment.Recipients)
            .FirstOrDefaultAsync(current => current.Id == eventId, cancellationToken);

        if (completionEvent is null
            || completionEvent.ProcessedAtUtc is not null
            || completionEvent.ProcessingAttempts >= _options.MaxAttempts)
        {
            return;
        }

        completionEvent.ProcessingAttempts++;
        completionEvent.LastError = null;

        var validationError = Validate(completionEvent, out var recipient);
        if (validationError is not null)
        {
            await RecordPermanentFailureAsync(completionEvent, validationError, cancellationToken);
            return;
        }

        var existingProgress = await _dbContext.ProgressLogs
            .AsNoTracking()
            .AnyAsync(log => log.AssignmentCompletionEventId == completionEvent.Id, cancellationToken);
        if (existingProgress)
        {
            completionEvent.ProcessedAtUtc = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync(cancellationToken);
            await RecalculateProgressionSafelyAsync(completionEvent.AthleteUserId, cancellationToken);
            _logger.LogInformation(
                "Assignment completion event {EventId} was already linked and is now marked processed.",
                completionEvent.Id);
            return;
        }

        if (!completionEvent.Assignment.CountsTowardProgression)
        {
            completionEvent.ProcessedAtUtc = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation(
                "Assignment completion event {EventId} was processed without ProgressLog because progression evidence is disabled.",
                completionEvent.Id);
            return;
        }

        _dbContext.ProgressLogs.Add(new ProgressLog
        {
            DrillId = completionEvent.DrillId,
            LogDate = completionEvent.OccurredAtUtc,
            Rating = recipient!.Rating,
            Notes = recipient.AthleteNotes ?? string.Empty,
            OwnerUserId = completionEvent.AthleteUserId,
            AssignmentCompletionEventId = completionEvent.Id
        });
        completionEvent.ProcessedAtUtc = DateTime.UtcNow;

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            await RecalculateProgressionSafelyAsync(completionEvent.AthleteUserId, cancellationToken);
            _logger.LogInformation(
                "Processed assignment completion event {EventId} for assignment {AssignmentId} and Athlete {AthleteUserId}.",
                completionEvent.Id,
                completionEvent.AssignmentId,
                completionEvent.AthleteUserId);
        }
        catch (DbUpdateException)
        {
            await RecoverFromPersistenceRaceAsync(eventId, cancellationToken);
        }
    }

    private static string? Validate(
        AssignmentCompletionEvent completionEvent,
        out DrillAssignmentRecipient? recipient)
    {
        recipient = null;
        if (completionEvent.EventType != AssignmentEventTypes.RecipientCompleted)
            return "Unsupported assignment completion event type.";
        if (completionEvent.Assignment.Id != completionEvent.AssignmentId)
            return "Assignment completion event has an invalid assignment reference.";
        if (completionEvent.Assignment.DrillId != completionEvent.DrillId)
            return "Assignment completion event Drill does not match the assignment.";

        recipient = completionEvent.Assignment.Recipients.FirstOrDefault(candidate =>
            candidate.AthleteUserId == completionEvent.AthleteUserId);
        if (recipient is null)
            return "Assignment completion event recipient does not exist.";
        if (recipient.Status != DrillAssignmentRecipientStatuses.Completed)
            return "Assignment completion event recipient is not Completed.";
        if (recipient.CompletedAtUtc is null)
            return "Assignment completion event recipient has no completion timestamp.";
        if ((recipient.CompletedAtUtc.Value - completionEvent.OccurredAtUtc).Duration() > TimeSpan.FromSeconds(1))
            return "Assignment completion event timestamp does not match the recipient completion.";
        if (recipient.AthleteNotes?.Length > 300)
            return "Assignment completion event notes exceed the ProgressLog limit.";
        return null;
    }

    private async Task RecordPermanentFailureAsync(
        AssignmentCompletionEvent completionEvent,
        string safeError,
        CancellationToken cancellationToken)
    {
        completionEvent.LastError = safeError;
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogWarning(
            "Assignment completion event {EventId} failed validation on attempt {AttemptCount}: {FailureReason}",
            completionEvent.Id,
            completionEvent.ProcessingAttempts,
            safeError);
    }

    private async Task RecoverFromPersistenceRaceAsync(long eventId, CancellationToken cancellationToken)
    {
        _dbContext.ChangeTracker.Clear();
        var linkedProgressExists = await _dbContext.ProgressLogs
            .AsNoTracking()
            .AnyAsync(log => log.AssignmentCompletionEventId == eventId, cancellationToken);
        var completionEvent = await _dbContext.AssignmentCompletionEvents
            .FirstOrDefaultAsync(current => current.Id == eventId, cancellationToken);
        if (completionEvent is null || completionEvent.ProcessedAtUtc is not null) return;

        completionEvent.ProcessingAttempts++;
        if (linkedProgressExists)
        {
            completionEvent.ProcessedAtUtc = DateTime.UtcNow;
            completionEvent.LastError = null;
            _logger.LogInformation(
                "Recovered assignment completion event {EventId} after a duplicate ProgressLog race.",
                eventId);
        }
        else
        {
            completionEvent.LastError = "ProgressLog persistence failed; the event will be retried.";
            _logger.LogWarning(
                "Assignment completion event {EventId} encountered a retryable persistence failure.",
                eventId);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        if (linkedProgressExists)
        {
            await RecalculateProgressionSafelyAsync(completionEvent.AthleteUserId, cancellationToken);
        }
    }

    private async Task RecalculateProgressionSafelyAsync(int athleteUserId, CancellationToken cancellationToken)
    {
        try
        {
            await _progressionService.RecalculateAthleteAsync(athleteUserId, cancellationToken);
            await _milestoneService.SyncMilestonesAsync(athleteUserId, cancellationToken);
            await _goalService.SynchronizeAthleteGoalsAsync(athleteUserId, cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            _logger.LogWarning(
                "Progression recalculation for Athlete {AthleteUserId} failed with {ExceptionType}; durable ProgressLog evidence remains available for repair.",
                athleteUserId,
                exception.GetType().Name);
        }
    }
}
