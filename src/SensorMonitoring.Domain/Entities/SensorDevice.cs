using System;
using SensorMonitoring.Domain.Enums;

namespace SensorMonitoring.Domain.Entities;

public class SensorDevice
{
    public Guid Id { get; private set; }
    public string DeviceCode { get; private set; }
    public string Name { get; private set; }
    public string Location { get; private set; }
    public SensorType Type { get; private set; }
    public DeviceStatus Status { get; private set; }
    public DateTime RegisteredAt { get; private set; }
    public DateTime? LastHeartbeat { get; private set; }

    // Constructor privado para EF Core
    private SensorDevice() { }

    public SensorDevice(string deviceCode, string name, string location, SensorType type)
    {
        if (string.IsNullOrWhiteSpace(deviceCode))
            throw new ArgumentException("El código del dispositivo no puede ser vacío", nameof(deviceCode));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del dispositivo no puede ser vacío", nameof(name));

        Id = Guid.NewGuid();
        DeviceCode = deviceCode.Trim().ToUpperInvariant();
        Name = name.Trim();
        Location = location?.Trim() ?? "General";
        Type = type;
        Status = DeviceStatus.Online;
        RegisteredAt = DateTime.UtcNow;
        LastHeartbeat = DateTime.UtcNow;
    }

    public void UpdateHeartbeat()
    {
        LastHeartbeat = DateTime.UtcNow;
    }

    public void SetStatus(DeviceStatus newStatus)
    {
        Status = newStatus;
    }
}
