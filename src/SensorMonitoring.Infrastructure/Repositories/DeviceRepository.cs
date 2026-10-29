using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SensorMonitoring.Domain.Entities;
using SensorMonitoring.Domain.Repositories;
using SensorMonitoring.Infrastructure.Persistence;

namespace SensorMonitoring.Infrastructure.Repositories;

public class DeviceRepository : IDeviceRepository
{
    private readonly SensorDbContext _context;

    public DeviceRepository(SensorDbContext context)
    {
        _context = context;
    }

    public async Task<SensorDevice?> GetByIdAsync(Guid id) =>
        await _context.Devices.FindAsync(id);

    public async Task<SensorDevice?> GetByCodeAsync(string deviceCode) =>
        await _context.Devices.FirstOrDefaultAsync(d => d.DeviceCode == deviceCode.ToUpperInvariant());

    public async Task<IEnumerable<SensorDevice>> GetAllAsync() =>
        await _context.Devices.AsNoTracking().ToListAsync();

    public async Task<SensorDevice> AddAsync(SensorDevice device)
    {
        await _context.Devices.AddAsync(device);
        await _context.SaveChangesAsync();
        return device;
    }

    public async Task UpdateAsync(SensorDevice device)
    {
        _context.Devices.Update(device);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(string deviceCode) =>
        await _context.Devices.AnyAsync(d => d.DeviceCode == deviceCode.ToUpperInvariant());
}
