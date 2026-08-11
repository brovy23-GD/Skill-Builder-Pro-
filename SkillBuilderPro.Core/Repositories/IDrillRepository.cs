// Location: SkillBuilderPro.Core/Repositories/IDrillRepository.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using SkillBuilderPro.Core.Models;
using Microsoft.EntityFrameworkCore; // 🟢 Required for .ToListAsync()


namespace SkillBuilderPro.Core.Repositories;

public interface IDrillRepository
{
    Task<IEnumerable<Drill>> GetDrillRangeAsync(int startId, int endId);
}
