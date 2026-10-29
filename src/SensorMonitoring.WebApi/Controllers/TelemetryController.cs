using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SensorMonitoring.Application.DTOs;
using SensorMonitoring.Application.Services;
using SensorMonitoring.Domain.Exceptions;

namespace SensorMonitoring.WebApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class TelemetryController : ControllerBase
{
    private readonly ITelemetryService _telemetryService;

    public TelemetryController(ITelemetryService telemetryService)
    {
        _telemetryService = telemetryService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(TelemetryResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Record([FromBody] RecordTelemetryDto dto)
    {
        try
        {
            var result = await _telemetryService.RecordTelemetryAsync(dto);
            return StatusCode(StatusCodes.Status201Created, result);
        }
        catch (DeviceNotFoundException ex)
        {
            return NotFound(new { title = "Dispositivo No Encontrado", detail = ex.Message });
        }
    }

    [HttpGet("readings/{deviceCode}")]
    [ProducesResponseType(typeof(IEnumerable<TelemetryResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRecent(string deviceCode, [FromQuery] int limit = 50)
    {
        try
        {
            var result = await _telemetryService.GetRecentReadingsAsync(deviceCode, limit);
            return Ok(result);
        }
        catch (DeviceNotFoundException ex)
        {
            return NotFound(new { title = "Dispositivo No Encontrado", detail = ex.Message });
        }
    }

    [HttpGet("anomalies")]
    [ProducesResponseType(typeof(IEnumerable<TelemetryResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAnomalies([FromQuery] int limit = 20)
    {
        var result = await _telemetryService.GetRecentAnomaliesAsync(limit);
        return Ok(result);
    }

    [HttpGet("stats/{deviceCode}")]
    [ProducesResponseType(typeof(DeviceStatsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStats(string deviceCode)
    {
        try
        {
            var result = await _telemetryService.GetDeviceStatsAsync(deviceCode);
            return Ok(result);
        }
        catch (DeviceNotFoundException ex)
        {
            return NotFound(new { title = "Dispositivo No Encontrado", detail = ex.Message });
        }
    }
}
