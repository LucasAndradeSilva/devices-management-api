using Devices.Application.Common;
using Devices.Application.DTOs;
using Devices.Application.Interfaces;
using Devices.Domain.Entities;
using Devices.Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using System.Net;

namespace Devices.Application.Services;

public class DeviceService : IDeviceService
{
    private readonly IGenericRepository<Device> _repository;
    private readonly ILogger<DeviceService> _logger;

    public DeviceService(IGenericRepository<Device> repository, ILogger<DeviceService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<DeviceResponseDto>> CreateAsync(DeviceDto dto)
    {
        try
        {
            _logger.LogInformation("Creating device {@Device}", dto);

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                _logger.LogWarning("Device creation failed: Name is required");
                return Result<DeviceResponseDto>.Failure("Name is required", HttpStatusCode.BadRequest, new[] { "invalid_name" });
            }

            if (string.IsNullOrWhiteSpace(dto.Brand))
            {
                _logger.LogWarning("Device creation failed: Brand is required");
                return Result<DeviceResponseDto>.Failure("Brand is required", HttpStatusCode.BadRequest, new[] { "invalid_brand" });
            }

            var device = new Device(dto.Name, dto.Brand);

            await _repository.AddAsync(device);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Device created successfully with Id {DeviceId}", device.Id);

            return Result<DeviceResponseDto>.Success(
                MapToDto(device),
                "Device created successfully",
                HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while creating device");
            return Result<DeviceResponseDto>.Failure("Unexpected error occurred", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Result<IEnumerable<DeviceResponseDto>>> GetAllAsync(
        int page = 1,
        int pageSize = 10,
        string? brand = null,
        DeviceState? state = null)
    {
        try
        {
            _logger.LogInformation(
                "Fetching devices Page: {Page}, PageSize: {PageSize}, Brand: {Brand}, State: {State}",
                page, pageSize, brand, state);

            Expression<Func<Device, bool>>? filter = null;

            if (!string.IsNullOrEmpty(brand) && state.HasValue)
            {
                filter = x => x.Brand == brand && x.State == state.Value;
            }
            else if (!string.IsNullOrEmpty(brand))
            {
                filter = x => x.Brand == brand;
            }
            else if (state.HasValue)
            {
                filter = x => x.State == state.Value;
            }

            var devices = await _repository.GetAsync(
                filter: filter,
                page: page,
                pageSize: pageSize,
                asNoTracking: true);

            _logger.LogInformation("Fetched {Count} devices", devices.Count());

            return Result<IEnumerable<DeviceResponseDto>>.Success(
                devices.Select(MapToDto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching devices");

            return Result<IEnumerable<DeviceResponseDto>>.Failure(
                "Unexpected error occurred",
                HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Result<DeviceResponseDto>> GetByIdAsync(Guid id)
    {
        try
        {
            _logger.LogInformation("Fetching device with Id {DeviceId}", id);

            var device = await _repository.GetByIdAsync(id);

            if (device == null)
            {
                _logger.LogWarning("Device not found {DeviceId}", id);
                return Result<DeviceResponseDto>.Failure("Device not found", HttpStatusCode.NotFound, new[] { "device_not_found" });
            }

            _logger.LogInformation("Device found {DeviceId}", id);

            return Result<DeviceResponseDto>.Success(MapToDto(device));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching device {DeviceId}", id);
            return Result<DeviceResponseDto>.Failure(
                "Unexpected error occurred",
                HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Result<bool>> UpdateAsync(Guid id, DeviceDto dto)
    {
        try
        {
            _logger.LogInformation("Updating device {DeviceId} with data {@Device}", id, dto);

            var device = await _repository.GetByIdAsync(id);

            if (device == null)
            {
                _logger.LogWarning("Update failed: Device not found {DeviceId}", id);

                return Result<bool>.Failure(
                    "Device not found",
                    HttpStatusCode.NotFound,
                    new[] { "device_not_found" });
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(dto.Name))
                    device.UpdateName(dto.Name);

                if (!string.IsNullOrWhiteSpace(dto.Brand))
                    device.UpdateBrand(dto.Brand);

                if (dto.State.HasValue)
                    device.UpdateState(dto.State.Value);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Business rule violation on update {DeviceId}", id);

                return Result<bool>.Failure(
                    ex.Message,
                    HttpStatusCode.BadRequest,
                    new[] { "business_rule_violation" });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error on update {DeviceId}", id);

                return Result<bool>.Failure(
                    ex.Message,
                    HttpStatusCode.BadRequest,
                    new[] { "validation_error" });
            }

            _repository.Update(device);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Device updated successfully {DeviceId}", id);

            return Result<bool>.Success(true, "Device updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while updating device {DeviceId}", id);

            return Result<bool>.Failure(
                "Unexpected error occurred",
                HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        try
        {
            _logger.LogInformation("Deleting device {DeviceId}", id);

            var device = await _repository.GetByIdAsync(id);

            if (device == null)
            {
                _logger.LogWarning("Delete failed: Device not found {DeviceId}", id);
                return Result<bool>.Failure("Device not found", HttpStatusCode.NotFound, new[] { "device_not_found" });
            }

            if (device.State == DeviceState.InUse)
            {
                _logger.LogWarning("Delete failed: Device is in use {DeviceId}", id);
                return Result<bool>.Failure("Device is in use", HttpStatusCode.BadRequest, new[] { "device_in_use" });
            }

            _repository.Delete(device);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Device deleted successfully {DeviceId}", id);

            return Result<bool>.Success(true, "Device deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while deleting device {DeviceId}", id);
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
            CreatedAt = device.CreatedAt,
            LastUpdatedAt = device.LastUpdatedAt
        };
    }
}