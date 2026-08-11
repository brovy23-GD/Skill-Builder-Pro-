using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; // 🟢 Essential for .ToListAsync() and .FindAsync()
using SkillBuilderPro.Core.Data;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.Core.Repositories;

public class DrillRepository : IRepository<Drill>, IDrillRepository
{
    private readonly AppDbContext _context;

    public DrillRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    // 🟢 FIXED: Implements the missing interface member contract
    public async Task<Drill?> GetByIdAsync(int id)
    {
        return await _context.Drills.FindAsync(id);
    }

    public async Task<IEnumerable<Drill>> GetAllAsync()
    {
        return await _context.Drills.AsNoTracking().ToListAsync();
    }

    public async Task<Drill?> FindAsync(Expression<Func<Drill, bool>> predicate)
    {
        return await _context.Drills.FirstOrDefaultAsync(predicate);
    }

    public async Task AddAsync(Drill entity)
    {
        await _context.Drills.AddAsync(entity);
    }

    public async Task UpdateAsync(Drill entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Drill entity)
    {
        _context.Drills.Remove(entity);
        await Task.CompletedTask;
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }

    // 🟢 FIXED: Removed duplicate copies and forced clean EF materialization (No PLINQ fallbacks)
    public async Task<IEnumerable<Drill>> GetDrillRangeAsync(int startId, int endId)
    {
        if (startId > endId) return Enumerable.Empty<Drill>();

        return await _context.Drills
            .Where(d => d.Id >= startId && d.Id <= endId)
            .AsNoTracking()
            .ToListAsync();
    }
}
