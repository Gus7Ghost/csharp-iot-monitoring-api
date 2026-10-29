using System;
using SensorMonitoring.Domain.Entities;

namespace SensorMonitoring.Application.DTOs;

public record TelemetryResponseDto(
    long Id,
    Guid DeviceId,
    double Value,
    string Unit,
    DateTime Timestamp,
    bool IsAnomaly,
    string? AlertMessage
)
{
    public static TelemetryResponseDto FromDomain(TelemetryReading t) =>
        new(t.Id, t.DeviceId, t.Value, t.Unit, t.Timestamp, t.IsAnomaly, t.AlertMessage);
}
