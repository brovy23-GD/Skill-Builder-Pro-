using System.Net;
using System.Text;
using System.Text.Json;
using SkillBuilderPro.Client.Services;

namespace SkillBuilderPro.Tests.Client;

public sealed class ApiClientTests
{
    [Fact]
    public async Task GetAsync_WhenResponseIsSuccessful_DeserializesPayloadAndUsesGet()
    {
        HttpRequestMessage? captured = null;
        var client = CreateClient(request =>
        {
            captured = request;
            return Json(HttpStatusCode.OK, new SampleResponse(7, "Footwork"));
        });

        var result = await client.GetAsync<SampleResponse>("api/drills/7");

        Assert.Equal(new SampleResponse(7, "Footwork"), result);
        Assert.Equal(HttpMethod.Get, captured!.Method);
        Assert.Equal("https://tests.local/api/drills/7", captured.RequestUri!.AbsoluteUri);
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.Forbidden)]
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.Conflict)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public async Task GetAsync_WhenResponseIsNotSuccessful_ReturnsNull(HttpStatusCode status)
    {
        var client = CreateClient(_ => new HttpResponseMessage(status));
        Assert.Null(await client.GetAsync<SampleResponse>("api/drills/7"));
    }

    [Fact]
    public async Task GetAsync_WhenBodyIsMalformedJson_ReturnsNull()
    {
        var client = CreateClient(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{not-json", Encoding.UTF8, "application/json")
        });
        Assert.Null(await client.GetAsync<SampleResponse>("api/drills/7"));
    }

    [Fact]
    public async Task GetAsync_WhenBodyIsEmpty_ReturnsNull()
    {
        var client = CreateClient(_ => new HttpResponseMessage(HttpStatusCode.NoContent));
        Assert.Null(await client.GetAsync<SampleResponse>("api/drills/7"));
    }

    [Fact]
    public async Task GetAsync_WhenTransportFails_ReturnsNull()
    {
        var client = CreateClient(_ => throw new HttpRequestException("offline"));
        Assert.Null(await client.GetAsync<SampleResponse>("api/drills/7"));
    }

    [Fact]
    public async Task PostAsync_WhenCreated_SerializesRequestAndDeserializesResponse()
    {
        string? body = null;
        HttpRequestMessage? captured = null;
        var client = CreateClient(request =>
        {
            captured = request;
            body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            return Json(HttpStatusCode.Created, new SampleResponse(8, "Created"));
        });

        var result = await client.PostAsync<CreateRequest, SampleResponse>("api/drills", new CreateRequest("Created"));

        Assert.Equal(new SampleResponse(8, "Created"), result);
        Assert.Equal(HttpMethod.Post, captured!.Method);
        Assert.Equal("Created", JsonDocument.Parse(body!).RootElement.GetProperty("name").GetString());
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.Forbidden)]
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.Conflict)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public async Task PostAsync_WhenResponseIsNotSuccessful_ReturnsNull(HttpStatusCode status)
    {
        var client = CreateClient(_ => new HttpResponseMessage(status));
        Assert.Null(await client.PostAsync<CreateRequest, SampleResponse>("api/drills", new CreateRequest("Drill")));
    }

    [Fact]
    public async Task PostAsync_WhenResponseBodyIsMalformed_ReturnsNull()
    {
        var client = CreateClient(_ => new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent("not-json", Encoding.UTF8, "application/json")
        });
        Assert.Null(await client.PostAsync<CreateRequest, SampleResponse>("api/drills", new CreateRequest("Drill")));
    }

    [Fact]
    public async Task PostAsync_WhenTransportFails_ReturnsNull()
    {
        var client = CreateClient(_ => throw new TaskCanceledException("timeout"));
        Assert.Null(await client.PostAsync<CreateRequest, SampleResponse>("api/drills", new CreateRequest("Drill")));
    }

    private static ApiClient CreateClient(Func<HttpRequestMessage, HttpResponseMessage> response) =>
        new(new HttpClient(new StubHandler(response)) { BaseAddress = new Uri("https://tests.local/") });

    private static HttpResponseMessage Json<T>(HttpStatusCode status, T value) => new(status)
    {
        Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json")
    };

    private sealed class StubHandler(Func<HttpRequestMessage, HttpResponseMessage> response) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(response(request));
    }

    private sealed record SampleResponse(int Id, string Name);
    private sealed record CreateRequest(string Name);
}
