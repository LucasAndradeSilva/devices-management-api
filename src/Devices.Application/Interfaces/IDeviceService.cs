using Devices.Application.Common;
using Devices.Application.DTOs;

namespace Devices.Application.Interfaces;

public interface IDeviceService
{
    Task<Result<DeviceResponseDto>> CreateAsync(DeviceDto dto);
    Task<Result<DeviceResponseDto>> GetByIdAsync(Guid id);
    Task<Result<IEnumerable<DeviceResponseDto>>> GetAllAsync(int page, int pageSize);
    Task<Result<bool>> UpdateAsync(Guid id, DeviceDto dto);
    Task<Result<bool>> DeleteAsync(Guid id);
}