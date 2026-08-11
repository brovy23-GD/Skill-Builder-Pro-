using System.Diagnostics;
using System.Net.Http;
using System.Windows.Forms;
using SkillBuilderPro.Core.Models;                 // Core Drill
using SkillBuilderPro.WinForms.Models;             // WinForms Drill
using SkillBuilderPro.Client.ApiClients;           // DrillApiClient
using SkillBuilderPro.Client.Services;             // ApiClient + IApiClient

namespace SkillBuilderPro.WinForms.Services
{
    public static class DrillProvider
    {
        private static readonly DrillApiClient _api;

        public static string LastSource { get; private set; } = "—";

        static DrillProvider()
        {
            var http = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:5001/")
            };

            _api = new DrillApiClient(http);
        }

        public static async Task<List<SkillBuilderPro.WinForms.Models.Drill>> GetBySportAsync(string sportName)
        {
            try
            {
                // Strongly typed Core Drill list
                List<SkillBuilderPro.Core.Models.Drill>? apiDrills = await _api.GetAllAsync();

                if (apiDrills != null && apiDrills.Count > 0)
                {
                    var filtered = apiDrills
                        .Where(d => d.Sport.Equals(sportName, StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    if (filtered.Count > 0)
                    {
                        LastSource = "API";
                        return filtered.Select(MapToWinForms).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DrillProvider] API error: {ex.GetType().Name}");
                Debug.WriteLine($"Message: {ex.Message}");
                Debug.WriteLine($"StackTrace:\n{ex.StackTrace}");

                string errorMsg = $"API Error:\n\n{ex.GetType().Name}\n\n{ex.Message}";
                if (ex.InnerException != null)
                    errorMsg += $"\n\nInner: {ex.InnerException.Message}";

                MessageBox.Show(errorMsg, "DrillProvider - API Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                LastSource = $"Offline ({ex.GetType().Name}: {ex.Message})";
            }

            // Offline fallback
            return DrillDatabase.GetDrillsBySport(sportName);
        }

        private static SkillBuilderPro.WinForms.Models.Drill MapToWinForms(
            SkillBuilderPro.Core.Models.Drill coreDrill)
        {
            return new SkillBuilderPro.WinForms.Models.Drill
            {
                Id = Guid.NewGuid(),                     // WinForms Drill uses Guid
                Name = coreDrill.Name,
                Sport = coreDrill.Sport,
                SkillCategory = coreDrill.Category,      // Core Drill uses Category
                Description = coreDrill.Description,
                VideoUrl = coreDrill.VideoUrl,
                Difficulty = (int)coreDrill.Difficulty        // Correct property name
            };
        }
    }
}
