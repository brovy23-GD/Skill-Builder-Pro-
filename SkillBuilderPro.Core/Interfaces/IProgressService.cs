using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.Core.Interfaces;

public interface IProgressService
{
    Task<List<ProgressLog>> GetAllAsync(int? drillId = null);
    Task<ProgressLog?> GetByIdAsync(int id);
    Task<ProgressLog?> CreateAsync(ProgressLog log);
    Task<bool> DeleteAsync(int id);
    Task<double?> GetAverageRatingAsync(int drillId);
}
