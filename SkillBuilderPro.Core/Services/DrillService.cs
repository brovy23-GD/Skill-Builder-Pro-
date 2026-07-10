using Microsoft.EntityFrameworkCore;
using SkillBuilderPro.Core.Data;
using SkillBuilderPro.Core.Interfaces;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.Core.Services;

public class DrillService : IDrillService
{
    private readonly AppDbContext _context;

    public DrillService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Drill>> GetAllAsync(string? sport = null, string? category = null)
    {
        IQueryable<Drill> query = _context.Drills.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(sport))
            query = query.Where(d => d.Sport.ToLower() == sport.ToLower());

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(d => d.Category.ToLower() == category.ToLower());

        return await query.OrderBy(d => d.Name).ToListAsync();
    }

    public async Task<Drill?> GetByIdAsync(int id)
    {
        return await _context.Drills
            .Include(d => d.Schedules)
            .Include(d => d.ProgressLogs)
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<Drill> CreateAsync(Drill drill)
    {
        _context.Drills.Add(drill);
        await _context.SaveChangesAsync();
        return drill;
    }

    public async Task<bool> UpdateAsync(int id, Drill drill)
    {
        Drill? existing = await _context.Drills.FindAsync(id);
        if (existing is null) return false;

        existing.Name = drill.Name;
        existing.Sport = drill.Sport;
        existing.Category = drill.Category;
        existing.Description = drill.Description;
        existing.VideoUrl = drill.VideoUrl;
        existing.DifficultyLevel = drill.DifficultyLevel;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        Drill? existing = await _context.Drills.FindAsync(id);
        if (existing is null) return false;

        _context.Drills.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}
