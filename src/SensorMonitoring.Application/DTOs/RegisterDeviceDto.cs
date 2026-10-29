using System.ComponentModel.DataAnnotations;
using SensorMonitoring.Domain.Enums;

namespace SensorMonitoring.Application.DTOs;

public record RegisterDeviceDto(
    [Required(ErrorMessage = "El código del dispositivo es obligatorio")]
    string DeviceCode,

    [Required(ErrorMessage = "El nombre del dispositivo es obligatorio")]
    string Name,

    string Location,

    [Required(ErrorMessage = "El tipo de sensor es obligatorio")]
    SensorType Type
);
