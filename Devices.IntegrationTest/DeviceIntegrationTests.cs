using Azure;
using Devices.Api;
using Devices.Application.DTOs;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace Devices.IntegrationTest;

public class DeviceIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly string _baseUrl = "/api/v1/devices";

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

        var response = await _client.PostAsJsonAsync(_baseUrl, dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateDevice_ShouldReturnBadRequest_WhenInvalid()
    {
        var dto = new
        {
            Name = "",
            Brand = "Apple"
        };

        var response = await _client.PostAsJsonAsync(_baseUrl, dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetDevices_ShouldReturnOk()
    {
        var response = await _client.GetAsync(_baseUrl);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}