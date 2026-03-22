using Devices.Application.Common;
using Devices.Application.DTOs;
using Devices.Domain.Enums;

namespace Devices.Application.Interfaces;

public interface IDeviceService
{
    Task<Result<DeviceResponseDto>> CreateAsync(DeviceDto dto);
    Task<Result<DeviceResponseDto>> GetByIdAsync(Guid id);    
    Task<Result<IEnumerable<DeviceResponseDto>>> GetAllAsync(
        int page = 1,
        int pageSize = 10,
        string? brand = null,
        DeviceState? state = null);
    Task<Result<bool>> UpdateAsync(Guid id, DeviceDto dto);
    Task<Result<bool>> DeleteAsync(Guid id);
}