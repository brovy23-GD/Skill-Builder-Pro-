using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillBuilderPro.API.Contracts.Admin;
using SkillBuilderPro.API.Security;
using SkillBuilderPro.Core.Data;
using SkillBuilderPro.Core.Identity;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.API.Controllers;

[ApiController]
[Authorize(Roles = ApplicationRoles.Administrator)]
[Route("api/admin/operations")]
public sealed class AdminOperationsController(AppDbContext db, UserManager<ApplicationUser> users, ICurrentUser currentUser) : ControllerBase
{
    private const int MaximumPageSize = 100;

    [HttpGet("users")]
    public async Task<IActionResult> Users([FromQuery] int page = 1, [FromQuery] int pageSize = 25, [FromQuery] string? search = null, [FromQuery] string? role = null, [FromQuery] bool? isActive = null)
    {
        (page, pageSize) = Page(page, pageSize);
        var query = db.Users.AsNoTracking().Include(x => x.Profile).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(x => x.Email!.Contains(search) || (x.Profile != null && x.Profile.FullName.Contains(search)));
        if (isActive.HasValue) query = query.Where(x => x.Profile != null && x.Profile.IsActive == isActive.Value);
        if (!string.IsNullOrWhiteSpace(role))
        {
            var roleId = await db.Roles.Where(x => x.Name == role).Select(x => (int?)x.Id).SingleOrDefaultAsync();
            if (roleId is null) return Ok(new { page, pageSize, total = 0, items = Array.Empty<object>() });
            query = query.Where(x => db.UserRoles.Any(ur => ur.UserId == x.Id && ur.RoleId == roleId));
        }
        int total = await query.CountAsync();
        var items = await query.OrderBy(x => x.Id).Skip((page - 1) * pageSize).Take(pageSize)
            .Select(x => new { x.Id, x.Email, FullName = x.Profile == null ? "" : x.Profile.FullName, IsActive = x.Profile != null && x.Profile.IsActive, x.Profile!.Sport, x.Profile.DateCreated }).ToListAsync();
        return Ok(new { page, pageSize, total, items });
    }

    [HttpGet("users/{id:int}")]
    public async Task<IActionResult> UserDetail(int id)
    {
        var user = await users.Users.AsNoTracking().Include(x => x.Profile).SingleOrDefaultAsync(x => x.Id == id);
        if (user is null) return NotFound();
        return Ok(new { user.Id, user.Email, user.UserName, user.EmailConfirmed, Profile = user.Profile, Roles = await users.GetRolesAsync(user) });
    }

    [HttpPut("users/{id:int}/role")]
    public async Task<IActionResult> ChangeRole(int id, AdminRoleChangeRequest request)
    {
        if (!ApplicationRoles.All.Contains(request.Role) || string.IsNullOrWhiteSpace(request.Reason)) return BadRequest("A valid role and reason are required.");
        var user = await users.FindByIdAsync(id.ToString()); if (user is null) return NotFound();
        var before = await users.GetRolesAsync(user);
        var remove = await users.RemoveFromRolesAsync(user, before); if (!remove.Succeeded) return Problem("The role change could not be saved.");
        var add = await users.AddToRoleAsync(user, request.Role); if (!add.Succeeded) { await users.AddToRolesAsync(user, before); return Problem("The role change could not be saved."); }
        await Audit("RoleChanged", "User", id.ToString(), before, new[] { request.Role }, request.Reason);
        return NoContent();
    }

