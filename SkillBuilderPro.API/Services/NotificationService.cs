using Microsoft.EntityFrameworkCore;
using SkillBuilderPro.API.Contracts.Notifications;
using SkillBuilderPro.Core.Data;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.API.Services;
public interface INotificationService
{
    Task<NotificationPageResponse> ListAsync(int userId,int page,int pageSize,bool unreadOnly,CancellationToken ct);
    Task<NotificationResponse?> GetAsync(int userId,long id,CancellationToken ct);
    Task<int> UnreadCountAsync(int userId,CancellationToken ct);
    Task<NotificationResponse?> MarkReadAsync(int userId,long id,CancellationToken ct);
    Task<int> MarkAllReadAsync(int userId,CancellationToken ct);
}
public sealed class NotificationService(AppDbContext db):INotificationService
{
    public async Task<NotificationPageResponse> ListAsync(int userId,int page,int size,bool unread,CancellationToken ct){page=Math.Max(1,page);size=Math.Clamp(size,1,100);var q=db.Notifications.AsNoTracking().Where(x=>x.RecipientUserId==userId);var unreadCount=await q.CountAsync(x=>!x.IsRead,ct);if(unread)q=q.Where(x=>!x.IsRead);var total=await q.CountAsync(ct);var items=await q.OrderByDescending(x=>x.CreatedAtUtc).ThenByDescending(x=>x.Id).Skip((page-1)*size).Take(size).Select(x=>new NotificationResponse(x.Id,x.Type,x.Title,x.Message,x.RelatedEntityType,x.RelatedEntityId,x.IsRead,x.ReadAtUtc,x.CreatedAtUtc,x.ActionRoute)).ToListAsync(ct);return new(items,page,size,total,unreadCount);}
    public Task<int> UnreadCountAsync(int id,CancellationToken ct)=>db.Notifications.CountAsync(x=>x.RecipientUserId==id&&!x.IsRead,ct);
    public async Task<NotificationResponse?> GetAsync(int uid,long id,CancellationToken ct){var x=await db.Notifications.AsNoTracking().FirstOrDefaultAsync(x=>x.Id==id&&x.RecipientUserId==uid,ct);return x is null?null:Map(x);}
    public async Task<NotificationResponse?> MarkReadAsync(int uid,long id,CancellationToken ct){var x=await db.Notifications.FirstOrDefaultAsync(x=>x.Id==id&&x.RecipientUserId==uid,ct);if(x is null)return null;if(!x.IsRead){x.IsRead=true;x.ReadAtUtc=DateTime.UtcNow;await db.SaveChangesAsync(ct);}return Map(x);}
    public Task<int> MarkAllReadAsync(int uid,CancellationToken ct){var now=DateTime.UtcNow;return db.Notifications.Where(x=>x.RecipientUserId==uid&&!x.IsRead).ExecuteUpdateAsync(s=>s.SetProperty(x=>x.IsRead,true).SetProperty(x=>x.ReadAtUtc,now),ct);}
    private static NotificationResponse Map(Notification x)=>new(x.Id,x.Type,x.Title,x.Message,x.RelatedEntityType,x.RelatedEntityId,x.IsRead,x.ReadAtUtc,x.CreatedAtUtc,x.ActionRoute);
}
