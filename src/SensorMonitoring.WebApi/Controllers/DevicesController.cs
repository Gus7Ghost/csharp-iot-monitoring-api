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
public class DevicesController : ControllerBase
{
    private readonly IDeviceService _deviceService;

    public DevicesController(IDeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(DeviceResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterDevice([FromBody] RegisterDeviceDto dto)
    {
        try
        {
            var result = await _deviceService.RegisterDeviceAsync(dto);
            return CreatedAtAction(nameof(GetByCode), new { deviceCode = result.DeviceCode }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { title = "Conflicto de Dispositivo", detail = ex.Message });
        }
    }

    [HttpGet("{deviceCode}")]
    [ProducesResponseType(typeof(DeviceResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(string deviceCode)
    {
        try
        {
            var result = await _deviceService.GetByCodeAsync(deviceCode);
            return Ok(result);
        }
        catch (DeviceNotFoundException ex)
        {
            return NotFound(new { title = "Dispositivo No Encontrado", detail = ex.Message });
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DeviceResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _deviceService.GetAllAsync();
        return Ok(result);
    }
}
