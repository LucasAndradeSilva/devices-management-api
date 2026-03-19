using Devices.Application.Common;
using Devices.Application.DTOs;

public interface IDeviceService
{
    Task<Result<DeviceResponseDto>> CreateAsync(DeviceDto dto);
    Task<Result<DeviceResponseDto>> GetByIdAsync(Guid id);    
    Task<Result<bool>> UpdateAsync(Guid id, DeviceDto dto);
    Task<Result<bool>> DeleteAsync(Guid id);
}