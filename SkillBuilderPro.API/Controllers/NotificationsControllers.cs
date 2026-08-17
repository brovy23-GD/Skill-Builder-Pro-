using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillBuilderPro.API.Contracts.Notifications;
using SkillBuilderPro.API.Security;
using SkillBuilderPro.API.Services;
using SkillBuilderPro.Core.Identity;
namespace SkillBuilderPro.API.Controllers;
public abstract class NotificationsControllerBase(ICurrentUser user,INotificationService service):ControllerBase
{
    [HttpGet] public async Task<ActionResult<NotificationPageResponse>> List([FromQuery]int page=1,[FromQuery]int pageSize=20,[FromQuery]bool unreadOnly=false,CancellationToken ct=default)=>user.UserId is int id?Ok(await service.ListAsync(id,page,pageSize,unreadOnly,ct)):Unauthorized();
    [HttpGet("unread-count")] public async Task<ActionResult<UnreadCountResponse>> Count(CancellationToken ct)=>user.UserId is int id?Ok(new UnreadCountResponse(await service.UnreadCountAsync(id,ct))):Unauthorized();
    [HttpGet("{notificationId:long}")] public async Task<ActionResult<NotificationResponse>> Get(long notificationId,CancellationToken ct){if(user.UserId is not int id)return Unauthorized();var x=await service.GetAsync(id,notificationId,ct);return x is null?NotFound():Ok(x);}
    [HttpPost("{notificationId:long}/read")] public async Task<ActionResult<NotificationResponse>> Read(long notificationId,CancellationToken ct){if(user.UserId is not int id)return Unauthorized();var x=await service.MarkReadAsync(id,notificationId,ct);return x is null?NotFound():Ok(x);}
    [HttpPost("read-all")] public async Task<ActionResult<MarkAllReadResponse>> ReadAll(CancellationToken ct)=>user.UserId is int id?Ok(new MarkAllReadResponse(await service.MarkAllReadAsync(id,ct))):Unauthorized();
}
[ApiController,Authorize(Roles=ApplicationRoles.Athlete),Route("api/athlete/notifications")] public sealed class AthleteNotificationsController(ICurrentUser u,INotificationService s):NotificationsControllerBase(u,s);
[ApiController,Authorize(Roles=ApplicationRoles.Parent),Route("api/parent/notifications")] public sealed class ParentNotificationsController(ICurrentUser u,INotificationService s):NotificationsControllerBase(u,s);
[ApiController,Authorize(Roles=ApplicationRoles.Coach),Route("api/coach/notifications")] public sealed class CoachNotificationsController(ICurrentUser u,INotificationService s):NotificationsControllerBase(u,s);
