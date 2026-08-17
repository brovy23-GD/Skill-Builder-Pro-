using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClosedXML.Excel;
using System.Globalization;
using SkillBuilderPro.Core.Identity;
using SkillBuilderPro.Core.Interfaces;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class DrillsController : ControllerBase
    {
        private readonly IDrillService _drillService;
        private readonly IWebHostEnvironment _environment;

        public DrillsController(IDrillService drillService, IWebHostEnvironment environment)
        {
            _drillService = drillService;
            _environment = environment;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Drill>>> GetDrills([FromQuery] string? sport = null, [FromQuery] string? category = null)
        {
            var drills = await _drillService.GetAllAsync(sport, category);
            return Ok(drills);
        }

        [HttpGet("demo")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Drill>> GetDemoDrills()
        {
            string workbookPath = Path.Combine(_environment.ContentRootPath, "Resources", "SkillBuilderPro_240_YouTube_Drill_Links.xlsx");
            if (!System.IO.File.Exists(workbookPath)) return Ok(Array.Empty<Drill>());

            using var workbook = new XLWorkbook(workbookPath);
            var sportCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var drills = new List<Drill>();
            foreach (var row in workbook.Worksheet("Drill Library").RowsUsed().Skip(1))
            {
                string sport = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(row.Cell(2).GetString().Trim().ToLowerInvariant());
                string videoUrl = row.Cell(7).GetString().Trim();
                if (string.IsNullOrWhiteSpace(sport) || !IsValidYouTubeUrl(videoUrl) || sportCounts.GetValueOrDefault(sport) >= 3) continue;
                int sourceId = row.Cell(1).GetValue<int>();
                drills.Add(new Drill { Id = 100000 + sourceId, Sport = sport, Category = row.Cell(3).GetString().Trim(), SubCategory = row.Cell(4).GetString().Trim(), Difficulty = row.Cell(5).GetValue<int>(), Name = row.Cell(6).GetString().Trim(), VideoUrl = videoUrl, Description = row.Cell(8).GetString().Trim(), Duration = "10:00", DateCreated = DateTime.UtcNow });
                sportCounts[sport] = sportCounts.GetValueOrDefault(sport) + 1;
            }
            return Ok(drills);
        }

        private static bool IsValidYouTubeUrl(string value)
        {
            if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps) return false;
            string host = uri.Host.ToLowerInvariant();
            return host.EndsWith("youtube.com", StringComparison.Ordinal) || host.EndsWith("youtu.be", StringComparison.Ordinal) || host.EndsWith("youtube-nocookie.com", StringComparison.Ordinal);
        }

        [HttpGet("range/{startId}/{endId}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Drill>>> GetDrillRange(int startId, int endId)
        {
            var drills = await _drillService.GetDrillRangeAsync(startId, endId);
            return Ok(drills);
        }

        [HttpPost]
        [Authorize(Roles = ApplicationRoles.Coach + "," + ApplicationRoles.Administrator)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<Drill>> CreateDrill([FromBody] Drill drill)
        {
            var createdDrill = await _drillService.CreateAsync(drill);
            return CreatedAtAction(nameof(GetDrills), new { id = createdDrill.Id }, createdDrill);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = ApplicationRoles.Coach + "," + ApplicationRoles.Administrator)]
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
        [Authorize(Roles = ApplicationRoles.Coach + "," + ApplicationRoles.Administrator)]
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
