using SkillBuilderPro.Client.Services;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.Client.ApiClients;

public class DrillApiClient
{
    private readonly IApiClient _api;

    public DrillApiClient(IApiClient api)
    {
        _api = api;
    }

    public Task<List<Drill>?> GetAllAsync()
        => _api.GetAsync<List<Drill>>("api/drills");

    public Task<Drill?> GetByIdAsync(int id)
        => _api.GetAsync<Drill>($"api/drills/{id}");
}
