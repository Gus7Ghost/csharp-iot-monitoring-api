using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SensorMonitoring.Application.DTOs;
using SensorMonitoring.Domain.Entities;
using SensorMonitoring.Domain.Exceptions;
using SensorMonitoring.Domain.Repositories;

namespace SensorMonitoring.Application.Services;

public class DeviceService : IDeviceService
{
    private readonly IDeviceRepository _deviceRepository;

    public DeviceService(IDeviceRepository deviceRepository)
    {
        _deviceRepository = deviceRepository;
    }

    public async Task<DeviceResponseDto> RegisterDeviceAsync(RegisterDeviceDto dto)
    {
        if (await _deviceRepository.ExistsAsync(dto.DeviceCode))
        {
            throw new InvalidOperationException($"El dispositivo con código '{dto.DeviceCode}' ya se encuentra registrado");
        }

        var device = new SensorDevice(dto.DeviceCode, dto.Name, dto.Location, dto.Type);
        var created = await _deviceRepository.AddAsync(device);
        return DeviceResponseDto.FromDomain(created);
    }

    public async Task<DeviceResponseDto> GetByCodeAsync(string deviceCode)
    {
        var device = await _deviceRepository.GetByCodeAsync(deviceCode)
            ?? throw new DeviceNotFoundException(deviceCode);

        return DeviceResponseDto.FromDomain(device);
    }

    public async Task<IEnumerable<DeviceResponseDto>> GetAllAsync()
    {
        var devices = await _deviceRepository.GetAllAsync();
        return devices.Select(DeviceResponseDto.FromDomain);
    }
}
