using Microsoft.EntityFrameworkCore;
using SkillBuilderPro.Core.Data;
using SkillBuilderPro.Core.Interfaces;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.API.Services;

public class ProgressService : IProgressService
{
    private readonly AppDbContext _context;

    public ProgressService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProgressLog>> GetAllAsync(
        int? drillId,
        int? ownerUserId)
    {
        IQueryable<ProgressLog> query = _context.ProgressLogs.AsNoTracking();

        if (ownerUserId.HasValue)
        {
            query = query.Where(log => log.OwnerUserId == ownerUserId.Value);
        }

        if (drillId.HasValue)
        {
            query = query.Where(log => log.DrillId == drillId.Value);
        }

        return await query.OrderBy(log => log.Id).ToListAsync();
    }

    public async Task<ProgressLog?> GetByIdAsync(int id, int? ownerUserId)
    {
        IQueryable<ProgressLog> query = _context.ProgressLogs.AsNoTracking();

        if (ownerUserId.HasValue)
        {
            query = query.Where(log => log.OwnerUserId == ownerUserId.Value);
        }

        return await query.FirstOrDefaultAsync(log => log.Id == id);
    }

    public async Task<ProgressLog?> CreateAsync(ProgressLog log)
    {
        bool drillExists = await _context.Drills.AnyAsync(drill =>
            drill.Id == log.DrillId);
        if (!drillExists)
        {
            return null;
        }

        _context.ProgressLogs.Add(log);
        await _context.SaveChangesAsync();
        return log;
    }

    public async Task<bool> DeleteAsync(int id, int? ownerUserId)
    {
        IQueryable<ProgressLog> query = _context.ProgressLogs;

        if (ownerUserId.HasValue)
        {
            query = query.Where(log => log.OwnerUserId == ownerUserId.Value);
        }

        var log = await query.FirstOrDefaultAsync(log => log.Id == id);
        if (log is null)
        {
            return false;
        }

        _context.ProgressLogs.Remove(log);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<double?> GetAverageRatingAsync(
        int drillId,
        int? ownerUserId)
    {
        IQueryable<ProgressLog> query = _context.ProgressLogs
            .AsNoTracking()
            .Where(log => log.DrillId == drillId);

        if (ownerUserId.HasValue)
        {
            query = query.Where(log => log.OwnerUserId == ownerUserId.Value);
        }

        return await query.AverageAsync(log => (double?)log.Rating);
    }
}
