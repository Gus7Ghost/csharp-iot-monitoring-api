using System.ComponentModel.DataAnnotations;

namespace SensorMonitoring.Application.DTOs;

public record RecordTelemetryDto(
    [Required(ErrorMessage = "El código del dispositivo es obligatorio")]
    string DeviceCode,

    [Required(ErrorMessage = "El valor de telemetría es obligatorio")]
    double Value,

    string Unit
);
