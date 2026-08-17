using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillBuilderPro.API.Contracts.Progression;
using SkillBuilderPro.API.Security;
using SkillBuilderPro.Core.Identity;
using SkillBuilderPro.Core.Interfaces;

namespace SkillBuilderPro.API.Controllers;

[ApiController, Authorize(Roles = ApplicationRoles.Athlete), Route("api/athlete/progression")]
public sealed class AthleteProgressionController : ControllerBase
{
    private readonly IProgressionService _progression;
    private readonly ICurrentUser _currentUser;
    public AthleteProgressionController(IProgressionService progression, ICurrentUser currentUser) => (_progression, _currentUser) = (progression, currentUser);

    [HttpGet]
    public async Task<ActionResult<AthleteProgressionResponse>> Get(CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not int athleteUserId) return Unauthorized();
        return Ok((await _progression.GetAthleteProgressionAsync(athleteUserId, cancellationToken)).ToResponse());
    }

    [HttpGet("skills")]
    public async Task<ActionResult<IReadOnlyCollection<AthleteSkillProgressResponse>>> GetSkills(CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not int athleteUserId) return Unauthorized();
        var skills = await _progression.GetAthleteSkillsAsync(athleteUserId, cancellationToken);
        return Ok(skills.Select(skill => skill.ToResponse()).ToList());
    }
}
