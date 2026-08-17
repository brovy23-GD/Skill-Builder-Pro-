using SkillBuilderPro.Core.Models;
namespace SkillBuilderPro.API.Services;
public static class NotificationEventFactory
{
    public static NotificationEvent Create(string type,string key,int recipient,int? actor,string title,string message,string entity,int entityId,string route,DateTime occurred)=>new(){EventType=type,SourceKey=key,RecipientUserId=recipient,ActorUserId=actor,RelatedEntityType=entity,RelatedEntityId=entityId,Title=title,Message=message,ActionRoute=route,OccurredAtUtc=occurred,CreatedAtUtc=DateTime.UtcNow};
}
