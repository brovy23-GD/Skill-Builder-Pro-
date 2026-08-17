using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.Core.Interfaces;

public interface IAssignmentService
{
    Task<AssignmentOperationResult<DrillAssignmentView>> CreateForParentAsync(int parentUserId, AssignmentCreateCommand command, CancellationToken cancellationToken = default);
    Task<AssignmentOperationResult<DrillAssignmentView>> CreateForTeamAsync(int coachUserId, int teamId, AssignmentCreateCommand command, CancellationToken cancellationToken = default);
    Task<AssignmentOperationResult<DrillAssignmentView>> CreateForSelectedTeamAthletesAsync(int coachUserId, int teamId, AssignmentCreateCommand command, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<AthleteAssignmentView>> GetForAthleteAsync(int athleteUserId, string? recipientStatus, CancellationToken cancellationToken = default);
    Task<AthleteAssignmentView?> GetForAthleteAsync(int athleteUserId, int assignmentId, CancellationToken cancellationToken = default);
    Task<AssignmentOperationResult<AthleteAssignmentView>> StartAsync(int athleteUserId, int assignmentId, CancellationToken cancellationToken = default);
    Task<AssignmentOperationResult<AthleteAssignmentView>> CompleteAsync(int athleteUserId, int assignmentId, string? athleteNotes, int? rating, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<CreatorAssignmentSummaryView>> GetCreatedAssignmentsAsync(int creatorUserId, CancellationToken cancellationToken = default);
    Task<DrillAssignmentView?> GetCreatedAssignmentAsync(int creatorUserId, int assignmentId, CancellationToken cancellationToken = default);
    Task<AssignmentOperationResult<DrillAssignmentView>> CancelCreatedAssignmentAsync(int creatorUserId, int assignmentId, CancellationToken cancellationToken = default);
}

public sealed record AssignmentCreateCommand(
    int DrillId,
    IReadOnlyCollection<int>? AthleteUserIds,
    DateTime? ScheduledForUtc,
    DateTime? DueAtUtc,
    string? Instructions,
    bool CountsTowardProgression);

public enum AssignmentOperationStatus
{
    Success,
    Created,
    ValidationError,
    NotFound,
    Conflict
}

public sealed record AssignmentOperationResult<T>(AssignmentOperationStatus Status, T? Value = default, string? Error = null)
{
    public static AssignmentOperationResult<T> Success(T value) => new(AssignmentOperationStatus.Success, value);
    public static AssignmentOperationResult<T> Created(T value) => new(AssignmentOperationStatus.Created, value);
    public static AssignmentOperationResult<T> Validation(string error) => new(AssignmentOperationStatus.ValidationError, default, error);
    public static AssignmentOperationResult<T> NotFound() => new(AssignmentOperationStatus.NotFound);
    public static AssignmentOperationResult<T> Conflict(string error) => new(AssignmentOperationStatus.Conflict, default, error);
}

public sealed record AssignmentRecipientView(int AthleteUserId, string DisplayName, string Status, DateTime? StartedAtUtc, DateTime? CompletedAtUtc, string? AthleteNotes, int? Rating);
public sealed record DrillAssignmentView(int AssignmentId, int DrillId, string DrillName, string DrillSport, string? DrillCategory, int AssignedByUserId, string AssignedByDisplayName, int? SourceTeamId, string? SourceTeamName, DateTime? ScheduledForUtc, DateTime? DueAtUtc, string? Instructions, string Status, bool CountsTowardProgression, DateTime CreatedAtUtc, DateTime? CancelledAtUtc, IReadOnlyCollection<AssignmentRecipientView> Recipients);
public sealed record AthleteAssignmentView(int AssignmentId, int DrillId, string DrillName, string DrillSport, string? DrillCategory, int AssignedByUserId, string AssignedByDisplayName, int? SourceTeamId, string? SourceTeamName, DateTime? ScheduledForUtc, DateTime? DueAtUtc, string? Instructions, string AssignmentStatus, bool CountsTowardProgression, DateTime CreatedAtUtc, string RecipientStatus, DateTime? StartedAtUtc, DateTime? CompletedAtUtc, string? AthleteNotes, int? Rating);
public sealed record CreatorAssignmentSummaryView(int AssignmentId, int DrillId, string DrillName, string DrillSport, string? DrillCategory, int? SourceTeamId, string? SourceTeamName, DateTime? ScheduledForUtc, DateTime? DueAtUtc, string? Instructions, string Status, bool CountsTowardProgression, DateTime CreatedAtUtc, DateTime? CancelledAtUtc, int RecipientCount, int AssignedCount, int InProgressCount, int CompletedCount, int MissedCount, int ExcusedCount);
