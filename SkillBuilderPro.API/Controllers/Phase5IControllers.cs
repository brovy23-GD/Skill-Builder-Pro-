using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillBuilderPro.API.Contracts.Goals;
using SkillBuilderPro.API.Contracts.TrainingRequests;
using SkillBuilderPro.API.Security;
using SkillBuilderPro.API.Services;
using SkillBuilderPro.Core.Identity;
using SkillBuilderPro.Core.Interfaces;

namespace SkillBuilderPro.API.Controllers;

[ApiController, Authorize(Roles = ApplicationRoles.Athlete), Route("api/athlete/goals")]
public sealed class AthleteGoalsController(ICurrentUser user, IGoalService goals) : ControllerBase
{
    [HttpPost] public async Task<ActionResult<GoalResponse>> Create(CreateGoalRequest r, CancellationToken ct) { if (user.UserId is not int id) return Unauthorized(); var x = await goals.CreateAsync(id, id, ApplicationRoles.Athlete, r, ct); return x.Value is null ? BadRequest(new { error = x.Error }) : CreatedAtAction(nameof(Get), new { goalId = x.Value.GoalId }, x.Value); }
    [HttpGet] public async Task<ActionResult<IReadOnlyCollection<GoalResponse>>> List(CancellationToken ct) => user.UserId is int id ? Ok(await goals.ListAsync(id, ct)) : Unauthorized();
    [HttpGet("{goalId:int}")] public async Task<ActionResult<GoalResponse>> Get(int goalId, CancellationToken ct) { if (user.UserId is not int id) return Unauthorized(); var x = await goals.GetAsync(id, goalId, ct); return x is null ? NotFound() : Ok(x); }
    [HttpPost("{goalId:int}/cancel")] public async Task<ActionResult<GoalResponse>> Cancel(int goalId, CancellationToken ct) { if (user.UserId is not int id) return Unauthorized(); var x = await goals.CancelAsync(id, goalId, id, ct); return x is null ? NotFound() : Ok(x); }
}

[ApiController, Authorize(Roles = ApplicationRoles.Parent), Route("api/parent/athletes/{athleteUserId:int}/goals")]
public sealed class ParentGoalsController(ICurrentUser user, IGoalService goals, IRelationshipAccessService access) : ControllerBase
{
    private async Task<bool> Allowed(int athlete, CancellationToken ct) => user.UserId is int id && await access.CanParentAccessAthleteAsync(id, athlete, ct);
    [HttpPost] public async Task<ActionResult<GoalResponse>> Create(int athleteUserId, CreateGoalRequest r, CancellationToken ct) { if (!await Allowed(athleteUserId, ct)) return NotFound(); var x = await goals.CreateAsync(athleteUserId, user.UserId!.Value, ApplicationRoles.Parent, r, ct); return x.Value is null ? BadRequest(new { error = x.Error }) : CreatedAtAction(nameof(Get), new { athleteUserId, goalId = x.Value.GoalId }, x.Value); }
    [HttpGet] public async Task<ActionResult<IReadOnlyCollection<GoalResponse>>> List(int athleteUserId, CancellationToken ct) => await Allowed(athleteUserId, ct) ? Ok(await goals.ListAsync(athleteUserId, ct)) : NotFound();
    [HttpGet("{goalId:int}")] public async Task<ActionResult<GoalResponse>> Get(int athleteUserId, int goalId, CancellationToken ct) { if (!await Allowed(athleteUserId, ct)) return NotFound(); var x = await goals.GetAsync(athleteUserId, goalId, ct); return x is null ? NotFound() : Ok(x); }
    [HttpPost("{goalId:int}/cancel")] public async Task<ActionResult<GoalResponse>> Cancel(int athleteUserId, int goalId, CancellationToken ct) { if (!await Allowed(athleteUserId, ct)) return NotFound(); var x = await goals.CancelAsync(athleteUserId, goalId, user.UserId!.Value, ct); return x is null ? NotFound() : Ok(x); }
}

[ApiController, Authorize(Roles = ApplicationRoles.Coach), Route("api/coach/athletes/{athleteUserId:int}/goals")]
public sealed class CoachGoalsController(ICurrentUser user, IGoalService goals, IRelationshipAccessService access) : ControllerBase
{
    private async Task<bool> Allowed(int athlete, CancellationToken ct) => user.UserId is int id && await access.CanCoachAccessAthleteAsync(id, athlete, ct);
    [HttpPost] public async Task<ActionResult<GoalResponse>> Create(int athleteUserId, CreateGoalRequest r, CancellationToken ct) { if (!await Allowed(athleteUserId, ct)) return NotFound(); var x = await goals.CreateAsync(athleteUserId, user.UserId!.Value, ApplicationRoles.Coach, r, ct); return x.Value is null ? BadRequest(new { error = x.Error }) : CreatedAtAction(nameof(Get), new { athleteUserId, goalId = x.Value.GoalId }, x.Value); }
    [HttpGet] public async Task<ActionResult<IReadOnlyCollection<GoalResponse>>> List(int athleteUserId, CancellationToken ct) => await Allowed(athleteUserId, ct) ? Ok(await goals.ListAsync(athleteUserId, ct)) : NotFound();
    [HttpGet("{goalId:int}")] public async Task<ActionResult<GoalResponse>> Get(int athleteUserId, int goalId, CancellationToken ct) { if (!await Allowed(athleteUserId, ct)) return NotFound(); var x = await goals.GetAsync(athleteUserId, goalId, ct); return x is null ? NotFound() : Ok(x); }
    [HttpPost("{goalId:int}/cancel")] public async Task<ActionResult<GoalResponse>> Cancel(int athleteUserId, int goalId, CancellationToken ct) { if (!await Allowed(athleteUserId, ct)) return NotFound(); var x = await goals.CancelAsync(athleteUserId, goalId, user.UserId!.Value, ct); return x is null ? NotFound() : Ok(x); }
}

