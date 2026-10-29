using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using SensorMonitoring.Application.DTOs;
using SensorMonitoring.Application.Services;
using SensorMonitoring.Domain.Entities;
using SensorMonitoring.Domain.Enums;
using SensorMonitoring.Domain.Exceptions;
using SensorMonitoring.Domain.Repositories;
using Xunit;

namespace SensorMonitoring.Tests.Services;

public class TelemetryServiceTests
{
    private readonly Mock<IDeviceRepository> _deviceRepoMock;
    private readonly Mock<ITelemetryRepository> _telemetryRepoMock;
    private readonly TelemetryService _service;

    public TelemetryServiceTests()
    {
        _deviceRepoMock = new Mock<IDeviceRepository>();
        _telemetryRepoMock = new Mock<ITelemetryRepository>();
        _service = new TelemetryService(_deviceRepoMock.Object, _telemetryRepoMock.Object);
    }

    [Fact]
    public async Task RecordTelemetry_WhenValueIsNormal_ShouldNotFlagAnomaly()
    {
        // Arrange
        var device = new SensorDevice("TEMP-01", "Sensor 1", "Room A", SensorType.Temperature);
        _deviceRepoMock.Setup(r => r.GetByCodeAsync("TEMP-01")).ReturnsAsync(device);
        _telemetryRepoMock.Setup(r => r.AddAsync(It.IsAny<TelemetryReading>()))
            .ReturnsAsync((TelemetryReading t) => t);

        var dto = new RecordTelemetryDto("TEMP-01", 50.0, "Celsius");

        // Act
        var result = await _service.RecordTelemetryAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.IsAnomaly.Should().BeFalse();
        result.AlertMessage.Should().BeNull();
        device.Status.Should().Be(DeviceStatus.Online);
        _deviceRepoMock.Verify(r => r.UpdateAsync(device), Times.Once);
    }

    [Fact]
    public async Task RecordTelemetry_WhenTemperatureExceedsThreshold_ShouldFlagAnomaly()
    {
        // Arrange
        var device = new SensorDevice("TEMP-01", "Sensor 1", "Room A", SensorType.Temperature);
        _deviceRepoMock.Setup(r => r.GetByCodeAsync("TEMP-01")).ReturnsAsync(device);
        _telemetryRepoMock.Setup(r => r.AddAsync(It.IsAny<TelemetryReading>()))
            .ReturnsAsync((TelemetryReading t) => t);

        var dto = new RecordTelemetryDto("TEMP-01", 82.5, "Celsius");

        // Act
        var result = await _service.RecordTelemetryAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.IsAnomaly.Should().BeTrue();
        result.AlertMessage.Should().Contain("supera el límite de 75°C");
        device.Status.Should().Be(DeviceStatus.Warning);
    }

    [Fact]
    public async Task RecordTelemetry_WhenDeviceDoesNotExist_ShouldThrowDeviceNotFoundException()
    {
        // Arrange
        _deviceRepoMock.Setup(r => r.GetByCodeAsync("UNKNOWN")).ReturnsAsync((SensorDevice?)null);
        var dto = new RecordTelemetryDto("UNKNOWN", 25.0, "Celsius");

        // Act
        Func<Task> act = async () => await _service.RecordTelemetryAsync(dto);

        // Assert
        await act.Should().ThrowAsync<DeviceNotFoundException>();
    }

    [Fact]
    public async Task GetDeviceStats_ShouldAggregateMetrics()
    {
        // Arrange
        var device = new SensorDevice("VIB-01", "Vibration Sensor", "Sector B", SensorType.Vibration);
        _deviceRepoMock.Setup(r => r.GetByCodeAsync("VIB-01")).ReturnsAsync(device);

        var readings = new List<TelemetryReading>
        {
            new TelemetryReading(device.Id, 4.2, "mm/s", false),
            new TelemetryReading(device.Id, 9.8, "mm/s", true, "High vibration")
        };

        _telemetryRepoMock.Setup(r => r.GetByDeviceIdAsync(device.Id, 100)).ReturnsAsync(readings);
        _telemetryRepoMock.Setup(r => r.GetAverageValueAsync(device.Id, It.IsAny<TimeSpan>())).ReturnsAsync(7.0);

        // Act
        var stats = await _service.GetDeviceStatsAsync("VIB-01");

        // Assert
        stats.Should().NotBeNull();
        stats.DeviceCode.Should().Be("VIB-01");
        stats.TotalReadings.Should().Be(2);
        stats.HasRecentAnomaly.Should().BeTrue();
        stats.AverageValueLastHour.Should().Be(7.0);
    }
}
