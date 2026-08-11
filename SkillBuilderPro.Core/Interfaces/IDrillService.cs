// Location: SkillBuilderPro.Core/Interfaces/IDrillService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.Core.Interfaces;

/// <summary>
/// Elite Principal Architectural Contract for the Drills Business Logic Service Layer.
/// </summary>
public interface IDrillService
{
    // 🟢 ELITE FIX: One method signature to handle all filtering variations cleanly.
    // Making category optional with a default null value allows single-parameter calls to work out-of-the-box.
    Task<IEnumerable<Drill>> GetAllAsync(string? sport, string? category = null);

    Task<Drill?> GetByIdAsync(int id);
    Task<Drill> CreateAsync(Drill drill);
    Task UpdateAsync(int id, Drill drill);
    Task DeleteAsync(int id);

    // Integrity Verification Range Query Contract
    Task<IEnumerable<Drill>> GetDrillRangeAsync(int startId, int endId);
}
