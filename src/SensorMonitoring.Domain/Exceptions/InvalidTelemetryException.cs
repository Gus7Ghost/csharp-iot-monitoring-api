using System;

namespace SensorMonitoring.Domain.Exceptions;

public class InvalidTelemetryException : Exception
{
    public InvalidTelemetryException(string message) : base(message) { }
}
