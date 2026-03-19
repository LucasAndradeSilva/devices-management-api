using System.Net;
using Devices.Application.Common;
using Devices.Application.DTOs;
using Devices.Domain.Entities;
using Devices.Domain.Enums;

namespace Devices.Application.Services;

public class DeviceService : IDeviceService
{
    private static readonly List<Device> _devices = new();

    public Task<Result<DeviceResponseDto>> CreateAsync(DeviceDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return Task.FromResult(Result<DeviceResponseDto>.Failure(
                "Name is required",
                HttpStatusCode.BadRequest,
                new[] { "invalid_name" }));

        if (string.IsNullOrWhiteSpace(dto.Brand))
            return Task.FromResult(Result<DeviceResponseDto>.Failure(
                "Brand is required",
                HttpStatusCode.BadRequest,
                new[] { "invalid_brand" }));

        var device = new Device(dto.Name, dto.Brand);

        _devices.Add(device);

        return Task.FromResult(Result<DeviceResponseDto>.Success(
            MapToDto(device),
            "Device created successfully",
            HttpStatusCode.Created));
    }

    public Task<Result<IEnumerable<DeviceResponseDto>>> GetAllAsync()
    {
        var result = _devices.Select(MapToDto);

        return Task.FromResult(Result<IEnumerable<DeviceResponseDto>>.Success(result));
    }

    public Task<Result<DeviceResponseDto>> GetByIdAsync(Guid id)
    {
        var device = _devices.FirstOrDefault(x => x.Id == id);

        if (device == null)
            return Task.FromResult(Result<DeviceResponseDto>.Failure(
                "Device not found",
                HttpStatusCode.NotFound,
                new[] { "device_not_found" }));

        return Task.FromResult(Result<DeviceResponseDto>.Success(MapToDto(device)));
    }

    public Task<Result<bool>> UpdateAsync(Guid id, DeviceDto dto)
    {
        var device = _devices.FirstOrDefault(x => x.Id == id);

        if (device == null)
            return Task.FromResult(Result<bool>.Failure(
                "Device not found",
                HttpStatusCode.NotFound,
                new[] { "device_not_found" }));

        if (device.State == DeviceState.InUse)
            return Task.FromResult(Result<bool>.Failure(
                "Device is in use",
                HttpStatusCode.BadRequest,
                new[] { "device_in_use" }));

        if (string.IsNullOrWhiteSpace(dto.Name))
            return Task.FromResult(Result<bool>.Failure(
                "Name is required",
                HttpStatusCode.BadRequest,
                new[] { "invalid_name" }));

        if (string.IsNullOrWhiteSpace(dto.Brand))
            return Task.FromResult(Result<bool>.Failure(
                "Brand is required",
                HttpStatusCode.BadRequest,
                new[] { "invalid_brand" }));

        device.Update(dto.Name, dto.Brand);

        return Task.FromResult(Result<bool>.Success(
            true,
            "Device updated successfully"));
    }

    public Task<Result<bool>> DeleteAsync(Guid id)
    {
        var device = _devices.FirstOrDefault(x => x.Id == id);

        if (device == null)
            return Task.FromResult(Result<bool>.Failure(
                "Device not found",
                HttpStatusCode.NotFound,
                new[] { "device_not_found" }));

        if (device.State == DeviceState.InUse)
            return Task.FromResult(Result<bool>.Failure(
                "Device is in use",
                HttpStatusCode.BadRequest,
                new[] { "device_in_use" }));

        _devices.Remove(device);

        return Task.FromResult(Result<bool>.Success(
            true,
            "Device deleted successfully"));
    }

    private DeviceResponseDto MapToDto(Device device)
    {
        return new DeviceResponseDto
        {
            Id = device.Id,
            Name = device.Name,
            Brand = device.Brand,
            State = device.State.ToString(),
            CreatedAt = device.CreatedAt
        };
    }
}