namespace SensorMonitoring.Application.DTOs;

public record DeviceStatsDto(
    string DeviceCode,
    string Name,
    string Status,
    double AverageValueLastHour,
    int TotalReadings,
    bool HasRecentAnomaly
);
