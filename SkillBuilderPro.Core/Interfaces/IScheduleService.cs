using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.Core.Interfaces;

public interface IScheduleService
{
    Task<List<TrainingSchedule>> GetAllAsync(bool? completed = null);
    Task<TrainingSchedule?> GetByIdAsync(int id);
    Task<TrainingSchedule?> CreateAsync(TrainingSchedule schedule);
    Task<bool> UpdateAsync(int id, TrainingSchedule schedule);
    Task<bool> MarkCompleteAsync(int id);
    Task<bool> DeleteAsync(int id);
}
