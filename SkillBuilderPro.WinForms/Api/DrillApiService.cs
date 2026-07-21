using SkillBuilderPro.Core.Interfaces;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Web;
// ALIAS: WinForms has its own Drill class (Guid key, Duration, SkillCategory).
// Core has a DIFFERENT Drill (int key, Sport, VideoUrl). Alias avoids ambiguity.
using CoreDrill = SkillBuilderPro.Core.Models.Drill;

namespace SkillBuilderPro.WinForms;

/// <summary>
/// HTTP implementation of IDrillService. This class is the WinForms client
/// that talks to the SkillBuilderPro Web API.
/// </summary>
public class DrillApiService : IDrillService
{
    // ---------------------------------------------------------
    // UPDATED BASE ADDRESS — THIS IS THE FIX
    // ---------------------------------------------------------
    // Your API is running at:
    //   http://localhost:5000
    //   https://localhost:5001
    //
    // The old port (62978) was DEAD, so WinForms always fell back to offline mode.
    // Updating this BaseAddress brings the API ONLINE again.
    //
    // Trailing slash REQUIRED: BaseAddress + "api/drills" concatenates correctly.
    // ---------------------------------------------------------
    private static readonly HttpClient _http = new HttpClient
    {
        BaseAddress = new Uri("http://localhost:5000/"),   // HTTP, not HTTPS

        // Fail fast if API is down — 10 seconds instead of hanging forever.
        Timeout = TimeSpan.FromSeconds(10)
    };

    public async Task<List<CoreDrill>> GetAllAsync(string? sport = null, string? category = null)
    {
        try
        {
            var query = HttpUtility.ParseQueryString(string.Empty);

            if (!string.IsNullOrWhiteSpace(sport))
                query["sport"] = sport;

            if (!string.IsNullOrWhiteSpace(category))
                query["category"] = category;

            string url = query.Count > 0 ? $"api/drills?{query}" : "api/drills";
            string fullUrl = _http.BaseAddress + url;

            System.Diagnostics.Debug.WriteLine($"[DrillApiService] Requesting: {fullUrl}");

            // Retry 3 times with 1-second delays (API may be starting up)
            int retries = 3;
            while (retries > 0)
            {
                try
                {
                    List<CoreDrill>? drills = await _http.GetFromJsonAsync<List<CoreDrill>>(url);
                    System.Diagnostics.Debug.WriteLine($"[DrillApiService] Success: got {drills?.Count ?? 0} drills");
                    return drills ?? new List<CoreDrill>();
                }
                catch (HttpRequestException) when (retries > 1)
                {
                    retries--;
                    System.Diagnostics.Debug.WriteLine($"[DrillApiService] Connection failed, retrying... ({retries} left)");
                    await Task.Delay(1000);  // Wait 1 second before retry
                }
            }

            return new List<CoreDrill>();
        }
        catch (HttpRequestException ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DrillApiService] HttpRequestException: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"[DrillApiService] InnerException: {ex.InnerException?.Message}");
            return new List<CoreDrill>();
        }
        catch (JsonException ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DrillApiService] JSON deserialization failed: {ex.Message}");
            return new List<CoreDrill>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DrillApiService] {ex.GetType().Name}: {ex.Message}");
            return new List<CoreDrill>();
        }
    }

    public async Task<CoreDrill?> GetByIdAsync(int id)
    {
        HttpResponseMessage response = await _http.GetAsync($"api/drills/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<CoreDrill>();
    }

    public async Task<CoreDrill> CreateAsync(CoreDrill drill)
    {
        HttpResponseMessage response = await _http.PostAsJsonAsync("api/drills", drill);
        response.EnsureSuccessStatusCode();

        CoreDrill? created = await response.Content.ReadFromJsonAsync<CoreDrill>();

        return created ?? throw new InvalidOperationException("API returned success but no drill body.");
    }

    public async Task<bool> UpdateAsync(int id, CoreDrill drill)
    {
        HttpResponseMessage response = await _http.PutAsJsonAsync($"api/drills/{id}", drill);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        HttpResponseMessage response = await _http.DeleteAsync($"api/drills/{id}");
        return response.IsSuccessStatusCode;
    }
}