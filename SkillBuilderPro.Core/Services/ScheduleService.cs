using Microsoft.EntityFrameworkCore;
using SkillBuilderPro.Core.Data;
using SkillBuilderPro.Core.Interfaces;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.Core.Services;

public class ScheduleService : IScheduleService
{
    private readonly AppDbContext _context;

    public ScheduleService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<TrainingSchedule>> GetAllAsync(bool? completed = null)
    {
        IQueryable<TrainingSchedule> query = _context.Schedules
            .Include(s => s.Drill)
            .AsNoTracking();

        if (completed.HasValue)
            query = query.Where(s => s.IsCompleted == completed.Value);

        return await query.OrderBy(s => s.ScheduledDate).ToListAsync();
    }

    public async Task<TrainingSchedule?> GetByIdAsync(int id)
    {
        return await _context.Schedules
            .Include(s => s.Drill)
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

        existing.ScheduledDate = schedule.ScheduledDate;
        existing.DurationMinutes = schedule.DurationMinutes;
        existing.IsCompleted = schedule.IsCompleted;
        existing.Notes = schedule.Notes;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkCompleteAsync(int id)
    {
        TrainingSchedule? existing = await _context.Schedules.FindAsync(id);
        if (existing is null) return false;

        existing.IsCompleted = true;
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
