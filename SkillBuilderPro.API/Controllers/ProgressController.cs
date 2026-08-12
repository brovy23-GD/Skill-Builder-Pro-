using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillBuilderPro.API.Contracts.Progress;
using SkillBuilderPro.API.Security;
using SkillBuilderPro.Core.Interfaces;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ProgressController : ControllerBase
{
    private readonly IProgressService _progressService;
    private readonly ICurrentUser _currentUser;

    public ProgressController(
        IProgressService progressService,
        ICurrentUser currentUser)
    {
        _progressService = progressService;
        _currentUser = currentUser;
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
}
