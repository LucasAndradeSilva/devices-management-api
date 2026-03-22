using Devices.Domain.Enums;

namespace Devices.Application.DTOs;

public class DeviceDto
{
    /// <example>iPhone 15</example>
    public string? Name { get; set; }

    /// <example>Apple</example>
    public string? Brand { get; set; }

    public DeviceState? State { get; set; }
}