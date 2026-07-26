using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SkillBuilderPro.API.Data;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.API.Repositories
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

        public async Task<ProgressLog> GetByIdAsync(int id)
        {
            return await _context.ProgressLogs.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<ProgressLog>> GetAllAsync()
        {
            return await _context.ProgressLogs.ToListAsync();
        }

        public async Task<IEnumerable<ProgressLog>> FindAsync(Expression<Func<ProgressLog, bool>> predicate)
        {
            return await _context.ProgressLogs.Where(predicate).ToListAsync();
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