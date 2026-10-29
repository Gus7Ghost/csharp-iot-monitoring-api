using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SensorMonitoring.Application.DTOs;
using SensorMonitoring.Domain.Entities;
using SensorMonitoring.Domain.Enums;
using SensorMonitoring.Domain.Exceptions;
using SensorMonitoring.Domain.Repositories;

namespace SensorMonitoring.Application.Services;

public class TelemetryService : ITelemetryService
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly ITelemetryRepository _telemetryRepository;

    public TelemetryService(IDeviceRepository deviceRepository, ITelemetryRepository telemetryRepository)
    {
        _deviceRepository = deviceRepository;
        _telemetryRepository = telemetryRepository;
    }

    public async Task<TelemetryResponseDto> RecordTelemetryAsync(RecordTelemetryDto dto)
    {
        var device = await _deviceRepository.GetByCodeAsync(dto.DeviceCode)
            ?? throw new DeviceNotFoundException(dto.DeviceCode);

        // Reglas de negocio para detección de anomalías
        bool isAnomaly = false;
        string? alert = null;

        switch (device.Type)
        {
            case SensorType.Temperature when dto.Value > 75.0:
                isAnomaly = true;
                alert = $"Temperatura crítica ({dto.Value:F1} {dto.Unit}) supera el límite de 75°C";
                break;
            case SensorType.Vibration when dto.Value > 8.5:
                isAnomaly = true;
                alert = $"Vibración anómala ({dto.Value:F1} {dto.Unit}) detectada";
                break;
            case SensorType.BatteryVoltage when dto.Value < 10.5:
                isAnomaly = true;
                alert = $"Voltaje de batería bajo ({dto.Value:F1} {dto.Unit})";
                break;
        }

        // Actualizar estado del dispositivo
        device.UpdateHeartbeat();
        device.SetStatus(isAnomaly ? DeviceStatus.Warning : DeviceStatus.Online);
        await _deviceRepository.UpdateAsync(device);

        var reading = new TelemetryReading(device.Id, dto.Value, dto.Unit, isAnomaly, alert);
        var created = await _telemetryRepository.AddAsync(reading);

        return TelemetryResponseDto.FromDomain(created);
    }

    public async Task<IEnumerable<TelemetryResponseDto>> GetRecentReadingsAsync(string deviceCode, int limit = 50)
    {
        var device = await _deviceRepository.GetByCodeAsync(deviceCode)
            ?? throw new DeviceNotFoundException(deviceCode);

        var readings = await _telemetryRepository.GetByDeviceIdAsync(device.Id, limit);
        return readings.Select(TelemetryResponseDto.FromDomain);
    }

    public async Task<IEnumerable<TelemetryResponseDto>> GetRecentAnomaliesAsync(int limit = 20)
    {
        var anomalies = await _telemetryRepository.GetAnomaliesAsync(limit);
        return anomalies.Select(TelemetryResponseDto.FromDomain);
    }

    public async Task<DeviceStatsDto> GetDeviceStatsAsync(string deviceCode)
    {
        var device = await _deviceRepository.GetByCodeAsync(deviceCode)
            ?? throw new DeviceNotFoundException(deviceCode);

        var readings = await _telemetryRepository.GetByDeviceIdAsync(device.Id, 100);
        var readingsList = readings.ToList();
        
        var avgLastHour = await _telemetryRepository.GetAverageValueAsync(device.Id, TimeSpan.FromHours(1));
        bool hasRecentAnomaly = readingsList.Take(10).Any(r => r.IsAnomaly);

        return new DeviceStatsDto(
            device.DeviceCode,
            device.Name,
            device.Status.ToString(),
            Math.Round(avgLastHour, 2),
            readingsList.Count,
            hasRecentAnomaly
        );
    }
}
