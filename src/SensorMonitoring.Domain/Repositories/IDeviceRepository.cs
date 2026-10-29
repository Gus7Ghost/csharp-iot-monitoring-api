using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SensorMonitoring.Domain.Entities;

namespace SensorMonitoring.Domain.Repositories;

public interface IDeviceRepository
{
    Task<SensorDevice?> GetByIdAsync(Guid id);
    Task<SensorDevice?> GetByCodeAsync(string deviceCode);
    Task<IEnumerable<SensorDevice>> GetAllAsync();
    Task<SensorDevice> AddAsync(SensorDevice device);
    Task UpdateAsync(SensorDevice device);
    Task<bool> ExistsAsync(string deviceCode);
}
