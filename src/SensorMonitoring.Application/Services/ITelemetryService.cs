using System.Collections.Generic;
using System.Threading.Tasks;
using SensorMonitoring.Application.DTOs;

namespace SensorMonitoring.Application.Services;

public interface ITelemetryService
{
    Task<TelemetryResponseDto> RecordTelemetryAsync(RecordTelemetryDto dto);
    Task<IEnumerable<TelemetryResponseDto>> GetRecentReadingsAsync(string deviceCode, int limit = 50);
    Task<IEnumerable<TelemetryResponseDto>> GetRecentAnomaliesAsync(int limit = 20);
    Task<DeviceStatsDto> GetDeviceStatsAsync(string deviceCode);
}
