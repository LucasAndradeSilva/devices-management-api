using Asp.Versioning;
using Devices.Application.Common;
using Devices.Application.DTOs;
using Devices.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Devices.Api.Controllers;

/// <summary>
/// Manages device resources.
/// Provides operations to create, retrieve, update and delete devices.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class DevicesController : ControllerBase
{
    private readonly IDeviceService _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="DevicesController"/>.
    /// </summary>
    /// <param name="service">Device service instance</param>
    public DevicesController(IDeviceService service)
    {
        _service = service;
    }

    /// <summary>
    /// Creates a new device.
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/v1/devices
    ///     {
    ///        "name": "iPhone 15",
    ///        "brand": "Apple"
    ///     }
    ///
    /// Rules:
    /// - Name is required
    /// - Brand is required
    /// </remarks>
    /// <param name="dto">Device data</param>
    /// <returns>Returns the created device</returns>
    /// <response code="201">Device successfully created</response>
    /// <response code="400">Invalid input data</response>
    /// <response code="500">Unexpected error</response>
    [HttpPost]
    [ProducesResponseType(typeof(Result<DeviceResponseDto>), 201)]
    [ProducesResponseType(typeof(Result<object>), 400)]
    [ProducesResponseType(typeof(Result<object>), 500)]
    public async Task<IActionResult> Create([FromBody] DeviceDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return StatusCode(result.StatusCodeValue, result);
    }

    /// <summary>
    /// Retrieves all devices with pagination.
    /// </summary>
    /// <remarks>
    /// You can paginate results using query parameters:
    ///
    ///     GET /api/v1/devices?page=1&amp;pageSize=10
    /// </remarks>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Number of items per page (default: 10)</param>
    /// <returns>List of devices</returns>
    /// <response code="200">Devices retrieved successfully</response>
    /// <response code="500">Unexpected error</response>
    [HttpGet]
    [ProducesResponseType(typeof(Result<IEnumerable<DeviceResponseDto>>), 200)]
    [ProducesResponseType(typeof(Result<object>), 500)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetAllAsync(page, pageSize);
        return StatusCode(result.StatusCodeValue, result);
    }

    /// <summary>
    /// Retrieves a device by its identifier.
    /// </summary>
    /// <param name="id">Device unique identifier</param>
    /// <returns>Device data</returns>
    /// <response code="200">Device found</response>
    /// <response code="404">Device not found</response>
    /// <response code="500">Unexpected error</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<DeviceResponseDto>), 200)]
    [ProducesResponseType(typeof(Result<object>), 404)]
    [ProducesResponseType(typeof(Result<object>), 500)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return StatusCode(result.StatusCodeValue, result);
    }

    /// <summary>
    /// Updates an existing device.
    /// </summary>
    /// <remarks>
    /// Rules:
    /// - Device must exist
    /// - Devices in use cannot be updated
    /// </remarks>
    /// <param name="id">Device identifier</param>
    /// <param name="dto">Updated device data</param>
    /// <returns>Update result</returns>
    /// <response code="200">Device updated successfully</response>
    /// <response code="400">Invalid operation</response>
    /// <response code="404">Device not found</response>
    /// <response code="500">Unexpected error</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Result<bool>), 200)]
    [ProducesResponseType(typeof(Result<object>), 400)]
    [ProducesResponseType(typeof(Result<object>), 404)]
    [ProducesResponseType(typeof(Result<object>), 500)]
    public async Task<IActionResult> Update(Guid id, [FromBody] DeviceDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return StatusCode(result.StatusCodeValue, result);
    }

    /// <summary>
    /// Deletes a device.
    /// </summary>
    /// <remarks>
    /// Rules:
    /// - Device must exist
    /// - Devices in use cannot be deleted
    /// </remarks>
    /// <param name="id">Device identifier</param>
    /// <returns>Delete result</returns>
    /// <response code="200">Device deleted successfully</response>
    /// <response code="400">Device cannot be deleted</response>
    /// <response code="404">Device not found</response>
    /// <response code="500">Unexpected error</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(Result<bool>), 200)]
    [ProducesResponseType(typeof(Result<object>), 400)]
    [ProducesResponseType(typeof(Result<object>), 404)]
    [ProducesResponseType(typeof(Result<object>), 500)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _service.DeleteAsync(id);
        return StatusCode(result.StatusCodeValue, result);
    }
}