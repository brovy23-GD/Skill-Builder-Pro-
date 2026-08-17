using System.Diagnostics;
using SkillBuilderPro.MAUI.Models;

namespace SkillBuilderPro.MAUI.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public ApiService()
    {
        _httpClient = new HttpClient { BaseAddress = ApiEndpointResolver.Resolve() };
        _baseUrl = new Uri(_httpClient.BaseAddress, "api").AbsoluteUri.TrimEnd('/');
    }

    public async Task<List<Drill>> GetDrillsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/drills");
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
            var response = await _httpClient.GetAsync($"{_baseUrl}/drills?sport={sport}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"API Response: {json}");
                var drills = System.Text.Json.JsonSerializer.Deserialize<List<Drill>>(json)
                    ?? new List<Drill>();
                Debug.WriteLine($"Deserialized {drills.Count} drills");
                return drills;
            }
            else
            {
                Debug.WriteLine($"API Error: {response.StatusCode} - {response.ReasonPhrase}");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error fetching drills by sport: {ex.Message}\n{ex.StackTrace}");
        }
        return new List<Drill>();
    }
}
