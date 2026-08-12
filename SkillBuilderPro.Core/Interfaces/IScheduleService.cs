using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.Core.Interfaces;

public interface IScheduleService
{
    Task<List<TrainingSchedule>> GetAllAsync(bool? completed, int? ownerUserId);
    Task<TrainingSchedule?> GetByIdAsync(int id, int? ownerUserId);
    Task<TrainingSchedule?> CreateAsync(TrainingSchedule schedule);
    Task<bool> UpdateAsync(int id, TrainingSchedule schedule, int? ownerUserId);
    Task<bool> MarkCompleteAsync(int id, int? ownerUserId);
    Task<bool> DeleteAsync(int id, int? ownerUserId);
}
