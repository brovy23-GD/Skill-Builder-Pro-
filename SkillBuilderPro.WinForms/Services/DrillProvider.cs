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
        private static readonly HttpClient _availabilityClient;
        private static DateTime _lastUnavailableNoticeUtc = DateTime.MinValue;

        public static string LastSource { get; private set; } = "—";

        static DrillProvider()
        {
            var http = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5000/")
            };

            _api = new DrillApiClient(http);
            _availabilityClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5000/"),
                Timeout = TimeSpan.FromSeconds(3)
            };
        }

        public static async Task<List<SkillBuilderPro.WinForms.Models.Drill>> GetBySportAsync(string sportName, bool demoMode = false)
        {
            if (demoMode)
            {
                LastSource = "Demo";
                return DrillDatabase.GetDrillsBySport(sportName);
            }

            bool retry;
            do
            {
                retry = false;
                try
                {
                    using var healthResponse = await _availabilityClient.GetAsync("health");
                    healthResponse.EnsureSuccessStatusCode();
                }
                catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
                {
                    LastSource = "Services unavailable";
                    if (DateTime.UtcNow - _lastUnavailableNoticeUtc > TimeSpan.FromSeconds(30))
                    {
                        _lastUnavailableNoticeUtc = DateTime.UtcNow;
                        retry = MessageBox.Show(
                            "Skill Builder Pro services are currently unavailable. Start the API and try again.",
                            "Services Unavailable",
                            MessageBoxButtons.RetryCancel,
                            MessageBoxIcon.Information) == DialogResult.Retry;
                    }

                    if (!retry)
                        return new List<SkillBuilderPro.WinForms.Models.Drill>();
                }
            } while (retry);

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

                LastSource = "Services unavailable";
            }

            return new List<SkillBuilderPro.WinForms.Models.Drill>();
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
