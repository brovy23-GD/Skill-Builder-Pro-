using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillBuilderPro.API.Contracts.Access;
using SkillBuilderPro.API.Contracts.Assignments;
using SkillBuilderPro.API.Contracts.Progression;
using SkillBuilderPro.API.Security;
using SkillBuilderPro.API.Services;
using SkillBuilderPro.Core.Identity;
using SkillBuilderPro.Core.Interfaces;

namespace SkillBuilderPro.API.Controllers;

[ApiController, Authorize(Roles = ApplicationRoles.Parent), Route("api/parent")]
public sealed class ParentController : ControllerBase
{
    private readonly IRelationshipDiscoveryService _discovery;
    private readonly ICurrentUser _currentUser;
    private readonly IAssignmentService _assignmentService;
    private readonly IProgressionService _progressionService;
    private readonly IRelationshipAccessService _relationshipAccess;
    private readonly IProgressionMilestoneService _milestoneService;

    public ParentController(
        IRelationshipDiscoveryService discovery,
        ICurrentUser currentUser,
        IAssignmentService assignmentService,
        IProgressionService progressionService,
        IRelationshipAccessService relationshipAccess,
        IProgressionMilestoneService milestoneService) =>
        (_discovery, _currentUser, _assignmentService, _progressionService, _relationshipAccess, _milestoneService) = (discovery, currentUser, assignmentService, progressionService, relationshipAccess, milestoneService);

    [HttpGet("athletes")]
    public async Task<ActionResult<IReadOnlyCollection<AthleteSummaryResponse>>> GetAthletes(CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not int userId) return Unauthorized();
        return Ok(await _discovery.GetParentAthletesAsync(userId, cancellationToken));
    }

    [HttpPost("assignments")]
    public async Task<ActionResult<DrillAssignmentResponse>> CreateAssignment(
        ParentAssignmentRequest request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not int userId) return Unauthorized();
        var result = await _assignmentService.CreateForParentAsync(
            userId,
            new AssignmentCreateCommand(request.DrillId, request.AthleteUserIds, request.ScheduledForUtc, request.DueAtUtc, request.Instructions, request.CountsTowardProgression),
            cancellationToken);
        return AssignmentControllerResult.Created(this, result);
    }

    [HttpGet("assignments")]
    public async Task<ActionResult<IReadOnlyCollection<CreatorAssignmentSummaryResponse>>> GetAssignments(
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not int userId) return Unauthorized();
        var assignments = await _assignmentService.GetCreatedAssignmentsAsync(userId, cancellationToken);
        return Ok(assignments.Select(assignment => assignment.ToResponse()).ToList());
    }

    [HttpGet("assignments/{assignmentId:int}")]
    public async Task<ActionResult<DrillAssignmentResponse>> GetAssignment(
        int assignmentId,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not int userId) return Unauthorized();
        var assignment = await _assignmentService.GetCreatedAssignmentAsync(userId, assignmentId, cancellationToken);
        return assignment is null ? NotFound() : Ok(assignment.ToResponse());
    }

    [HttpPost("assignments/{assignmentId:int}/cancel")]
    public async Task<ActionResult<DrillAssignmentResponse>> CancelAssignment(
        int assignmentId,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not int userId) return Unauthorized();
        var result = await _assignmentService.CancelCreatedAssignmentAsync(userId, assignmentId, cancellationToken);
        return AssignmentControllerResult.Action(this, result);
    }

    [HttpGet("athletes/{athleteUserId:int}/progression")]
    public async Task<ActionResult<AthleteProgressionResponse>> GetProgression(int athleteUserId, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not int userId) return Unauthorized();
        if (!await _relationshipAccess.CanParentAccessAthleteAsync(userId, athleteUserId, cancellationToken)) return NotFound();
        return Ok((await _progressionService.GetAthleteProgressionAsync(athleteUserId, cancellationToken)).ToResponse());
    }

    [HttpGet("athletes/{athleteUserId:int}/progression/skills")]
    public async Task<ActionResult<IReadOnlyCollection<AthleteSkillProgressResponse>>> GetProgressionSkills(int athleteUserId, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not int userId) return Unauthorized();
        if (!await _relationshipAccess.CanParentAccessAthleteAsync(userId, athleteUserId, cancellationToken)) return NotFound();
        var skills = await _progressionService.GetAthleteSkillsAsync(athleteUserId, cancellationToken);
        return Ok(skills.Select(skill => skill.ToResponse()).ToList());
    }

    [HttpGet("athletes/{athleteUserId:int}/trophy-room")]
    public async Task<ActionResult<TrophyRoomResponse>> GetTrophyRoom(int athleteUserId, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not int userId) return Unauthorized();
        if (!await _relationshipAccess.CanParentAccessAthleteAsync(userId, athleteUserId, cancellationToken)) return NotFound();
        return Ok((await _milestoneService.GetTrophyRoomAsync(athleteUserId, cancellationToken)).ToResponse());
    }
}
