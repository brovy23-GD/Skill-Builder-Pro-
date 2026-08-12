using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillBuilderPro.API.Contracts.Schedules;
using SkillBuilderPro.API.Security;
using SkillBuilderPro.Core.Interfaces;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class SchedulesController : ControllerBase
{
    private readonly IScheduleService _scheduleService;
    private readonly ICurrentUser _currentUser;

    public SchedulesController(
        IScheduleService scheduleService,
        ICurrentUser currentUser)
    {
        _scheduleService = scheduleService;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<ActionResult<List<TrainingSchedule>>> GetAll(
        [FromQuery] bool? completed)
    {
        if (_currentUser.UserId is not int userId)
        {
            return Unauthorized();
        }

        int? ownerScope = _currentUser.IsAdministrator ? null : userId;
        var schedules = await _scheduleService.GetAllAsync(
            completed,
            ownerScope);
        return Ok(schedules);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TrainingSchedule>> GetById(int id)
    {
        if (_currentUser.UserId is not int userId)
        {
            return Unauthorized();
        }

        int? ownerScope = _currentUser.IsAdministrator ? null : userId;
        var schedule = await _scheduleService.GetByIdAsync(id, ownerScope);
        return schedule is null ? NotFound() : Ok(schedule);
    }

    [HttpPost]
    public async Task<ActionResult<TrainingSchedule>> Create(
        TrainingScheduleRequest request)
    {
        if (_currentUser.UserId is not int userId)
        {
            return Unauthorized();
        }

        var schedule = CreateSchedule(request, userId);
        var created = await _scheduleService.CreateAsync(schedule);
        return created is null
            ? BadRequest($"Drill {request.DrillId} does not exist.")
            : CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        TrainingScheduleRequest request)
    {
        if (_currentUser.UserId is not int userId)
        {
            return Unauthorized();
        }

        int? ownerScope = _currentUser.IsAdministrator ? null : userId;
        var schedule = CreateSchedule(request, userId);
        bool updated = await _scheduleService.UpdateAsync(
            id,
            schedule,
            ownerScope);
        return updated ? NoContent() : NotFound();
    }

    [HttpPatch("{id:int}/complete")]
    public async Task<IActionResult> MarkComplete(int id)
    {
        if (_currentUser.UserId is not int userId)
        {
            return Unauthorized();
        }

        int? ownerScope = _currentUser.IsAdministrator ? null : userId;
        bool completed = await _scheduleService.MarkCompleteAsync(
            id,
            ownerScope);
        return completed ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (_currentUser.UserId is not int userId)
        {
            return Unauthorized();
        }

        int? ownerScope = _currentUser.IsAdministrator ? null : userId;
        bool deleted = await _scheduleService.DeleteAsync(id, ownerScope);
        return deleted ? NoContent() : NotFound();
    }

    private static TrainingSchedule CreateSchedule(
        TrainingScheduleRequest request,
        int ownerUserId)
    {
        return new TrainingSchedule
        {
            DrillId = request.DrillId,
            Title = request.Title,
            Description = request.Description,
            Status = request.Status,
            OwnerUserId = ownerUserId
        };
    }
}
