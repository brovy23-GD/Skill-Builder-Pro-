using Microsoft.EntityFrameworkCore;
using SkillBuilderPro.Core.Data;
using SkillBuilderPro.Core.Interfaces;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.API.Services;

public class ScheduleService : IScheduleService
{
    private readonly AppDbContext _context;

    public ScheduleService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<TrainingSchedule>> GetAllAsync(
        bool? completed,
        int? ownerUserId)
    {
        IQueryable<TrainingSchedule> query = _context.Schedules.AsNoTracking();

        if (ownerUserId.HasValue)
        {
            query = query.Where(schedule =>
                schedule.OwnerUserId == ownerUserId.Value);
        }

        if (completed.HasValue)
        {
            string status = completed.Value ? "Completed" : "Pending";
            query = query.Where(schedule => schedule.Status == status);
        }

        return await query.OrderBy(schedule => schedule.Id).ToListAsync();
    }

    public async Task<TrainingSchedule?> GetByIdAsync(
        int id,
        int? ownerUserId)
    {
        IQueryable<TrainingSchedule> query = _context.Schedules.AsNoTracking();

        if (ownerUserId.HasValue)
        {
            query = query.Where(schedule =>
                schedule.OwnerUserId == ownerUserId.Value);
        }

        return await query.FirstOrDefaultAsync(schedule => schedule.Id == id);
    }

    public async Task<TrainingSchedule?> CreateAsync(
        TrainingSchedule schedule)
    {
        bool drillExists = await _context.Drills.AnyAsync(drill =>
            drill.Id == schedule.DrillId);
        if (!drillExists)
        {
            return null;
        }

        _context.Schedules.Add(schedule);
        await _context.SaveChangesAsync();
        return schedule;
    }

    public async Task<bool> UpdateAsync(
        int id,
        TrainingSchedule schedule,
        int? ownerUserId)
    {
        IQueryable<TrainingSchedule> query = _context.Schedules;

        if (ownerUserId.HasValue)
        {
            query = query.Where(existing =>
                existing.OwnerUserId == ownerUserId.Value);
        }

        var existing = await query.FirstOrDefaultAsync(existing =>
            existing.Id == id);
        if (existing is null)
        {
            return false;
        }

        existing.DrillId = schedule.DrillId;
        existing.Title = schedule.Title;
        existing.Description = schedule.Description;
        existing.Status = schedule.Status;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkCompleteAsync(int id, int? ownerUserId)
    {
        IQueryable<TrainingSchedule> query = _context.Schedules;

        if (ownerUserId.HasValue)
        {
            query = query.Where(existing =>
                existing.OwnerUserId == ownerUserId.Value);
        }

        var existing = await query.FirstOrDefaultAsync(existing =>
            existing.Id == id);
        if (existing is null)
        {
            return false;
        }

        existing.Status = "Completed";
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id, int? ownerUserId)
    {
        IQueryable<TrainingSchedule> query = _context.Schedules;

        if (ownerUserId.HasValue)
        {
            query = query.Where(existing =>
                existing.OwnerUserId == ownerUserId.Value);
        }

        var existing = await query.FirstOrDefaultAsync(existing =>
            existing.Id == id);
        if (existing is null)
        {
            return false;
        }

        _context.Schedules.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<TrainingSchedule>> GetAllForAthleteAsync(
        int athleteUserId,
        bool? completed)
    {
        var query = _context.Schedules.AsNoTracking()
            .Where(schedule => schedule.OwnerUserId == athleteUserId);

        if (completed.HasValue)
        {
            string status = completed.Value ? "Completed" : "Pending";
            query = query.Where(schedule => schedule.Status == status);
        }

        return await query.OrderBy(schedule => schedule.Id).ToListAsync();
    }

    public Task<TrainingSchedule?> GetByIdForAthleteAsync(
        int athleteUserId,
        int id) =>
        _context.Schedules.AsNoTracking().FirstOrDefaultAsync(
            schedule => schedule.Id == id
                && schedule.OwnerUserId == athleteUserId);
}
