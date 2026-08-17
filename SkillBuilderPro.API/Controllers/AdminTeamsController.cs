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
[Route("api/admin/teams")]
public sealed class AdminTeamsController : ControllerBase
{
    private readonly IAdminRelationshipService _service;
    private readonly ICurrentUser _currentUser;

    public AdminTeamsController(IAdminRelationshipService service, ICurrentUser currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<TeamResponse>>> ListTeams(
        CancellationToken cancellationToken) =>
        Ok(await _service.ListTeamsAsync(cancellationToken));

    [HttpGet("{teamId:int}")]
    public async Task<ActionResult<TeamResponse>> GetTeam(
        [Range(1, int.MaxValue)] int teamId,
        CancellationToken cancellationToken) =>
        ToActionResult(await _service.GetTeamAsync(teamId, cancellationToken));

    [HttpPost]
    public async Task<ActionResult<TeamResponse>> CreateTeam(
        CreateTeamRequest request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not int administratorUserId)
        {
            return Unauthorized();
        }

        return ToActionResult(await _service.CreateTeamAsync(
            request,
            administratorUserId,
            cancellationToken));
    }

    [HttpPut("{teamId:int}")]
    public async Task<ActionResult<TeamResponse>> UpdateTeam(
        [Range(1, int.MaxValue)] int teamId,
        UpdateTeamRequest request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not int administratorUserId)
        {
            return Unauthorized();
        }

        return ToActionResult(await _service.UpdateTeamAsync(
            teamId,
            request,
            administratorUserId,
            cancellationToken));
    }

    [HttpPost("{teamId:int}/deactivate")]
    public Task<ActionResult<TeamResponse>> DeactivateTeam(
        [Range(1, int.MaxValue)] int teamId,
        CancellationToken cancellationToken) =>
        SetTeamActive(teamId, false, cancellationToken);

    [HttpPost("{teamId:int}/reactivate")]
    public Task<ActionResult<TeamResponse>> ReactivateTeam(
        [Range(1, int.MaxValue)] int teamId,
        CancellationToken cancellationToken) =>
        SetTeamActive(teamId, true, cancellationToken);

    [HttpGet("{teamId:int}/coaches")]
    public async Task<ActionResult<IReadOnlyCollection<TeamCoachResponse>>> ListTeamCoaches(
        [Range(1, int.MaxValue)] int teamId,
        CancellationToken cancellationToken) =>
        ToActionResult(await _service.ListTeamCoachesAsync(teamId, cancellationToken));

    [HttpPost("{teamId:int}/coaches")]
    public async Task<ActionResult<TeamCoachResponse>> AddTeamCoach(
        [Range(1, int.MaxValue)] int teamId,
        AddTeamCoachRequest request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not int administratorUserId)
        {
            return Unauthorized();
        }

        return ToActionResult(await _service.AddTeamCoachAsync(
            teamId,
            request,
            administratorUserId,
            cancellationToken));
    }

    [HttpPut("{teamId:int}/coaches/{coachUserId:int}")]
    public async Task<ActionResult<TeamCoachResponse>> UpdateTeamCoachRole(
        [Range(1, int.MaxValue)] int teamId,
        [Range(1, int.MaxValue)] int coachUserId,
        UpdateTeamCoachRequest request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not int administratorUserId)
        {
            return Unauthorized();
        }

        return ToActionResult(await _service.UpdateTeamCoachRoleAsync(
            teamId,
            coachUserId,
            request,
            administratorUserId,
            cancellationToken));
    }

    [HttpPost("{teamId:int}/coaches/{coachUserId:int}/deactivate")]
    public Task<ActionResult<TeamCoachResponse>> DeactivateTeamCoach(
        [Range(1, int.MaxValue)] int teamId,
        [Range(1, int.MaxValue)] int coachUserId,
        CancellationToken cancellationToken) =>
        SetTeamCoachActive(teamId, coachUserId, false, cancellationToken);

    [HttpPost("{teamId:int}/coaches/{coachUserId:int}/reactivate")]
    public Task<ActionResult<TeamCoachResponse>> ReactivateTeamCoach(
        [Range(1, int.MaxValue)] int teamId,
        [Range(1, int.MaxValue)] int coachUserId,
        CancellationToken cancellationToken) =>
        SetTeamCoachActive(teamId, coachUserId, true, cancellationToken);

    [HttpGet("{teamId:int}/athletes")]
    public async Task<ActionResult<IReadOnlyCollection<TeamAthleteResponse>>> ListTeamAthletes(
        [Range(1, int.MaxValue)] int teamId,
        CancellationToken cancellationToken) =>
        ToActionResult(await _service.ListTeamAthletesAsync(teamId, cancellationToken));

    [HttpPost("{teamId:int}/athletes")]
    public async Task<ActionResult<TeamAthleteResponse>> AddTeamAthlete(
        [Range(1, int.MaxValue)] int teamId,
        AddTeamAthleteRequest request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not int administratorUserId)
        {
            return Unauthorized();
        }

        return ToActionResult(await _service.AddTeamAthleteAsync(
            teamId,
            request,
            administratorUserId,
            cancellationToken));
    }

    [HttpPost("{teamId:int}/athletes/{athleteUserId:int}/deactivate")]
    public Task<ActionResult<TeamAthleteResponse>> DeactivateTeamAthlete(
        [Range(1, int.MaxValue)] int teamId,
        [Range(1, int.MaxValue)] int athleteUserId,
        CancellationToken cancellationToken) =>
        SetTeamAthleteActive(teamId, athleteUserId, false, cancellationToken);

    [HttpPost("{teamId:int}/athletes/{athleteUserId:int}/reactivate")]
    public Task<ActionResult<TeamAthleteResponse>> ReactivateTeamAthlete(
        [Range(1, int.MaxValue)] int teamId,
        [Range(1, int.MaxValue)] int athleteUserId,
        CancellationToken cancellationToken) =>
        SetTeamAthleteActive(teamId, athleteUserId, true, cancellationToken);

    private async Task<ActionResult<TeamResponse>> SetTeamActive(
        int teamId,
        bool isActive,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not int administratorUserId)
        {
            return Unauthorized();
        }

        return ToActionResult(await _service.SetTeamActiveAsync(
            teamId,
            isActive,
            administratorUserId,
            cancellationToken));
    }

    private async Task<ActionResult<TeamCoachResponse>> SetTeamCoachActive(
        int teamId,
        int coachUserId,
        bool isActive,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not int administratorUserId)
        {
            return Unauthorized();
        }

        return ToActionResult(await _service.SetTeamCoachActiveAsync(
            teamId,
            coachUserId,
            isActive,
            administratorUserId,
            cancellationToken));
    }

    private async Task<ActionResult<TeamAthleteResponse>> SetTeamAthleteActive(
        int teamId,
        int athleteUserId,
        bool isActive,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not int administratorUserId)
        {
            return Unauthorized();
        }

        return ToActionResult(await _service.SetTeamAthleteActiveAsync(
            teamId,
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