    [HttpPut("users/{id:int}/status")]
    public async Task<IActionResult> ChangeStatus(int id, AdminStatusChangeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Reason)) return BadRequest("A reason is required.");
        var profile = await db.UserProfiles.SingleOrDefaultAsync(x => x.UserId == id); if (profile is null) return NotFound();
        bool before = profile.IsActive; profile.IsActive = request.IsActive;
        await Audit("StatusChanged", "User", id.ToString(), new { IsActive = before }, new { request.IsActive }, request.Reason, save: false);
        await db.SaveChangesAsync(); return NoContent();
    }

    [HttpGet("drills")]
    public async Task<IActionResult> Drills([FromQuery] int page = 1, [FromQuery] int pageSize = 25, [FromQuery] string? search = null, [FromQuery] string? sport = null, [FromQuery] string? category = null, [FromQuery] string? subCategory = null)
    {
        (page, pageSize) = Page(page, pageSize); var query = db.Drills.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(x => x.Name.Contains(search));
        if (!string.IsNullOrWhiteSpace(sport)) query = query.Where(x => x.Sport == sport);
        if (!string.IsNullOrWhiteSpace(category)) query = query.Where(x => x.Category == category);
        if (!string.IsNullOrWhiteSpace(subCategory)) query = query.Where(x => x.SubCategory == subCategory);
        int total = await query.CountAsync(); var items = await query.OrderBy(x => x.Id).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return Ok(new { page, pageSize, total, items });
    }

    [HttpGet("drills/{id:int}")]
    public async Task<IActionResult> DrillDetail(int id) => (await db.Drills.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id)) is { } drill ? Ok(drill) : NotFound();

    [HttpPost("drills")]
    public async Task<IActionResult> CreateDrill(AdminDrillRequest request)
    {
        if (!Valid(request, out string error)) return BadRequest(error);
        var drill = Map(request, new Drill { DateCreated = DateTime.UtcNow }); db.Drills.Add(drill);
        await db.SaveChangesAsync(); await Audit("Created", "Drill", drill.Id.ToString(), null, drill, request.Reason);
        return CreatedAtAction(nameof(DrillDetail), new { id = drill.Id }, drill);
    }

    [HttpPut("drills/{id:int}")]
    public async Task<IActionResult> UpdateDrill(int id, AdminDrillRequest request)
    {
        if (!Valid(request, out string error)) return BadRequest(error);
        var drill = await db.Drills.SingleOrDefaultAsync(x => x.Id == id); if (drill is null) return NotFound();
        var before = Snapshot(drill); Map(request, drill);
        await Audit("Updated", "Drill", id.ToString(), before, Snapshot(drill), request.Reason, save: false); await db.SaveChangesAsync(); return NoContent();
    }

    [HttpGet("audit-logs")]
    public async Task<IActionResult> AuditLogs([FromQuery] int page = 1, [FromQuery] int pageSize = 25)
    {
        (page, pageSize) = Page(page, pageSize); int total = await db.AuditLogs.CountAsync();
        var items = await db.AuditLogs.AsNoTracking().OrderByDescending(x => x.TimestampUtc).Skip((page - 1) * pageSize).Take(pageSize)
            .Select(x => new { x.AuditLogId, x.AdministratorUserId, x.Action, x.ResourceType, x.ResourceId, x.Reason, x.TimestampUtc }).ToListAsync();
        return Ok(new { page, pageSize, total, items });
    }

    [HttpGet("system-health")]
    public async Task<IActionResult> SystemHealth()
    {
        bool database = await db.Database.CanConnectAsync();
        int invalidVideos = await db.Drills.CountAsync(x => x.VideoUrl == null || (!x.VideoUrl.Contains("youtube.com") && !x.VideoUrl.Contains("youtu.be")));
        return Ok(new { Api = "healthy", Database = database ? "healthy" : "unavailable", Authentication = "configured", Drills = new { Total = await db.Drills.CountAsync(), InvalidVideoUrls = invalidVideos }, Notifications = new { Pending = await db.NotificationEvents.CountAsync(x => x.ProcessedAtUtc == null) } });
    }

    [HttpGet("snapshot")]
    public async Task<IActionResult> Snapshot() => Ok(new { TotalUsers = await db.Users.CountAsync(), Athletes = await RoleCount(ApplicationRoles.Athlete), Coaches = await RoleCount(ApplicationRoles.Coach), Parents = await RoleCount(ApplicationRoles.Parent), Administrators = await RoleCount(ApplicationRoles.Administrator), TotalDrills = await db.Drills.CountAsync(), SuspendedUsers = await db.UserProfiles.CountAsync(x => !x.IsActive), RecentAdminActions = await db.AuditLogs.CountAsync(x => x.TimestampUtc >= DateTime.UtcNow.AddDays(-7)) });

    private async Task<int> RoleCount(string role) => await (from ur in db.UserRoles join r in db.Roles on ur.RoleId equals r.Id where r.Name == role select ur.UserId).CountAsync();
    private static (int, int) Page(int page, int pageSize) => (Math.Max(1, page), Math.Clamp(pageSize, 1, MaximumPageSize));
    private static bool Valid(AdminDrillRequest r, out string error) { if (string.IsNullOrWhiteSpace(r.Name) || string.IsNullOrWhiteSpace(r.Sport) || string.IsNullOrWhiteSpace(r.Category) || string.IsNullOrWhiteSpace(r.Reason)) { error = "Name, sport, category, and reason are required."; return false; } if (!Uri.TryCreate(r.VideoUrl, UriKind.Absolute, out var uri) || uri.Scheme != "https" || !(uri.Host.EndsWith("youtube.com") || uri.Host.EndsWith("youtu.be") || uri.Host.EndsWith("youtube-nocookie.com"))) { error = "A valid HTTPS YouTube URL is required."; return false; } error = ""; return true; }
    private static Drill Map(AdminDrillRequest r, Drill d) { d.Name=r.Name.Trim(); d.Sport=r.Sport.Trim(); d.Category=r.Category.Trim(); d.SubCategory=r.SubCategory?.Trim(); d.Description=r.Description?.Trim(); d.Difficulty=r.Difficulty; d.Duration=r.Duration?.Trim(); d.VideoUrl=r.VideoUrl.Trim(); return d; }
    private static object Snapshot(Drill d) => new { d.Id, d.Name, d.Sport, d.Category, d.SubCategory, d.Description, d.Difficulty, d.Duration, d.VideoUrl };
    private async Task Audit(string action, string type, string id, object? before, object? after, string reason, bool save = true) { db.AuditLogs.Add(new AuditLog { AdministratorUserId = currentUser.UserId!.Value, Action = action, ResourceType = type, ResourceId = id, BeforeData = before is null ? null : JsonSerializer.Serialize(before), AfterData = after is null ? null : JsonSerializer.Serialize(after), Reason = reason.Trim(), TimestampUtc = DateTime.UtcNow }); if (save) await db.SaveChangesAsync(); }
}
