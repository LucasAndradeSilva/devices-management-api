using Devices.Application.DTOs;
using Devices.Application.Interfaces;
using Devices.Application.Services;
using Devices.Domain.Entities;
using Devices.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Devices.UnitTest.Services
{
    public class DeviceServiceDomainTests
    {
        private readonly Mock<IGenericRepository<Device>> _repositoryMock;
        private readonly Mock<ILogger<DeviceService>> _loggerMock;
        private readonly DeviceService _service;

        public DeviceServiceDomainTests()
        {
            _repositoryMock = new Mock<IGenericRepository<Device>>();
            _loggerMock = new Mock<ILogger<DeviceService>>();
            _service = new DeviceService(_repositoryMock.Object, _loggerMock.Object);
        }

        #region Helpers

        private static Device CreateDevice(string name = "Device A", string brand = "BrandX", DeviceState state = DeviceState.Available)
        {
            var device = new Device(name, brand);
            device.UpdateState(state);

            return device;
        }

        private static DeviceDto CreateDto(string name = "Device A", string brand = "BrandX")
        {
            return new DeviceDto
            {
                Name = name,
                Brand = brand
            };
        }

        #endregion

        [Fact]
        public async Task CreateAsync_ShouldFail_WhenNameIsEmpty()
        {
            var dto = new DeviceDto { Name = "", Brand = "Apple" };

            var result = await _service.CreateAsync(dto);

            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Device>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_ShouldSucceed_WhenValid()
        {
            var dto = CreateDto("iPhone", "Apple");

            _repositoryMock
               .Setup(x => x.AddAsync(It.IsAny<Device>()))
               .Returns(Task.CompletedTask);

            var result = await _service.CreateAsync(dto);

            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.Created);
            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Device>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldNotChange_CreatedAt()
        {
            var existing = CreateDevice();
            var dto = new DeviceDto { Name = "NewName", Brand = existing.Brand };

            _repositoryMock.Setup(x => x.GetByIdAsync(existing.Id)).ReturnsAsync(existing);            

            var result = await _service.UpdateAsync(existing.Id, dto);

            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.OK);

            _repositoryMock.Verify(x => x.Update(It.Is<Device>(d =>
                d.CreatedAt == existing.CreatedAt &&
                d.Name == dto.Name
            )), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldFail_WhenDeviceInUse_AndNameOrBrandChanged()
        {
            var existing = CreateDevice(state: DeviceState.InUse, name: "OldName", brand: "OldBrand");
            var dto = new DeviceDto { Name = "NewName", Brand = "OldBrand" }; 

            _repositoryMock.Setup(x => x.GetByIdAsync(existing.Id)).ReturnsAsync(existing);

            var result = await _service.UpdateAsync(existing.Id, dto);

            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);            
            _repositoryMock.Verify(x => x.Update(It.IsAny<Device>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ShouldSucceed_WhenValidUpdate_OnNotInUse()
        {
            var existing = CreateDevice(state: DeviceState.Available, name: "OldName", brand: "OldBrand");
            var dto = new DeviceDto { Name = "NewName", Brand = "NewBrand" };

            _repositoryMock.Setup(x => x.GetByIdAsync(existing.Id)).ReturnsAsync(existing);            

            var result = await _service.UpdateAsync(existing.Id, dto);

            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            _repositoryMock.Verify(x => x.Update(It.Is<Device>(d =>
                d.Name == dto.Name && d.Brand == dto.Brand && d.CreatedAt == existing.CreatedAt
            )), Times.Once);
        }

        [Fact]
        public async Task GetById_ShouldReturnDevice_WhenExists()
        {
            var existing = CreateDevice();
            _repositoryMock.Setup(x => x.GetByIdAsync(existing.Id)).ReturnsAsync(existing);

            var result = await _service.GetByIdAsync(existing.Id);

            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Data.Should().NotBeNull();
            result.Data!.Id.Should().Be(existing.Id);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenMissing()
        {
            var id = Guid.NewGuid();
            _repositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync((Device?)null);

            var result = await _service.GetByIdAsync(id);

            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }   
    
        [Fact]
        public async Task Delete_ShouldFail_WhenDeviceInUse()
        {
            var existing = CreateDevice(state: DeviceState.InUse);
            _repositoryMock.Setup(x => x.GetByIdAsync(existing.Id)).ReturnsAsync(existing);

            var result = await _service.DeleteAsync(existing.Id);

            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            _repositoryMock.Verify(x => x.Delete(It.IsAny<Device>()), Times.Never);
        }

        [Fact]
        public async Task Delete_ShouldSucceed_WhenNotInUse()
        {
            var existing = CreateDevice(state: DeviceState.Available);
            _repositoryMock.Setup(x => x.GetByIdAsync(existing.Id)).ReturnsAsync(existing);            

            var result = await _service.DeleteAsync(existing.Id);

            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            _repositoryMock.Verify(x => x.Delete(It.Is<Device>(d => d.Id == existing.Id)), Times.Once);
        }
    }
}
