using System;
using SensorMonitoring.Domain.Entities;
using SensorMonitoring.Domain.Enums;

namespace SensorMonitoring.Application.DTOs;

public record DeviceResponseDto(
    Guid Id,
    string DeviceCode,
    string Name,
    string Location,
    string Type,
    string Status,
    DateTime RegisteredAt,
    DateTime? LastHeartbeat
)
{
    public static DeviceResponseDto FromDomain(SensorDevice d) =>
        new(d.Id, d.DeviceCode, d.Name, d.Location, d.Type.ToString(), d.Status.ToString(), d.RegisteredAt, d.LastHeartbeat);
}
