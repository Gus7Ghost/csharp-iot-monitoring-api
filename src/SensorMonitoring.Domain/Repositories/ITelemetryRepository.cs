using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SensorMonitoring.Domain.Entities;

namespace SensorMonitoring.Domain.Repositories;

public interface ITelemetryRepository
{
    Task<TelemetryReading> AddAsync(TelemetryReading reading);
    Task<IEnumerable<TelemetryReading>> GetByDeviceIdAsync(Guid deviceId, int limit = 50);
    Task<IEnumerable<TelemetryReading>> GetAnomaliesAsync(int limit = 20);
    Task<double> GetAverageValueAsync(Guid deviceId, TimeSpan window);
}
