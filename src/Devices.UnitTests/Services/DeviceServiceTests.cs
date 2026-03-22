using FluentAssertions;
using Moq;
using System.Net;
using Microsoft.Extensions.Logging;
using Devices.Application.Services;
using Devices.Application.DTOs;
using Devices.Application.Interfaces;
using Devices.Domain.Entities;
using Xunit;

namespace Devices.UnitTests.Services;

public class DeviceServiceTests
{
    private readonly Mock<IGenericRepository<Device>> _repositoryMock;
    private readonly Mock<ILogger<DeviceService>> _loggerMock;
    private readonly DeviceService _service;

    public DeviceServiceTests()
    {
        _repositoryMock = new Mock<IGenericRepository<Device>>();
        _loggerMock = new Mock<ILogger<DeviceService>>();

        _service = new DeviceService(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldFail_WhenNameIsEmpty()
    {
        var dto = new DeviceDto { Name = "", Brand = "Apple" };

        _repositoryMock
           .Setup(x => x.AddAsync(It.IsAny<Device>()))
           .Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(dto);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Device>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldSucceed_WhenValid()
    {
        var dto = new DeviceDto { Name = "iPhone", Brand = "Apple" };

        var result = await _service.CreateAsync(dto);

        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.Created);

        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Device>()), Times.Never);
    }
}