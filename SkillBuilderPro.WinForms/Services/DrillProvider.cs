using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SkillBuilderPro.WinForms.Models;

namespace SkillBuilderPro.WinForms.Services
{
    /// <summary>
    /// Single entry point for drills. Prefers the Web API; falls back to the
    /// offline DrillDatabase when the API is unreachable. Callers get WinForms
    /// Drills either way and never know which source answered.
    /// </summary>
    public static class DrillProvider
    {
        private static readonly DrillApiService _api = new DrillApiService();

        /// <summary>Which source answered the last request: "API" or "Offline".</summary>
        public static string LastSource { get; private set; } = "—";

        public static async Task<List<Drill>> GetBySportAsync(string sport)
        {
            try
            {
                var apiDrills = await _api.GetAllAsync(sport);
                if (apiDrills != null && apiDrills.Count > 0)
                {
                    LastSource = "API";
                    return apiDrills.Select(MapToWinForms).ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DrillProvider] API error: {ex.GetType().Name}");
                Debug.WriteLine($"Message: {ex.Message}");
                Debug.WriteLine($"StackTrace:\n{ex.StackTrace}");

                // Show error popup to user
                string errorMsg = $"API Error:\n\n{ex.GetType().Name}\n\n{ex.Message}";
                if (ex.InnerException != null)
                    errorMsg += $"\n\nInner: {ex.InnerException.Message}";

                MessageBox.Show(errorMsg, "DrillProvider - API Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);

                LastSource = $"Offline ({ex.GetType().Name}: {ex.Message})";
            }

            // Fallback to offline
            return DrillDatabase.GetDrillsBySport(sport);
        }

        /// <summary>
        /// Maps Core.Models.Drill to WinForms.Models.Drill.
        /// Core has Category, SubCategory, int Id; WinForms has SkillCategory, Guid Id.
        /// </summary>
        private static Drill MapToWinForms(dynamic coreDrill)
        {
            return new Drill
            {
                Id = Guid.NewGuid(),
                Name = coreDrill.Name,
                Sport = coreDrill.Sport,
                SkillCategory = coreDrill.Category ?? "",  // Core only has Category, not SubCategory
                Description = coreDrill.Description,
                VideoUrl = coreDrill.VideoUrl,
                Difficulty = coreDrill.Difficulty
            };
        }
    }
}