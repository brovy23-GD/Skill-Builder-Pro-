using Microsoft.AspNetCore.Mvc;
using SkillBuilderPro.API.Services;
using SkillBuilderPro.Core.Models;
using SkillBuilderPro.Core.Interfaces;

namespace SkillBuilderPro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DrillsController : ControllerBase
    {
        private readonly IDrillService _drillService;

        public DrillsController(IDrillService drillService)
        {
            _drillService = drillService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Drill>>> GetDrills([FromQuery] string? sport = null, [FromQuery] string? category = null)
        {
            var drills = await _drillService.GetAllAsync(sport, category);
            return Ok(drills);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Drill>> GetDrill(int id)
        {
            var drill = await _drillService.GetByIdAsync(id);
            if (drill == null)
                return NotFound();

            return Ok(drill);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<Drill>> CreateDrill([FromBody] Drill drill)
        {
            var createdDrill = await _drillService.CreateAsync(drill);
            return CreatedAtAction(nameof(GetDrill), new { id = createdDrill.Id }, createdDrill);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateDrill(int id, [FromBody] Drill drill)
        {
            var existing = await _drillService.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            drill.Id = id;
            await _drillService.UpdateAsync(id, drill);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDrill(int id)
        {
            var drill = await _drillService.GetByIdAsync(id);
            if (drill == null)
                return NotFound();

            await _drillService.DeleteAsync(id);
            return NoContent();
        }
    }
}