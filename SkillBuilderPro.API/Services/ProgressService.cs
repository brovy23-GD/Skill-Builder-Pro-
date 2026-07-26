using SkillBuilderPro.API.Repositories;
using SkillBuilderPro.Core.Interfaces;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.API.Services
{
    /// <summary>
    /// Service implementation for progress tracking.
    /// </summary>
    public class ProgressService : IProgressService
    {
        private readonly IRepository<ProgressLog> _progressRepository;

        public ProgressService(IRepository<ProgressLog> progressRepository)
        {
            _progressRepository = progressRepository;
        }

        public async Task<List<ProgressLog>> GetAllAsync(int? drillId = null)
        {
            var logs = await _progressRepository.GetAllAsync();
            var result = logs.ToList();

            if (drillId.HasValue)
            {
                result = result.Where(l => l.DrillId == drillId.Value).ToList();
            }

            return result;
        }

        public async Task<ProgressLog?> GetByIdAsync(int id)
        {
            return await _progressRepository.GetByIdAsync(id);
        }

        public async Task<ProgressLog?> CreateAsync(ProgressLog log)
        {
            await _progressRepository.AddAsync(log);
            await _progressRepository.SaveAsync();
            return log;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var log = await _progressRepository.GetByIdAsync(id);
            if (log == null) return false;

            await _progressRepository.DeleteAsync(log);
            await _progressRepository.SaveAsync();
            return true;
        }

        public async Task<double?> GetAverageRatingAsync(int drillId)
        {
            var logs = await _progressRepository.FindAsync(l => l.DrillId == drillId);
            var logList = logs.ToList();

            if (!logList.Any()) return null;

            return logList.Average(l => l.Rating);
        }
    }
}