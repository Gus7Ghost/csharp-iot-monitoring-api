using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SensorMonitoring.Domain.Entities;
using SensorMonitoring.Domain.Repositories;
using SensorMonitoring.Infrastructure.Persistence;

namespace SensorMonitoring.Infrastructure.Repositories;

public class TelemetryRepository : ITelemetryRepository
{
    private readonly SensorDbContext _context;

    public TelemetryRepository(SensorDbContext context)
    {
        _context = context;
    }

    public async Task<TelemetryReading> AddAsync(TelemetryReading reading)
    {
        await _context.TelemetryReadings.AddAsync(reading);
        await _context.SaveChangesAsync();
        return reading;
    }

    public async Task<IEnumerable<TelemetryReading>> GetByDeviceIdAsync(Guid deviceId, int limit = 50) =>
        await _context.TelemetryReadings
            .AsNoTracking()
            .Where(t => t.DeviceId == deviceId)
            .OrderByDescending(t => t.Timestamp)
            .Take(limit)
            .ToListAsync();

    public async Task<IEnumerable<TelemetryReading>> GetAnomaliesAsync(int limit = 20) =>
        await _context.TelemetryReadings
            .AsNoTracking()
            .Where(t => t.IsAnomaly)
            .OrderByDescending(t => t.Timestamp)
            .Take(limit)
            .ToListAsync();

    public async Task<double> GetAverageValueAsync(Guid deviceId, TimeSpan window)
    {
        var cutoff = DateTime.UtcNow.Subtract(window);
        var query = _context.TelemetryReadings
            .Where(t => t.DeviceId == deviceId && t.Timestamp >= cutoff);

        if (!await query.AnyAsync()) return 0.0;
        return await query.AverageAsync(t => t.Value);
    }
}
