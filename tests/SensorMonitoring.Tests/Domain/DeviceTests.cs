using System;
using FluentAssertions;
using SensorMonitoring.Domain.Entities;
using SensorMonitoring.Domain.Enums;
using Xunit;

namespace SensorMonitoring.Tests.Domain;

public class DeviceTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldInitializeCorrectly()
    {
        // Act
        var device = new SensorDevice("TEMP-01", "Sensor Temp", "Sector A", SensorType.Temperature);

        // Assert
        device.DeviceCode.Should().Be("TEMP-01");
        device.Name.Should().Be("Sensor Temp");
        device.Location.Should().Be("Sector A");
        device.Type.Should().Be(SensorType.Temperature);
        device.Status.Should().Be(DeviceStatus.Online);
        device.LastHeartbeat.Should().NotBeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithInvalidCode_ShouldThrowArgumentException(string? invalidCode)
    {
        // Act
        Action act = () => new SensorDevice(invalidCode!, "Valid Name", "Sector A", SensorType.Temperature);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void SetStatus_ShouldUpdateDeviceStatus()
    {
        // Arrange
        var device = new SensorDevice("TEMP-01", "Sensor Temp", "Sector A", SensorType.Temperature);

        // Act
        device.SetStatus(DeviceStatus.Warning);

        // Assert
        device.Status.Should().Be(DeviceStatus.Warning);
    }

    [Fact]
    public void UpdateHeartbeat_ShouldUpdateLastHeartbeat()
    {
        // Arrange
        var device = new SensorDevice("TEMP-01", "Sensor Temp", "Sector A", SensorType.Temperature);
        var initialHeartbeat = device.LastHeartbeat;

        // Act
        device.UpdateHeartbeat();

        // Assert
        device.LastHeartbeat.Should().BeOnOrAfter(initialHeartbeat!.Value);
    }
}
