using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SkillBuilderPro.API.Data;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.API.Repositories
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

        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<IEnumerable<User>> FindAsync(Expression<Func<User, bool>> predicate)
        {
            return await _context.Users.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(User entity)
        {
            await _context.Users.AddAsync(entity);
        }

        public async Task UpdateAsync(User entity)
        {
            _context.Users.Update(entity);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(User entity)
        {
            _context.Users.Remove(entity);
            await Task.CompletedTask;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

}