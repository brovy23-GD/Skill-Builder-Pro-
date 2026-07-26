using SkillBuilderPro.Core.Interfaces;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Web;
using CoreDrill = SkillBuilderPro.Core.Models.Drill;

namespace SkillBuilderPro.WinForms;

public class DrillApiService : IDrillService
{
    private static readonly HttpClient _http = new HttpClient
    {
        BaseAddress = new Uri("http://localhost:5000/"),
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
                    await Task.Delay(1000);
                }
            }

            return new List<CoreDrill>();
        }
        catch (HttpRequestException ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DrillApiService] HttpRequestException: {ex.Message}");
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