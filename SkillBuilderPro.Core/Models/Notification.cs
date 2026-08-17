using System.ComponentModel.DataAnnotations;
using SkillBuilderPro.Core.Identity;

namespace SkillBuilderPro.Core.Models;

public static class NotificationTypes
{
    public const string TrainingRequestReceived="TRAINING_REQUEST_RECEIVED", TrainingRequestApproved="TRAINING_REQUEST_APPROVED", TrainingRequestDeclined="TRAINING_REQUEST_DECLINED", TrainingRequestCancelled="TRAINING_REQUEST_CANCELLED", AssignmentCreated="ASSIGNMENT_CREATED", AssignmentCompleted="ASSIGNMENT_COMPLETED", GoalCompleted="GOAL_COMPLETED", RankEarned="RANK_EARNED", SkillLevelEarned="SKILL_LEVEL_EARNED", AchievementEarned="ACHIEVEMENT_EARNED";
    public static readonly string[] All=[TrainingRequestReceived,TrainingRequestApproved,TrainingRequestDeclined,TrainingRequestCancelled,AssignmentCreated,AssignmentCompleted,GoalCompleted,RankEarned,SkillLevelEarned,AchievementEarned];
}
public sealed class Notification
{
    public long Id { get; set; }
    public int RecipientUserId { get; set; }
    public int? ActorUserId { get; set; }
    [MaxLength(100)] public string Type { get; set; }=string.Empty;
    [MaxLength(200)] public string SourceKey { get; set; }=string.Empty;
    [MaxLength(150)] public string Title { get; set; }=string.Empty;
    [MaxLength(500)] public string Message { get; set; }=string.Empty;
    [MaxLength(100)] public string? RelatedEntityType { get; set; }
    public int? RelatedEntityId { get; set; }
    [MaxLength(300)] public string? ActionRoute { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public ApplicationUser RecipientUser { get; set; }=null!;
    public ApplicationUser? ActorUser { get; set; }
}
public sealed class NotificationEvent
{
    public long Id { get; set; }
    [MaxLength(100)] public string EventType { get; set; }=string.Empty;
    [MaxLength(200)] public string SourceKey { get; set; }=string.Empty;
    public int RecipientUserId { get; set; }
    public int? ActorUserId { get; set; }
    public int? SubjectUserId { get; set; }
    [MaxLength(100)] public string? RelatedEntityType { get; set; }
    public int? RelatedEntityId { get; set; }
    [MaxLength(150)] public string Title { get; set; }=string.Empty;
    [MaxLength(500)] public string Message { get; set; }=string.Empty;
    [MaxLength(300)] public string? ActionRoute { get; set; }
    public DateTime OccurredAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? ProcessedAtUtc { get; set; }
    public int ProcessingAttempts { get; set; }
    [MaxLength(1000)] public string? LastError { get; set; }
    public ApplicationUser RecipientUser { get; set; }=null!;
    public ApplicationUser? ActorUser { get; set; }
    public ApplicationUser? SubjectUser { get; set; }
}
