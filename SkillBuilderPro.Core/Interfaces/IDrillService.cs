using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.Core.Interfaces;

public interface IDrillService
{
    Task<List<Drill>> GetAllAsync(string? sport = null, string? category = null);
    Task<Drill?> GetByIdAsync(int id);
    Task<Drill> CreateAsync(Drill drill);
    Task<bool> UpdateAsync(int id, Drill drill);
    Task<bool> DeleteAsync(int id);
}
