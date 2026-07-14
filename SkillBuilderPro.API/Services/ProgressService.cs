using Microsoft.EntityFrameworkCore;
using SkillBuilderPro.Core.Models;
using SkillBuilderPro.Core.Interfaces;
using SkillBuilderPro.API.Data;

namespace SkillBuilderPro.API.Services;


public class ProgressService : IProgressService
{
    private readonly AppDbContext _context;

    public ProgressService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProgressLog>> GetAllAsync(int? drillId = null)
    {
        IQueryable<ProgressLog> query = _context.ProgressLogs
            .Include(p => p.Drill)
            .AsNoTracking();

        if (drillId.HasValue)
            query = query.Where(p => p.DrillId == drillId.Value);

        return await query.OrderByDescending(p => p.LogDate).ToListAsync();
    }

    public async Task<ProgressLog?> GetByIdAsync(int id)
    {
        return await _context.ProgressLogs
            .Include(p => p.Drill)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<ProgressLog?> CreateAsync(ProgressLog log)
    {
        bool drillExists = await _context.Drills.AnyAsync(d => d.Id == log.DrillId);
        if (!drillExists) return null;

        _context.ProgressLogs.Add(log);
        await _context.SaveChangesAsync();
        return log;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        ProgressLog? existing = await _context.ProgressLogs.FindAsync(id);
        if (existing is null) return false;

        _context.ProgressLogs.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<double?> GetAverageRatingAsync(int drillId)
    {
        bool hasLogs = await _context.ProgressLogs.AnyAsync(p => p.DrillId == drillId);
        if (!hasLogs) return null;

        return await _context.ProgressLogs
            .Where(p => p.DrillId == drillId)
            .AverageAsync(p => (double)p.Rating);
    }
}
