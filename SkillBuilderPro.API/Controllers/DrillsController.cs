using Microsoft.AspNetCore.Mvc;
using SkillBuilderPro.Core.Interfaces;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DrillsController : ControllerBase
{
    private readonly IDrillService _drillService;

    public DrillsController(IDrillService drillService)
    {
        _drillService = drillService;
    }

    // GET api/drills?sport=Basketball&category=Dribbling
    [HttpGet]
    public async Task<ActionResult<List<Drill>>> GetAll([FromQuery] string? sport, [FromQuery] string? category)
    {
        List<Drill> drills = await _drillService.GetAllAsync(sport, category);
        return Ok(drills);
    }

    // GET api/drills/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Drill>> GetById(int id)
    {
        Drill? drill = await _drillService.GetByIdAsync(id);
        return drill is null ? NotFound($"Drill {id} not found.") : Ok(drill);
    }

    // POST api/drills
    [HttpPost]
    public async Task<ActionResult<Drill>> Create([FromBody] Drill drill)
    {
        Drill created = await _drillService.CreateAsync(drill);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT api/drills/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Drill drill)
    {
        bool updated = await _drillService.UpdateAsync(id, drill);
        return updated ? NoContent() : NotFound($"Drill {id} not found.");
    }

    // DELETE api/drills/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        bool deleted = await _drillService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound($"Drill {id} not found.");
    }
}
