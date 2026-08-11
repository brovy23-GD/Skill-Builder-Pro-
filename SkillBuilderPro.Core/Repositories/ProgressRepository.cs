using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SkillBuilderPro.Core.Data;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.Core.Repositories
{
    /// <summary>
    /// Repository implementation for ProgressLog entity.
    /// Handles all data access operations for athlete progress tracking.
    /// </summary>
    public class ProgressRepository : IRepository<ProgressLog>
    {
        private readonly AppDbContext _context;

        public ProgressRepository(AppDbContext context)
        {
            _context = context;
        }

        // 🟢 FIXED: Change signature return type to include '?' to match the IRepository contract
        public async Task<ProgressLog?> GetByIdAsync(int id)
        {
            return await _context.Set<ProgressLog>().FindAsync(id);
        }

        public async Task<IEnumerable<ProgressLog>> GetAllAsync()
        {
            return await _context.ProgressLogs.ToListAsync();
        }

        // 🟢 ELITE CORRECTION: Match the Task<ProgressLog?> contract return signature
        public async Task<ProgressLog?> FindAsync(System.Linq.Expressions.Expression<Func<ProgressLog, bool>> predicate)
        {
            return await _context.Set<ProgressLog>().FirstOrDefaultAsync(predicate);
        }

        public async Task AddAsync(ProgressLog entity)
        {
            await _context.ProgressLogs.AddAsync(entity);
        }

        public async Task UpdateAsync(ProgressLog entity)
        {
            _context.ProgressLogs.Update(entity);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(ProgressLog entity)
        {
            _context.ProgressLogs.Remove(entity);
            await Task.CompletedTask;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}