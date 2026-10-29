using System.Collections.Generic;
using System.Threading.Tasks;
using SensorMonitoring.Application.DTOs;

namespace SensorMonitoring.Application.Services;

public interface IDeviceService
{
    Task<DeviceResponseDto> RegisterDeviceAsync(RegisterDeviceDto dto);
    Task<DeviceResponseDto> GetByCodeAsync(string deviceCode);
    Task<IEnumerable<DeviceResponseDto>> GetAllAsync();
}