[ApiController, Authorize(Roles = ApplicationRoles.Athlete), Route("api/athlete/training-requests")]
public sealed class AthleteTrainingRequestsController(ICurrentUser user, ITrainingRequestService requests) : ControllerBase
{
    [HttpPost] public async Task<ActionResult<TrainingRequestResponse>> Create(CreateTrainingRequest r, CancellationToken ct) { if (user.UserId is not int id) return Unauthorized(); var x = await requests.CreateAsync(id, r, ct); return x.Value is null ? BadRequest(new { error = x.Error }) : CreatedAtAction(nameof(Get), new { requestId = x.Value.RequestId }, x.Value); }
    [HttpGet] public async Task<ActionResult<IReadOnlyCollection<TrainingRequestResponse>>> List(CancellationToken ct) => user.UserId is int id ? Ok(await requests.ListForAthleteAsync(id, ct)) : Unauthorized();
    [HttpGet("{requestId:int}")] public async Task<ActionResult<TrainingRequestResponse>> Get(int requestId, CancellationToken ct) { if (user.UserId is not int id) return Unauthorized(); var x = await requests.GetForAthleteAsync(id, requestId, ct); return x is null ? NotFound() : Ok(x); }
    [HttpPost("{requestId:int}/cancel")] public async Task<ActionResult<TrainingRequestResponse>> Cancel(int requestId, CancellationToken ct) { if (user.UserId is not int id) return Unauthorized(); var x = await requests.CancelAsync(id, requestId, ct); return x is null ? NotFound() : Ok(x); }
}

[ApiController, Authorize(Roles = ApplicationRoles.Parent), Route("api/parent/training-requests")]
public sealed class ParentTrainingRequestsController(ICurrentUser user, ITrainingRequestService requests) : ControllerBase
{
    [HttpGet] public async Task<ActionResult<IReadOnlyCollection<TrainingRequestResponse>>> List(CancellationToken ct) => user.UserId is int id ? Ok(await requests.InboxAsync(id, ApplicationRoles.Parent, ct)) : Unauthorized();
    [HttpGet("{requestId:int}")] public async Task<ActionResult<TrainingRequestResponse>> Get(int requestId, CancellationToken ct) { if (user.UserId is not int id) return Unauthorized(); var x = await requests.InboxItemAsync(id, ApplicationRoles.Parent, requestId, ct); return x is null ? NotFound() : Ok(x); }
    [HttpPost("{requestId:int}/approve")] public async Task<ActionResult<TrainingRequestResponse>> Approve(int requestId, ApproveTrainingRequest r, CancellationToken ct) { if (user.UserId is not int id) return Unauthorized(); var x = await requests.ApproveAsync(id, ApplicationRoles.Parent, requestId, r, ct); return x.Value is not null ? Ok(x.Value) : x.Error is null ? NotFound() : Conflict(new { error = x.Error }); }
    [HttpPost("{requestId:int}/decline")] public async Task<ActionResult<TrainingRequestResponse>> Decline(int requestId, CancellationToken ct) { if (user.UserId is not int id) return Unauthorized(); var x = await requests.DeclineAsync(id, ApplicationRoles.Parent, requestId, ct); return x is null ? NotFound() : Ok(x); }
}

[ApiController, Authorize(Roles = ApplicationRoles.Coach), Route("api/coach/training-requests")]
public sealed class CoachTrainingRequestsController(ICurrentUser user, ITrainingRequestService requests) : ControllerBase
{
    [HttpGet] public async Task<ActionResult<IReadOnlyCollection<TrainingRequestResponse>>> List(CancellationToken ct) => user.UserId is int id ? Ok(await requests.InboxAsync(id, ApplicationRoles.Coach, ct)) : Unauthorized();
    [HttpGet("{requestId:int}")] public async Task<ActionResult<TrainingRequestResponse>> Get(int requestId, CancellationToken ct) { if (user.UserId is not int id) return Unauthorized(); var x = await requests.InboxItemAsync(id, ApplicationRoles.Coach, requestId, ct); return x is null ? NotFound() : Ok(x); }
    [HttpPost("{requestId:int}/approve")] public async Task<ActionResult<TrainingRequestResponse>> Approve(int requestId, ApproveTrainingRequest r, CancellationToken ct) { if (user.UserId is not int id) return Unauthorized(); var x = await requests.ApproveAsync(id, ApplicationRoles.Coach, requestId, r, ct); return x.Value is not null ? Ok(x.Value) : x.Error is null ? NotFound() : Conflict(new { error = x.Error }); }
    [HttpPost("{requestId:int}/decline")] public async Task<ActionResult<TrainingRequestResponse>> Decline(int requestId, CancellationToken ct) { if (user.UserId is not int id) return Unauthorized(); var x = await requests.DeclineAsync(id, ApplicationRoles.Coach, requestId, ct); return x is null ? NotFound() : Ok(x); }
}
