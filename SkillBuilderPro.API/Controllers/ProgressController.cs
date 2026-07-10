using Microsoft.AspNetCore.Mvc;
using SkillBuilderPro.Core.Interfaces;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProgressController : ControllerBase
{
    private readonly IProgressService _progressService;

    public ProgressController(IProgressService progressService)
    {
        _progressService = progressService;
    }

    // GET api/progress?drillId=1
    [HttpGet]
    public async Task<ActionResult<List<ProgressLog>>> GetAll([FromQuery] int? drillId)
    {
        List<ProgressLog> logs = await _progressService.GetAllAsync(drillId);
        return Ok(logs);
    }

    // GET api/progress/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProgressLog>> GetById(int id)
    {
        ProgressLog? log = await _progressService.GetByIdAsync(id);
        return log is null ? NotFound($"Progress log {id} not found.") : Ok(log);
    }

    // GET api/progress/average/1
    [HttpGet("average/{drillId:int}")]
    public async Task<ActionResult<double>> GetAverageRating(int drillId)
    {
        double? average = await _progressService.GetAverageRatingAsync(drillId);
        return average is null ? NotFound($"No progress logs for drill {drillId}.") : Ok(average);
    }

    // POST api/progress
    [HttpPost]
    public async Task<ActionResult<ProgressLog>> Create([FromBody] ProgressLog log)
    {
        ProgressLog? created = await _progressService.CreateAsync(log);
        return created is null
            ? BadRequest($"Drill {log.DrillId} does not exist.")
            : CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // DELETE api/progress/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        bool deleted = await _progressService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound($"Progress log {id} not found.");
    }
}
