using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillBuilderPro.API.Contracts.Admin;
using SkillBuilderPro.API.Security;
using SkillBuilderPro.API.Services;
using SkillBuilderPro.Core.Identity;

namespace SkillBuilderPro.API.Controllers;

[ApiController]
[Authorize(Roles = ApplicationRoles.Administrator)]
[Route("api/admin/relationships")]
public sealed class AdminRelationshipsController : ControllerBase
{
    private readonly IAdminRelationshipService _service;
    private readonly ICurrentUser _currentUser;

    public AdminRelationshipsController(
        IAdminRelationshipService service,
        ICurrentUser currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpGet("parent-athletes")]
    public async Task<ActionResult<IReadOnlyCollection<ParentAthleteResponse>>> ListParentAthletes(
        CancellationToken cancellationToken) =>
        Ok(await _service.ListParentAthletesAsync(cancellationToken));

    [HttpGet("parent-athletes/{parentUserId:int}/{athleteUserId:int}")]
    public async Task<ActionResult<ParentAthleteResponse>> GetParentAthlete(
        [Range(1, int.MaxValue)] int parentUserId,
        [Range(1, int.MaxValue)] int athleteUserId,
        CancellationToken cancellationToken) =>
        ToActionResult(await _service.GetParentAthleteAsync(
            parentUserId,
            athleteUserId,
            cancellationToken));

    [HttpPost("parent-athletes")]
    public async Task<ActionResult<ParentAthleteResponse>> CreateParentAthlete(
        CreateParentAthleteRequest request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not int administratorUserId)
        {
            return Unauthorized();
        }

        return ToActionResult(await _service.CreateParentAthleteAsync(
            request,
            administratorUserId,
            cancellationToken));
    }

    [HttpPost("parent-athletes/{parentUserId:int}/{athleteUserId:int}/deactivate")]
    public Task<ActionResult<ParentAthleteResponse>> DeactivateParentAthlete(
        [Range(1, int.MaxValue)] int parentUserId,
        [Range(1, int.MaxValue)] int athleteUserId,
        CancellationToken cancellationToken) =>
        SetParentAthleteActive(parentUserId, athleteUserId, false, cancellationToken);

    [HttpPost("parent-athletes/{parentUserId:int}/{athleteUserId:int}/reactivate")]
    public Task<ActionResult<ParentAthleteResponse>> ReactivateParentAthlete(
        [Range(1, int.MaxValue)] int parentUserId,
        [Range(1, int.MaxValue)] int athleteUserId,
        CancellationToken cancellationToken) =>
        SetParentAthleteActive(parentUserId, athleteUserId, true, cancellationToken);

    private async Task<ActionResult<ParentAthleteResponse>> SetParentAthleteActive(
        int parentUserId,
        int athleteUserId,
        bool isActive,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not int administratorUserId)
        {
            return Unauthorized();
        }

        return ToActionResult(await _service.SetParentAthleteActiveAsync(
            parentUserId,
            athleteUserId,
            isActive,
            administratorUserId,
            cancellationToken));
    }

    private ActionResult<T> ToActionResult<T>(AdminServiceResult<T> result) =>
        result.Status switch
        {
            AdminServiceStatus.Success => Ok(result.Value),
            AdminServiceStatus.Created => StatusCode(StatusCodes.Status201Created, result.Value),
            AdminServiceStatus.ValidationError => BadRequest(new { Message = result.Error }),
            AdminServiceStatus.NotFound => NotFound(new { Message = result.Error }),
            AdminServiceStatus.Conflict => Conflict(new { Message = result.Error }),
            AdminServiceStatus.Forbidden => StatusCode(
                StatusCodes.Status403Forbidden,
                new { Message = result.Error }),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
}
