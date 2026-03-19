using System.Net;
using Devices.Application.Common;
using Devices.Application.DTOs;
using Devices.Application.Interfaces;
using Devices.Domain.Entities;
using Devices.Domain.Enums;

namespace Devices.Application.Services;

public class DeviceService : IDeviceService
{
    private readonly IGenericRepository<Device> _repository;

    public DeviceService(IGenericRepository<Device> repository)
    {
        _repository = repository;
    }

    public async Task<Result<DeviceResponseDto>> CreateAsync(DeviceDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return Result<DeviceResponseDto>.Failure("Name is required", HttpStatusCode.BadRequest, new[] { "invalid_name" });

            if (string.IsNullOrWhiteSpace(dto.Brand))
                return Result<DeviceResponseDto>.Failure("Brand is required", HttpStatusCode.BadRequest,new[] { "invalid_brand" });

            var device = new Device(dto.Name, dto.Brand);

            await _repository.AddAsync(device);
            await _repository.SaveChangesAsync();

            return Result<DeviceResponseDto>.Success(
                MapToDto(device),
                "Device created successfully",
                HttpStatusCode.Created);
        }
        catch (Exception)
        {
            return Result<DeviceResponseDto>.Failure("Unexpected error occurred", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Result<IEnumerable<DeviceResponseDto>>> GetAllAsync(int page = 1, int pageSize = 10)
    {
        try
        {
            var devices = await _repository.GetAsync(
                page: page,
                pageSize: pageSize,
                asNoTracking: true);

            return Result<IEnumerable<DeviceResponseDto>>.Success(
                devices.Select(MapToDto));
        }
        catch (Exception)
        {
            return Result<IEnumerable<DeviceResponseDto>>.Failure(
                "Unexpected error occurred",
                HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Result<DeviceResponseDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var device = await _repository.GetByIdAsync(id);

            if (device == null)
                return Result<DeviceResponseDto>.Failure("Device not found", HttpStatusCode.NotFound, new[] { "device_not_found" });

            return Result<DeviceResponseDto>.Success(MapToDto(device));
        }
        catch (Exception)
        {
            return Result<DeviceResponseDto>.Failure(
                "Unexpected error occurred",
                HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Result<bool>> UpdateAsync(Guid id, DeviceDto dto)
    {
        try
        {
            var device = await _repository.GetByIdAsync(id);

            if (device == null)
                return Result<bool>.Failure("Device not found", HttpStatusCode.NotFound, new[] { "device_not_found" });

            if (device.State == DeviceState.InUse)
                return Result<bool>.Failure("Device is in use", HttpStatusCode.BadRequest, new[] { "device_in_use" });

            if (string.IsNullOrWhiteSpace(dto.Name))
                return Result<bool>.Failure("Name is required", HttpStatusCode.BadRequest, new[] { "invalid_name" });

            if (string.IsNullOrWhiteSpace(dto.Brand))
                return Result<bool>.Failure("Brand is required", HttpStatusCode.BadRequest, new[] { "invalid_brand" });

            device.Update(dto.Name, dto.Brand);

            _repository.Update(device);
            await _repository.SaveChangesAsync();

            return Result<bool>.Success(true, "Device updated successfully");
        }
        catch (Exception)
        {
            return Result<bool>.Failure(
                "Unexpected error occurred",
                HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        try
        {
            var device = await _repository.GetByIdAsync(id);

            if (device == null)
                return Result<bool>.Failure("Device not found", HttpStatusCode.NotFound, new[] { "device_not_found" });

            if (device.State == DeviceState.InUse)
                return Result<bool>.Failure("Device is in use", HttpStatusCode.BadRequest, new[] { "device_in_use" });

            _repository.Delete(device);
            await _repository.SaveChangesAsync();

            return Result<bool>.Success(true, "Device deleted successfully");
        }
        catch (Exception)
        {
            return Result<bool>.Failure("Unexpected error occurred", HttpStatusCode.InternalServerError);
        }
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