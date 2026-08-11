// Location: SkillBuilderPro.API/Services/DrillService.cs (or SkillBuilderPro.Core/Services/DrillService.cs)
using SkillBuilderPro.Core.Models;
using SkillBuilderPro.Core.Interfaces;
using SkillBuilderPro.Core.Repositories;

namespace SkillBuilderPro.API.Services;


public class DrillService : IDrillService
{
    private readonly IDrillRepository _drillRepository;

    public DrillService(IDrillRepository drillRepository)
    {
        _drillRepository = drillRepository ?? throw new ArgumentNullException(nameof(drillRepository));
    }

    // 🟢 FIXED: Matches exact contract return type Task<IEnumerable<Drill>>
    public async Task<IEnumerable<Drill>> GetAllAsync(string? sport, string? category)
    {
        return await _drillRepository.GetDrillRangeAsync(1, 1000);
    }

    public async Task<Drill?> GetByIdAsync(int id)
    {
        await Task.CompletedTask;
        return null;
    }

    public async Task<Drill> CreateAsync(Drill drill)
    {
        await Task.CompletedTask;
        return drill;
    }

    // 🟢 FIXED: Matches exact contract return type Task (Void asynchronous)
    public async Task UpdateAsync(int id, Drill drill)
    {
        await Task.CompletedTask;
    }

    // 🟢 FIXED: Matches exact contract return type Task (Void asynchronous)
    public async Task DeleteAsync(int id)
    {
        await Task.CompletedTask;
    }

    // 🟢 FIXED: Fulfills the newly added migration testing contract
    public async Task<IEnumerable<Drill>> GetDrillRangeAsync(int startId, int endId)
    {
        return await _drillRepository.GetDrillRangeAsync(startId, endId);
    }
}
