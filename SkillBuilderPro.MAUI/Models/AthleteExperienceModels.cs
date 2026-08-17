namespace SkillBuilderPro.MAUI.Models;
public sealed record LoginRequest(string Email,string Password);
public sealed record RegisterRequest(string Email,string Password,string FullName,string Role);
public sealed record AuthResponse(string AccessToken,DateTime ExpiresAtUtc,CurrentUser User);
public sealed record CurrentUser(int UserId,string Email,string FullName,IReadOnlyCollection<string> Roles,string Phone,string Sport,string TargetArea,string ExperienceLevel,bool IsActive);
public sealed record Progression(int AthleteUserId,string OverallRank,int OverallRankNumber,int ProgressionScore,int ProgressToNextRank,string? NextRank,int? NextRankThreshold,int? PointsToNextRank,int TotalQualifyingCompletions,int ActiveSkillCount,int CurrentStreak,int LongestStreak,DateTime? LastCompletedAtUtc,DateTime? UpdatedAtUtc);
public sealed record Goal(int GoalId,int AthleteUserId,string GoalType,string? Sport,string? Category,string? SubCategory,string Title,string? Description,int TargetValue,string? TargetDisplayName,int CurrentValue,int ProgressPercent,string Status,bool IsComplete,bool IsOverdue,DateTime? DueAtUtc,DateTime CreatedAtUtc,DateTime UpdatedAtUtc,DateTime? CompletedAtUtc,DateTime? CancelledAtUtc);
public sealed record RankHistory(int RankNumber,string RankName,DateTime EarnedAtUtc,int ProgressionScoreAtEarned,int TotalQualifyingCompletionsAtEarned,int ActiveSkillCountAtEarned);
public sealed record SkillMilestone(string Sport,string Category,string SubCategory,int Level,string LevelName,DateTime EarnedAtUtc,int QualifyingCompletionsAtEarned,double? AverageRatingAtEarned);
public sealed record Achievement(string Code,string Name,string Description,string Category,string Tier,bool IsEarned,DateTime? EarnedAtUtc,int SortOrder);
public sealed record TrophyRoom(Progression CurrentProgression,IReadOnlyCollection<RankHistory> RankHistory,IReadOnlyCollection<SkillMilestone> SkillMilestones,IReadOnlyCollection<Achievement> Achievements);
public sealed record DrillSummary(int DrillId,string Name,string Sport,string? Category);
public sealed record UserSummary(int UserId,string DisplayName);
public sealed record AthleteAssignment(int AssignmentId,DrillSummary Drill,UserSummary AssignedBy,object? SourceTeam,DateTime? ScheduledForUtc,DateTime? DueAtUtc,string? Instructions,string AssignmentStatus,bool CountsTowardProgression,DateTime CreatedAtUtc,string RecipientStatus,DateTime? StartedAtUtc,DateTime? CompletedAtUtc,string? AthleteNotes,int? Rating);
public sealed record NotificationItem(long NotificationId,string Type,string Title,string Message,string? RelatedEntityType,int? RelatedEntityId,bool IsRead,DateTime? ReadAtUtc,DateTime CreatedAtUtc,string? ActionRoute);
public sealed record NotificationPage(IReadOnlyCollection<NotificationItem> Items,int Page,int PageSize,int TotalCount,int UnreadCount);
public sealed class UnreadCount{[System.Text.Json.Serialization.JsonPropertyName("UnreadCount")]public int UnreadCountValue{get;set;}}
public sealed record RequestPerson(int UserId,string DisplayName,string Role);
public sealed record TrainingRequestItem(int RequestId,RequestPerson Athlete,RequestPerson Recipient,string RecipientRole,int? TeamId,string? TeamName,object? RequestedDrill,string? Sport,string? Category,string? SubCategory,string? Message,string Status,DateTime CreatedAtUtc,DateTime? RespondedAtUtc,DateTime? CancelledAtUtc,int? ApprovedAssignmentId);
