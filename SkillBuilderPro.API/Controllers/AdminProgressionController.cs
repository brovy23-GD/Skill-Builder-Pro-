using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillBuilderPro.API.Contracts.Progression;
using SkillBuilderPro.Core.Identity;
using SkillBuilderPro.Core.Interfaces;

namespace SkillBuilderPro.API.Controllers;

[ApiController, Authorize(Roles = ApplicationRoles.Administrator), Route("api/admin/athletes/{athleteUserId:int}/progression")]
public sealed class AdminProgressionController : ControllerBase
{
    private readonly IProgressionService _progression;
    private readonly IRelationshipAccessService _access;
    private readonly IProgressionMilestoneService _milestones;
    public AdminProgressionController(IProgressionService progression, IRelationshipAccessService access, IProgressionMilestoneService milestones) => (_progression, _access, _milestones) = (progression, access, milestones);

    [HttpGet]
    public async Task<ActionResult<AthleteProgressionResponse>> Get(int athleteUserId, CancellationToken cancellationToken)
    {
        if (!await _access.IsUserInRoleAsync(athleteUserId, ApplicationRoles.Athlete, cancellationToken)) return NotFound();
        return Ok((await _progression.GetAthleteProgressionAsync(athleteUserId, cancellationToken)).ToResponse());
    }

    [HttpGet("skills")]
    public async Task<ActionResult<IReadOnlyCollection<AthleteSkillProgressResponse>>> GetSkills(int athleteUserId, CancellationToken cancellationToken)
    {
        if (!await _access.IsUserInRoleAsync(athleteUserId, ApplicationRoles.Athlete, cancellationToken)) return NotFound();
        var skills = await _progression.GetAthleteSkillsAsync(athleteUserId, cancellationToken);
        return Ok(skills.Select(skill => skill.ToResponse()).ToList());
    }

    [HttpGet("trophy-room")]
    public async Task<ActionResult<TrophyRoomResponse>> GetTrophyRoom(int athleteUserId, CancellationToken cancellationToken)
    {
        if (!await _access.IsUserInRoleAsync(athleteUserId, ApplicationRoles.Athlete, cancellationToken)) return NotFound();
        return Ok((await _milestones.GetTrophyRoomAsync(athleteUserId, cancellationToken)).ToResponse());
    }
}
