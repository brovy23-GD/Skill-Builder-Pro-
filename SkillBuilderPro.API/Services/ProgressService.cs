using SkillBuilderPro.Core.Interfaces;
using SkillBuilderPro.Core.Models;
using SkillBuilderPro.Core.Repositories;
using Microsoft.EntityFrameworkCore; // 🟢 Essential for high-performance database materializations

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
            // 1. Fetch all raw progress items asynchronously from the data layer
            var allLogs = await _progressRepository.GetAllAsync();

            // 2. Filter down to the matching drillId and materialize into memory
            var logList = allLogs.Where(l => l.DrillId == drillId).ToList();

            // 3. Defensive Boundary: If no records match this drill, return null cleanly
            if (!logList.Any())
            {
                return null;
            }

            // 4. Safely compute the double calculation over the matching Rating parameters
            return logList.Average(l => (double?)l.Rating);
        }
    }
}
