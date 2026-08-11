using Microsoft.EntityFrameworkCore;
using SkillBuilderPro.Core.Models;
using SkillBuilderPro.Core.Interfaces;
using SkillBuilderPro.Core.Data;

namespace SkillBuilderPro.API.Services;

public class ScheduleService : IScheduleService
{
    private readonly AppDbContext _context;

    public ScheduleService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<TrainingSchedule>> GetAllAsync(bool? completed = null)
    {
        IQueryable<TrainingSchedule> query = _context.Schedules.AsNoTracking();

        if (completed.HasValue)
        {
            string status = completed.Value ? "Completed" : "Pending";
            query = query.Where(s => s.Status == status);
        }

        return await query.OrderBy(s => s.Id).ToListAsync();
    }

    public async Task<TrainingSchedule?> GetByIdAsync(int id)
    {
        return await _context.Schedules
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<TrainingSchedule?> CreateAsync(TrainingSchedule schedule)
    {
        bool drillExists = await _context.Drills.AnyAsync(d => d.Id == schedule.DrillId);
        if (!drillExists) return null;

        _context.Schedules.Add(schedule);
        await _context.SaveChangesAsync();
        return schedule;
    }

    public async Task<bool> UpdateAsync(int id, TrainingSchedule schedule)
    {
        TrainingSchedule? existing = await _context.Schedules.FindAsync(id);
        if (existing is null) return false;

        existing.DrillId = schedule.DrillId;
        existing.Title = schedule.Title;
        existing.Description = schedule.Description;
        existing.Status = schedule.Status;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkCompleteAsync(int id)
    {
        TrainingSchedule? existing = await _context.Schedules.FindAsync(id);
        if (existing is null) return false;

        existing.Status = "Completed";
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        TrainingSchedule? existing = await _context.Schedules.FindAsync(id);
        if (existing is null) return false;

        _context.Schedules.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}