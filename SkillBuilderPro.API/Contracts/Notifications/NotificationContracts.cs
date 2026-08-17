namespace SkillBuilderPro.API.Contracts.Notifications;
public sealed record NotificationResponse(long NotificationId,string Type,string Title,string Message,string? RelatedEntityType,int? RelatedEntityId,bool IsRead,DateTime? ReadAtUtc,DateTime CreatedAtUtc,string? ActionRoute);
public sealed record NotificationPageResponse(IReadOnlyCollection<NotificationResponse> Items,int Page,int PageSize,int TotalCount,int UnreadCount);
public sealed record UnreadCountResponse(int UnreadCount);
public sealed record MarkAllReadResponse(int UpdatedCount);
