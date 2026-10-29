using System;

namespace SensorMonitoring.Domain.Exceptions;

public class DeviceNotFoundException : Exception
{
    public DeviceNotFoundException(string message) : base(message) { }
    public DeviceNotFoundException(string code, Exception? inner = null) 
        : base($"Dispositivo sensor no encontrado con identificador: {code}", inner) { }
}
