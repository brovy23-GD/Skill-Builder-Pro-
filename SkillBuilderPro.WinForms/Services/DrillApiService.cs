using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SkillBuilderPro.Client.Services;
using SkillBuilderPro.Core.Interfaces;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.WinForms.Services;

public class DrillApiService : IDrillService
{
    private readonly IApiClient _apiClient;

    public DrillApiService(IApiClient apiClient)
    {
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
    }

    public async Task<IEnumerable<Drill>> GetAllAsync(string? sport, string? category = null)
    {
        string endpoint =
            $"api/Drills?sport={Uri.EscapeDataString(sport ?? "")}&category={Uri.EscapeDataString(category ?? "")}";

        var drills = await _apiClient.GetAsync<List<Drill>>(endpoint);
        return drills ?? new List<Drill>();
    }

    public async Task<Drill?> GetByIdAsync(int id)
    {
        return await _apiClient.GetAsync<Drill>($"api/Drills/{id}");
    }

    public async Task<Drill> CreateAsync(Drill drill)
    {
        if (drill == null) throw new ArgumentNullException(nameof(drill));

        var created = await _apiClient.PostAsync<Drill, Drill>("api/Drills", drill);
        return created ?? throw new InvalidOperationException("API returned no drill after create.");
    }

    public Task UpdateAsync(int id, Drill drill)
    {
        throw new NotImplementedException("IApiClient does not yet support PUT.");
    }

    public Task DeleteAsync(int id)
    {
        throw new NotImplementedException("IApiClient does not yet support DELETE.");
    }

    public async Task<IEnumerable<Drill>> GetDrillRangeAsync(int startId, int endId)
    {
        var drills = await _apiClient.GetAsync<List<Drill>>($"api/Drills/range/{startId}/{endId}");
        return drills ?? new List<Drill>();
    }
}