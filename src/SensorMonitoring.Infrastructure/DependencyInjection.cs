using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SensorMonitoring.Application.Services;
using SensorMonitoring.Domain.Repositories;
using SensorMonitoring.Infrastructure.Persistence;
using SensorMonitoring.Infrastructure.Repositories;

namespace SensorMonitoring.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddDbContext<SensorDbContext>(options =>
            options.UseInMemoryDatabase("SensorMonitoringDb"));

        services.AddScoped<IDeviceRepository, DeviceRepository>();
        services.AddScoped<ITelemetryRepository, TelemetryRepository>();
        services.AddScoped<IDeviceService, DeviceService>();
        services.AddScoped<ITelemetryService, TelemetryService>();

        return services;
    }
}
