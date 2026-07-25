using System.Diagnostics;
using SkillBuilderPro.MAUI.Models;

namespace SkillBuilderPro.MAUI.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl = "http://localhost:5000/api";

    public ApiService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<List<Drill>> GetDrillsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/drils");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<List<Drill>>(json)
                    ?? new List<Drill>();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error fetching drills: {ex.Message}");
        }

        return new List<Drill>();
    }

    public async Task<List<Drill>> GetDrillsBySportAsync(string sport)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/drils/sport/{sport}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<List<Drill>>(json)
                    ?? new List<Drill>();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error fetching drills by sport: {ex.Message}");
        }

        return new List<Drill>();
    }

    public async Task LogProgressAsync(int userId, int drillId, int reps)
    {
        try
        {
            var progress = new AthleteProgress
            {
                UserId = userId,
                DrillId = drillId,
                CompletedDate = DateTime.Now,
                RepetitionsCompleted = reps
            };

            var json = System.Text.Json.JsonSerializer.Serialize(progress);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            await _httpClient.PostAsync($"{_baseUrl}/progress", content);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error logging progress: {ex.Message}");
        }
    }
}