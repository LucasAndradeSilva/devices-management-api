using Devices.Api;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

public class DeviceIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public DeviceIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateDevice_ShouldReturnCreated()
    {
        var dto = new
        {
            Name = "iPhone",
            Brand = "Apple"
        };

        var response = await _client.PostAsJsonAsync("/api/devices", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}