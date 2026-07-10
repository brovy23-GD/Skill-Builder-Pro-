using Microsoft.AspNetCore.Mvc;
using SkillBuilderPro.Core.Interfaces;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SchedulesController : ControllerBase
{
    private readonly IScheduleService _scheduleService;

    public SchedulesController(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    // GET api/schedules?completed=false
    [HttpGet]
    public async Task<ActionResult<List<TrainingSchedule>>> GetAll([FromQuery] bool? completed)
    {
        List<TrainingSchedule> schedules = await _scheduleService.GetAllAsync(completed);
        return Ok(schedules);
    }

    // GET api/schedules/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TrainingSchedule>> GetById(int id)
    {
        TrainingSchedule? schedule = await _scheduleService.GetByIdAsync(id);
        return schedule is null ? NotFound($"Schedule {id} not found.") : Ok(schedule);
    }

    // POST api/schedules
    [HttpPost]
    public async Task<ActionResult<TrainingSchedule>> Create([FromBody] TrainingSchedule schedule)
    {
        TrainingSchedule? created = await _scheduleService.CreateAsync(schedule);
        return created is null
            ? BadRequest($"Drill {schedule.DrillId} does not exist.")
            : CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT api/schedules/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] TrainingSchedule schedule)
    {
        bool updated = await _scheduleService.UpdateAsync(id, schedule);
        return updated ? NoContent() : NotFound($"Schedule {id} not found.");
    }

    // PATCH api/schedules/1/complete
    [HttpPatch("{id:int}/complete")]
    public async Task<IActionResult> MarkComplete(int id)
    {
        bool completed = await _scheduleService.MarkCompleteAsync(id);
        return completed ? NoContent() : NotFound($"Schedule {id} not found.");
    }

    // DELETE api/schedules/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        bool deleted = await _scheduleService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound($"Schedule {id} not found.");
    }
}
