using SkillBuilderPro.API.Repositories;
using SkillBuilderPro.Core.Interfaces;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.API.Services
{
    /// <summary>
    /// Service implementation for drill-related business logic.
    /// </summary>
    public class DrillService : IDrillService
    {
        private readonly IRepository<Drill> _drillRepository;

        public DrillService(IRepository<Drill> drillRepository)
        {
            _drillRepository = drillRepository;
        }

        public async Task<List<Drill>> GetAllAsync(string? sport = null, string? category = null)
        {
            var drills = await _drillRepository.GetAllAsync();
            var result = drills.ToList();

            if (!string.IsNullOrEmpty(sport))
            {
                result = result.Where(d => d.Sport.ToLower() == sport.ToLower()).ToList();
            }

            if (!string.IsNullOrEmpty(category))
            {
                result = result.Where(d => d.Category.ToLower() == category.ToLower()).ToList();
            }

            return result;
        }

        public async Task<Drill?> GetByIdAsync(int id)
        {
            return await _drillRepository.GetByIdAsync(id);
        }

        public async Task<Drill> CreateAsync(Drill drill)
        {
            await _drillRepository.AddAsync(drill);
            await _drillRepository.SaveAsync();
            return drill;
        }

        public async Task<bool> UpdateAsync(int id, Drill drill)
        {
            drill.Id = id;
            await _drillRepository.UpdateAsync(drill);
            await _drillRepository.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var drill = await _drillRepository.GetByIdAsync(id);
            if (drill == null) return false;

            await _drillRepository.DeleteAsync(drill);
            await _drillRepository.SaveAsync();
            return true;
        }
    }
}