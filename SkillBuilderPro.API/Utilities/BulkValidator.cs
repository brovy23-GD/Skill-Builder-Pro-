using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkillBuilderPro.Core.Data;
using SkillBuilderPro.API.Utilities;

namespace SkillBuilderPro.API.Utilities
{
    public static class BulkValidator
    {
        public static async Task<int> RemoveBrokenDrillUrlsAsync(AppDbContext db, ILogger logger)
        {
            var allDrills = await db.Drills.ToListAsync();
            var broken = new List<SkillBuilderPro.Core.Models.Drill>();

            foreach (var drill in allDrills)
            {
                if (!await UrlValidator.IsValidYouTubeUrlAsync(drill.VideoUrl))
                {
                    broken.Add(drill);
                }
            }

            if (broken.Count > 0)
            {
                db.Drills.RemoveRange(broken);
                await db.SaveChangesAsync();
            }

            logger.LogWarning("🧹 Bulk Validator removed {Count} broken drill URLs.", broken.Count);

            return broken.Count;
        }
    }
}
