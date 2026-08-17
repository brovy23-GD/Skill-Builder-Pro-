using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillBuilderPro.API.Contracts.Progress;
using SkillBuilderPro.API.Security;
using SkillBuilderPro.Core.Interfaces;
using SkillBuilderPro.Core.Identity;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ProgressController : ControllerBase
{
    private readonly IProgressService _progressService;
    private readonly ICurrentUser _currentUser;
    private readonly IRelationshipAccessService _relationshipAccessService;

    public ProgressController(
        IProgressService progressService,
        ICurrentUser currentUser,
        IRelationshipAccessService relationshipAccessService)
    {
        _progressService = progressService;
        _currentUser = currentUser;
        _relationshipAccessService = relationshipAccessService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProgressLog>>> GetAll(
        [FromQuery] int? drillId)
    {
        if (_currentUser.UserId is not int userId)
        {
            return Unauthorized();
        }

        int? ownerScope = _currentUser.IsAdministrator ? null : userId;
        var logs = await _progressService.GetAllAsync(drillId, ownerScope);
        return Ok(logs);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProgressLog>> GetById(int id)
    {
        if (_currentUser.UserId is not int userId)
        {
            return Unauthorized();
        }

        int? ownerScope = _currentUser.IsAdministrator ? null : userId;
        var log = await _progressService.GetByIdAsync(id, ownerScope);
        return log is null ? NotFound() : Ok(log);
    }

    [HttpGet("average/{drillId:int}")]
    public async Task<ActionResult<double>> GetAverageRating(int drillId)
    {
        if (_currentUser.UserId is not int userId)
        {
            return Unauthorized();
        }

        int? ownerScope = _currentUser.IsAdministrator ? null : userId;
        var average = await _progressService.GetAverageRatingAsync(
            drillId,
            ownerScope);

        return average is null ? NotFound() : Ok(average);
    }

    [HttpGet("athlete/{athleteUserId:int}")]
    [Authorize(Roles = ApplicationRoles.Athlete + "," + ApplicationRoles.Parent + "," + ApplicationRoles.Coach + "," + ApplicationRoles.Administrator)]
    public async Task<ActionResult<List<ProgressLog>>> GetForAthlete(
        int athleteUserId,
        CancellationToken cancellationToken)
    {
        if (!await CanReadAthleteAsync(athleteUserId, cancellationToken))
        {
            return NotFound();
        }

        return Ok(await _progressService.GetAllForAthleteAsync(athleteUserId));
    }

    [HttpGet("athlete/{athleteUserId:int}/{progressId:int}")]
    [Authorize(Roles = ApplicationRoles.Athlete + "," + ApplicationRoles.Parent + "," + ApplicationRoles.Coach + "," + ApplicationRoles.Administrator)]
    public async Task<ActionResult<ProgressLog>> GetForAthleteById(
        int athleteUserId,
        int progressId,
        CancellationToken cancellationToken)
    {
        if (!await CanReadAthleteAsync(athleteUserId, cancellationToken))
        {
            return NotFound();
        }

        var log = await _progressService.GetByIdForAthleteAsync(athleteUserId, progressId);
        return log is null ? NotFound() : Ok(log);
    }

    [HttpGet("athlete/{athleteUserId:int}/average/{drillId:int}")]
    [Authorize(Roles = ApplicationRoles.Athlete + "," + ApplicationRoles.Parent + "," + ApplicationRoles.Coach + "," + ApplicationRoles.Administrator)]
    public async Task<ActionResult<double>> GetAverageForAthlete(
        int athleteUserId,
        int drillId,
        CancellationToken cancellationToken)
    {
        if (!await CanReadAthleteAsync(athleteUserId, cancellationToken))
        {
            return NotFound();
        }

        var average = await _progressService.GetAverageRatingForAthleteAsync(
            athleteUserId,
            drillId);
        return average is null ? NotFound() : Ok(average);
    }

    [HttpPost]
    public async Task<ActionResult<ProgressLog>> Create(
        ProgressLogRequest request)
    {
        if (_currentUser.UserId is not int userId)
        {
            return Unauthorized();
        }

        var log = new ProgressLog
        {
            DrillId = request.DrillId,
            LogDate = request.LogDate ?? DateTime.UtcNow,
            Rating = request.Rating,
            Notes = request.Notes,
            OwnerUserId = userId
        };

        var created = await _progressService.CreateAsync(log);
        return created is null
            ? BadRequest($"Drill {request.DrillId} does not exist.")
            : CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (_currentUser.UserId is not int userId)
        {
            return Unauthorized();
        }

        int? ownerScope = _currentUser.IsAdministrator ? null : userId;
        bool deleted = await _progressService.DeleteAsync(id, ownerScope);
        return deleted ? NoContent() : NotFound();
    }

    private async Task<bool> CanReadAthleteAsync(
        int athleteUserId,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not int actorUserId
            || !await _relationshipAccessService.IsUserInRoleAsync(
                athleteUserId,
                ApplicationRoles.Athlete,
                cancellationToken))
        {
            return false;
        }

        var scope = await _relationshipAccessService.GetAccessibleAthleteIdsAsync(
            actorUserId,
            cancellationToken);
        return scope.CanAccessAthlete(athleteUserId);
    }
}
