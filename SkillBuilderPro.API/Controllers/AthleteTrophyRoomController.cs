using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillBuilderPro.API.Contracts.Progression;
using SkillBuilderPro.API.Security;
using SkillBuilderPro.Core.Identity;
using SkillBuilderPro.Core.Interfaces;

namespace SkillBuilderPro.API.Controllers;

[ApiController, Authorize(Roles = ApplicationRoles.Athlete), Route("api/athlete/trophy-room")]
public sealed class AthleteTrophyRoomController : ControllerBase
{
    private readonly IProgressionMilestoneService _service; private readonly ICurrentUser _currentUser;
    public AthleteTrophyRoomController(IProgressionMilestoneService service, ICurrentUser currentUser) => (_service, _currentUser) = (service, currentUser);
    [HttpGet] public async Task<ActionResult<TrophyRoomResponse>> Get(CancellationToken token)
    {
        if (_currentUser.UserId is not int id) return Unauthorized();
        return Ok((await _service.GetTrophyRoomAsync(id, token)).ToResponse());
    }
}
