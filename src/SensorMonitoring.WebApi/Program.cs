using Microsoft.OpenApi.Models;
using SensorMonitoring.Application.DTOs;
using SensorMonitoring.Application.Services;
using SensorMonitoring.Domain.Enums;
using SensorMonitoring.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Sensor Monitoring IoT API (.NET 8)",
        Version = "v1",
        Description = "API RESTful bajo Clean Architecture para ingesta de telemetría IoT, evaluación de umbrales críticos y detección de anomalías.",
        Contact = new OpenApiContact
        {
            Name = "Gustavo López Cosme",
            Email = "glopezc@uni.pe",
            Url = new Uri("https://github.com/Gus7Ghost")
        }
    });
});

// Registrar capa de infraestructura y aplicación
builder.Services.AddInfrastructureServices();

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Seed initial data for quick testing in Swagger UI
using (var scope = app.Services.CreateScope())
{
    var deviceService = scope.ServiceProvider.GetRequiredService<IDeviceService>();
    var telemetryService = scope.ServiceProvider.GetRequiredService<ITelemetryService>();

    // Seed devices
    await deviceService.RegisterDeviceAsync(new RegisterDeviceDto(
        "SENS-TEMP-101",
        "Sensor de Temperatura de Turbina 1",
        "Sala de Máquinas - Bloque A",
        SensorType.Temperature
    ));

    await deviceService.RegisterDeviceAsync(new RegisterDeviceDto(
        "SENS-VIB-202",
        "Sensor de Vibración Eje Central",
        "Turbina Hidráulica Principal",
        SensorType.Vibration
    ));

    // Seed sample telemetry
    await telemetryService.RecordTelemetryAsync(new RecordTelemetryDto(
        "SENS-TEMP-101",
        45.2,
        "Celsius"
    ));
    await telemetryService.RecordTelemetryAsync(new RecordTelemetryDto(
        "SENS-TEMP-101",
        92.4, // Anomaly! > 75.0
        "Celsius"
    ));
    await telemetryService.RecordTelemetryAsync(new RecordTelemetryDto(
        "SENS-VIB-202",
        4.8,
        "mm/s"
    ));
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sensor Monitoring API v1");
    c.RoutePrefix = string.Empty; // Swagger UI served at root '/'
});

app.UseCors("AllowAll");

app.MapControllers();

app.Run();
