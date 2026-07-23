using System.Net.Http;
using System.Net.Http.Json;
using SkillBuilderPro.WinForms.Models;

namespace SkillBuilderPro.WinForms.Services
{
    public class DrillApiClient
    {
        private readonly HttpClient _http;

        public DrillApiClient(string baseUrl)
        {
            _http = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
        }

        public async Task<List<ApiDrill>> GetAllDrillsAsync()
        {
            try
            {
                var drills = await _http.GetFromJsonAsync<List<ApiDrill>>("api/drills");
                return drills ?? new List<ApiDrill>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"API Error: {ex.Message}");
                return new List<ApiDrill>();
            }
        }
    }
}
