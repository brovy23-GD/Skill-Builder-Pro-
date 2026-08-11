// Location: SkillBuilderPro.API/Services/DrillApiService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SkillBuilderPro.Core.Models;
using SkillBuilderPro.Core.Interfaces;
using SkillBuilderPro.Core.Repositories;

namespace SkillBuilderPro.API.Services;

public class DrillApiService : IDrillService
{
    private readonly IRepository<Drill> _genericRepository;
    private readonly IDrillRepository _drillRepository;

    public DrillApiService(IRepository<Drill> genericRepository, IDrillRepository drillRepository)
    {
        _genericRepository = genericRepository ?? throw new ArgumentNullException(nameof(genericRepository));
        _drillRepository = drillRepository ?? throw new ArgumentNullException(nameof(drillRepository));
    }

    // 🟢 FIXED: Perfectly maps to the consolidated interface contract definition
    public async Task<IEnumerable<Drill>> GetAllAsync(string? sport, string? category = null)
    {
        var allDrills = await _genericRepository.GetAllAsync();
        var query = allDrills.AsQueryable();

        if (!string.IsNullOrWhiteSpace(sport))
            query = query.Where(d => d.Sport.Equals(sport, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(d => d.Category.Equals(category, StringComparison.OrdinalIgnoreCase));

        return query.ToList();
    }

    public async Task<Drill?> GetByIdAsync(int id) => await _genericRepository.GetByIdAsync(id);
    public async Task<Drill> CreateAsync(Drill drill) { await _genericRepository.AddAsync(drill); await _genericRepository.SaveAsync(); return drill; }
    public async Task UpdateAsync(int id, Drill drill) { drill.Id = id; await _genericRepository.UpdateAsync(drill); await _genericRepository.SaveAsync(); }
    public async Task DeleteAsync(int id) { var drill = await _genericRepository.GetByIdAsync(id); if (drill != null) { await _genericRepository.DeleteAsync(drill); await _genericRepository.SaveAsync(); } }
    public async Task<IEnumerable<Drill>> GetDrillRangeAsync(int startId, int endId) => await _drillRepository.GetDrillRangeAsync(startId, endId);
}
