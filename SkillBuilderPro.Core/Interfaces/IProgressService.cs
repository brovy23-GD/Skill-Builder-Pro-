using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.Core.Interfaces;

public interface IProgressService
{
    Task<List<ProgressLog>> GetAllAsync(int? drillId, int? ownerUserId);
    Task<ProgressLog?> GetByIdAsync(int id, int? ownerUserId);
    Task<ProgressLog?> CreateAsync(ProgressLog log);
    Task<bool> DeleteAsync(int id, int? ownerUserId);
    Task<double?> GetAverageRatingAsync(int drillId, int? ownerUserId);
}
