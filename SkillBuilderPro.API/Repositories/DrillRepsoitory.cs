using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SkillBuilderPro.API.Data;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.API.Repositories
{
    /// <summary>
    /// Repository implementation for Drill entity.
    /// Handles all data access operations for drills.
    /// </summary>
    public class DrillRepository : IRepository<Drill>
    {
        private readonly AppDbContext _context;

        public DrillRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets a drill by its ID.
        /// </summary>
        public async Task<Drill> GetByIdAsync(int id)
        {
            return await _context.Drills.FirstOrDefaultAsync(d => d.Id == id);
        }

        /// <summary>
        /// Gets all drills.
        /// </summary>
        public async Task<IEnumerable<Drill>> GetAllAsync()
        {
            return await _context.Drills.ToListAsync();
        }

        /// <summary>
        /// Finds drills matching a predicate (e.g., by sport, difficulty).
        /// </summary>
        public async Task<IEnumerable<Drill>> FindAsync(Expression<Func<Drill, bool>> predicate)
        {
            return await _context.Drills.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Adds a new drill to the database.
        /// </summary>
        public async Task AddAsync(Drill entity)
        {
            await _context.Drills.AddAsync(entity);
        }

        /// <summary>
        /// Updates an existing drill.
        /// </summary>
        public async Task UpdateAsync(Drill entity)
        {
            _context.Drills.Update(entity);
            await Task.CompletedTask;
        }

        /// <summary>
        /// Deletes a drill from the database.
        /// </summary>
        public async Task DeleteAsync(Drill entity)
        {
            _context.Drills.Remove(entity);
            await Task.CompletedTask;
        }

        /// <summary>
        /// Commits all changes to the database.
        /// </summary>
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}