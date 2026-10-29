using System;

namespace SensorMonitoring.Domain.Entities;

public class TelemetryReading
{
    public long Id { get; private set; }
    public Guid DeviceId { get; private set; }
    public double Value { get; private set; }
    public string Unit { get; private set; }
    public DateTime Timestamp { get; private set; }
    public bool IsAnomaly { get; private set; }
    public string? AlertMessage { get; private set; }

    private TelemetryReading() { }

    public TelemetryReading(Guid deviceId, double value, string unit, bool isAnomaly = false, string? alertMessage = null)
    {
        DeviceId = deviceId;
        Value = value;
        Unit = unit ?? string.Empty;
        Timestamp = DateTime.UtcNow;
        IsAnomaly = isAnomaly;
        AlertMessage = alertMessage;
    }
}
