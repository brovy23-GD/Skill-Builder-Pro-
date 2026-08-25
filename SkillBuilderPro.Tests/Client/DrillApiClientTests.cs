using SkillBuilderPro.Client.ApiClients;
using SkillBuilderPro.Client.Services;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.Tests.Client;

public sealed class DrillApiClientTests
{
    [Fact]
    public async Task GetAllAsync_RequestsCanonicalDrillsEndpointAndReturnsResponse()
    {
        var expected = new List<Drill> { new() { Id = 7, Name = "Footwork" } };
        var api = new RecordingApiClient { Response = expected };
        var client = new DrillApiClient(api);

        var actual = await client.GetAllAsync();

        Assert.Same(expected, actual);
        Assert.Equal("api/drills", api.LastEndpoint);
        Assert.Equal(typeof(List<Drill>), api.LastResponseType);
    }

    [Theory]
    [InlineData(1, "api/drills/1")]
    [InlineData(42, "api/drills/42")]
    public async Task GetByIdAsync_UsesRequestedIdInCanonicalEndpoint(int id, string expectedEndpoint)
    {
        var expected = new Drill { Id = id, Name = "Drill" };
        var api = new RecordingApiClient { Response = expected };
        var client = new DrillApiClient(api);

        var actual = await client.GetByIdAsync(id);

        Assert.Same(expected, actual);
        Assert.Equal(expectedEndpoint, api.LastEndpoint);
        Assert.Equal(typeof(Drill), api.LastResponseType);
    }

    private sealed class RecordingApiClient : IApiClient
    {
        public object? Response { get; init; }
        public string? LastEndpoint { get; private set; }
        public Type? LastResponseType { get; private set; }

        public Task<T?> GetAsync<T>(string endpoint)
        {
            LastEndpoint = endpoint;
            LastResponseType = typeof(T);
            return Task.FromResult((T?)Response);
        }

        public Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data) =>
            throw new NotSupportedException();
    }
}
