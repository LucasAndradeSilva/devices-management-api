using Devices.Domain.Enums;

namespace Devices.Domain.Entities;

public class Device
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Brand { get; private set; }
    public DeviceState State { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime LastUpdatedAt { get; private set; }

    public Device(string name, string brand)
    {
        Id = Guid.NewGuid();
        Name = name;
        Brand = brand;
        State = DeviceState.Available;
        CreatedAt = DateTime.UtcNow;        

        Validate();
    }

    public void Update(string name, string brand)
    {
        if (State == DeviceState.InUse)
            throw new InvalidOperationException("Cannot update name or brand when device is in use.");

        Name = name;
        Brand = brand;
        LastUpdatedAt = DateTime.UtcNow;

        Validate();
    }

    public void UpdateState(DeviceState state)
    {
        State = state;
    }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Name is required.");

        if (string.IsNullOrWhiteSpace(Brand))
            throw new ArgumentException("Brand is required.");
    }
}