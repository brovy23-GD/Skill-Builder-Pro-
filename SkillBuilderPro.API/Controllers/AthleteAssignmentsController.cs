using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillBuilderPro.API.Contracts.Assignments;
using SkillBuilderPro.API.Security;
using SkillBuilderPro.Core.Identity;
using SkillBuilderPro.Core.Interfaces;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.API.Controllers;

[ApiController]
[Authorize(Roles = ApplicationRoles.Athlete)]
[Route("api/athlete/assignments")]
public sealed class AthleteAssignmentsController : ControllerBase
{
    private readonly IAssignmentService _assignmentService;
    private readonly ICurrentUser _currentUser;

    public AthleteAssignmentsController(IAssignmentService assignmentService, ICurrentUser currentUser) =>
        (_assignmentService, _currentUser) = (assignmentService, currentUser);

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<AthleteAssignmentResponse>>> GetAll(
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not int athleteUserId) return Unauthorized();
        if (status is not null && !DrillAssignmentRecipientStatuses.All.Contains(status))
            return BadRequest(new { error = "Unknown recipient status." });
        var assignments = await _assignmentService.GetForAthleteAsync(athleteUserId, status, cancellationToken);
        return Ok(assignments.Select(assignment => assignment.ToResponse()).ToList());
    }

    [HttpGet("{assignmentId:int}")]
    public async Task<ActionResult<AthleteAssignmentResponse>> GetById(int assignmentId, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not int athleteUserId) return Unauthorized();
        var assignment = await _assignmentService.GetForAthleteAsync(athleteUserId, assignmentId, cancellationToken);
        return assignment is null ? NotFound() : Ok(assignment.ToResponse());
    }

    [HttpPost("{assignmentId:int}/start")]
    public async Task<ActionResult<AthleteAssignmentResponse>> Start(int assignmentId, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not int athleteUserId) return Unauthorized();
        var result = await _assignmentService.StartAsync(athleteUserId, assignmentId, cancellationToken);
        return AssignmentControllerResult.Action(this, result);
    }

    [HttpPost("{assignmentId:int}/complete")]
    public async Task<ActionResult<AthleteAssignmentResponse>> Complete(
        int assignmentId,
        CompleteAssignmentRequest request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not int athleteUserId) return Unauthorized();
        var result = await _assignmentService.CompleteAsync(athleteUserId, assignmentId, request.AthleteNotes, request.Rating, cancellationToken);
        return AssignmentControllerResult.Action(this, result);
    }
}

internal static class AssignmentControllerResult
{
    public static ActionResult<DrillAssignmentResponse> Created(
        ControllerBase controller,
        AssignmentOperationResult<DrillAssignmentView> result) => result.Status switch
    {
        AssignmentOperationStatus.Created => controller.StatusCode(StatusCodes.Status201Created, result.Value!.ToResponse()),
        AssignmentOperationStatus.ValidationError => controller.BadRequest(new { error = result.Error }),
        AssignmentOperationStatus.NotFound => controller.NotFound(),
        AssignmentOperationStatus.Conflict => controller.Conflict(new { error = result.Error }),
        _ => controller.StatusCode(StatusCodes.Status500InternalServerError)
    };

    public static ActionResult<AthleteAssignmentResponse> Action(
        ControllerBase controller,
        AssignmentOperationResult<AthleteAssignmentView> result) => result.Status switch
    {
        AssignmentOperationStatus.Success => controller.Ok(result.Value!.ToResponse()),
        AssignmentOperationStatus.ValidationError => controller.BadRequest(new { error = result.Error }),
        AssignmentOperationStatus.NotFound => controller.NotFound(),
        AssignmentOperationStatus.Conflict => controller.Conflict(new { error = result.Error }),
        _ => controller.StatusCode(StatusCodes.Status500InternalServerError)
    };

    public static ActionResult<DrillAssignmentResponse> Action(
        ControllerBase controller,
        AssignmentOperationResult<DrillAssignmentView> result) => result.Status switch
    {
        AssignmentOperationStatus.Success => controller.Ok(result.Value!.ToResponse()),
        AssignmentOperationStatus.ValidationError => controller.BadRequest(new { error = result.Error }),
        AssignmentOperationStatus.NotFound => controller.NotFound(),
        AssignmentOperationStatus.Conflict => controller.Conflict(new { error = result.Error }),
        _ => controller.StatusCode(StatusCodes.Status500InternalServerError)
    };
}
