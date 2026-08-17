using SkillBuilderPro.Core.Interfaces;

namespace SkillBuilderPro.API.Contracts.Assignments;

public sealed record AssignmentRecipientResponse(int AthleteUserId, string DisplayName, string Status, DateTime? StartedAtUtc, DateTime? CompletedAtUtc, string? AthleteNotes, int? Rating);
public sealed record DrillAssignmentResponse(int AssignmentId, AssignmentDrillSummary Drill, AssignmentUserSummary AssignedBy, AssignmentTeamSummary? SourceTeam, DateTime? ScheduledForUtc, DateTime? DueAtUtc, string? Instructions, string Status, bool CountsTowardProgression, DateTime CreatedAtUtc, DateTime? CancelledAtUtc, IReadOnlyCollection<AssignmentRecipientResponse> Recipients);
public sealed record CreatorAssignmentSummaryResponse(int AssignmentId, AssignmentDrillSummary Drill, AssignmentTeamSummary? SourceTeam, DateTime? ScheduledForUtc, DateTime? DueAtUtc, string? Instructions, string Status, bool CountsTowardProgression, DateTime CreatedAtUtc, DateTime? CancelledAtUtc, int RecipientCount, RecipientStatusCountsResponse RecipientCounts);
public sealed record RecipientStatusCountsResponse(int Assigned, int InProgress, int Completed, int Missed, int Excused);
public sealed record AthleteAssignmentResponse(int AssignmentId, AssignmentDrillSummary Drill, AssignmentUserSummary AssignedBy, AssignmentTeamSummary? SourceTeam, DateTime? ScheduledForUtc, DateTime? DueAtUtc, string? Instructions, string AssignmentStatus, bool CountsTowardProgression, DateTime CreatedAtUtc, string RecipientStatus, DateTime? StartedAtUtc, DateTime? CompletedAtUtc, string? AthleteNotes, int? Rating);
public sealed record AssignmentDrillSummary(int DrillId, string Name, string Sport, string? Category);
public sealed record AssignmentUserSummary(int UserId, string DisplayName);
public sealed record AssignmentTeamSummary(int TeamId, string Name);

public static class AssignmentResponseMapper
{
    public static DrillAssignmentResponse ToResponse(this DrillAssignmentView view) => new(
        view.AssignmentId,
        new AssignmentDrillSummary(view.DrillId, view.DrillName, view.DrillSport, view.DrillCategory),
        new AssignmentUserSummary(view.AssignedByUserId, view.AssignedByDisplayName),
        view.SourceTeamId is int teamId ? new AssignmentTeamSummary(teamId, view.SourceTeamName ?? string.Empty) : null,
        view.ScheduledForUtc,
        view.DueAtUtc,
        view.Instructions,
        view.Status,
        view.CountsTowardProgression,
        view.CreatedAtUtc,
        view.CancelledAtUtc,
        view.Recipients.Select(recipient => new AssignmentRecipientResponse(recipient.AthleteUserId, recipient.DisplayName, recipient.Status, recipient.StartedAtUtc, recipient.CompletedAtUtc, recipient.AthleteNotes, recipient.Rating)).ToList());

    public static CreatorAssignmentSummaryResponse ToResponse(this CreatorAssignmentSummaryView view) => new(
        view.AssignmentId,
        new AssignmentDrillSummary(view.DrillId, view.DrillName, view.DrillSport, view.DrillCategory),
        view.SourceTeamId is int teamId ? new AssignmentTeamSummary(teamId, view.SourceTeamName ?? string.Empty) : null,
        view.ScheduledForUtc,
        view.DueAtUtc,
        view.Instructions,
        view.Status,
        view.CountsTowardProgression,
        view.CreatedAtUtc,
        view.CancelledAtUtc,
        view.RecipientCount,
        new RecipientStatusCountsResponse(view.AssignedCount, view.InProgressCount, view.CompletedCount, view.MissedCount, view.ExcusedCount));

    public static AthleteAssignmentResponse ToResponse(this AthleteAssignmentView view) => new(
        view.AssignmentId,
        new AssignmentDrillSummary(view.DrillId, view.DrillName, view.DrillSport, view.DrillCategory),
        new AssignmentUserSummary(view.AssignedByUserId, view.AssignedByDisplayName),
        view.SourceTeamId is int teamId ? new AssignmentTeamSummary(teamId, view.SourceTeamName ?? string.Empty) : null,
        view.ScheduledForUtc,
        view.DueAtUtc,
        view.Instructions,
        view.AssignmentStatus,
        view.CountsTowardProgression,
        view.CreatedAtUtc,
        view.RecipientStatus,
        view.StartedAtUtc,
        view.CompletedAtUtc,
        view.AthleteNotes,
        view.Rating);
}
