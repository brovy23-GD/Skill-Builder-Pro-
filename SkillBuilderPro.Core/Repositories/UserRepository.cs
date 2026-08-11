using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SkillBuilderPro.Core.Data;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.Core.Repositories
{
    /// <summary>
    /// Repository implementation for User entity.
    /// Handles all data access operations for users and authentication.
    /// </summary>
    public class UserRepository : IRepository<User>
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Set<User>().FindAsync(id);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.LegacyUsers.ToListAsync();
        }

        public async Task<User?> FindAsync(Expression<Func<User, bool>> predicate)
        {
            return await _context.Set<User>().FirstOrDefaultAsync(predicate);
        }

        public async Task AddAsync(User entity)
        {
            await _context.LegacyUsers.AddAsync(entity);
        }

        public Task UpdateAsync(User entity)
        {
            _context.LegacyUsers.Update(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(User entity)
        {
            _context.LegacyUsers.Remove(entity);
            return Task.CompletedTask;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
