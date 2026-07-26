using System.Linq.Expressions;

namespace SkillBuilderPro.API.Repositories
{
    /// <summary>
    /// Generic repository interface for data access operations.
    /// </summary>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Gets an entity by its primary key.
        /// </summary>
        Task<T> GetByIdAsync(int id);

        /// <summary>
        /// Gets all entities.
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Finds entities matching a predicate.
        /// </summary>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Adds a new entity.
        /// </summary>
        Task AddAsync(T entity);

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        Task UpdateAsync(T entity);

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        Task DeleteAsync(T entity);

        /// <summary>
        /// Saves all changes to the database.
        /// </summary>
        Task SaveAsync();
    }
}