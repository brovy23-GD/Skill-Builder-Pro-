using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SkillBuilderPro.Core.Data;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.API.Services;
public sealed class NotificationProcessingOptions{public const string SectionName="NotificationProcessing";public int PollIntervalSeconds{get;set;}=10;public int BatchSize{get;set;}=20;public int MaxAttempts{get;set;}=5;}
public interface INotificationEventProcessor{Task<int> ProcessPendingBatchAsync(CancellationToken ct);}
public sealed class NotificationEventProcessor(AppDbContext db,IOptions<NotificationProcessingOptions> options,ILogger<NotificationEventProcessor> logger):INotificationEventProcessor
{
    public async Task<int> ProcessPendingBatchAsync(CancellationToken ct){var ids=await db.NotificationEvents.AsNoTracking().Where(x=>x.ProcessedAtUtc==null&&x.ProcessingAttempts<options.Value.MaxAttempts).OrderBy(x=>x.CreatedAtUtc).Select(x=>x.Id).Take(options.Value.BatchSize).ToListAsync(ct);foreach(var id in ids)await Process(id,ct);return ids.Count;}
    private async Task Process(long id,CancellationToken ct){var e=await db.NotificationEvents.FirstOrDefaultAsync(x=>x.Id==id&&x.ProcessedAtUtc==null,ct);if(e is null||e.ProcessingAttempts>=options.Value.MaxAttempts)return;e.ProcessingAttempts++;try{if(!await db.Notifications.AnyAsync(x=>x.RecipientUserId==e.RecipientUserId&&x.Type==e.EventType&&x.SourceKey==e.SourceKey,ct))db.Notifications.Add(new Notification{RecipientUserId=e.RecipientUserId,ActorUserId=e.ActorUserId,Type=e.EventType,SourceKey=e.SourceKey,Title=e.Title,Message=e.Message,RelatedEntityType=e.RelatedEntityType,RelatedEntityId=e.RelatedEntityId,ActionRoute=e.ActionRoute,CreatedAtUtc=DateTime.UtcNow});e.ProcessedAtUtc=DateTime.UtcNow;e.LastError=null;await db.SaveChangesAsync(ct);}catch(DbUpdateException){db.ChangeTracker.Clear();var exists=await db.Notifications.AsNoTracking().AnyAsync(x=>x.RecipientUserId==e.RecipientUserId&&x.Type==e.EventType&&x.SourceKey==e.SourceKey,ct);var current=await db.NotificationEvents.FirstAsync(x=>x.Id==id,ct);if(exists){current.ProcessedAtUtc=DateTime.UtcNow;current.LastError=null;}else current.LastError="Notification persistence failed; retry pending.";await db.SaveChangesAsync(ct);logger.LogWarning("Notification event {NotificationEventId} persistence race on attempt {Attempt}.",id,current.ProcessingAttempts);}}
}
public sealed class NotificationEventBackgroundService(IServiceScopeFactory scopes,IOptions<NotificationProcessingOptions> options,ILogger<NotificationEventBackgroundService> logger):BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stop){while(!stop.IsCancellationRequested){try{using var scope=scopes.CreateScope();await scope.ServiceProvider.GetRequiredService<INotificationEventProcessor>().ProcessPendingBatchAsync(stop);}catch(Exception ex)when(ex is not OperationCanceledException){logger.LogWarning("Notification processor cycle failed with {ExceptionType}.",ex.GetType().Name);}await Task.Delay(TimeSpan.FromSeconds(options.Value.PollIntervalSeconds),stop);}}
}
