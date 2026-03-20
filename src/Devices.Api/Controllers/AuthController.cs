using Asp.Versioning;
using Devices.Application.DTOs;
using Devices.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Devices.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/auth")]
[ApiVersion("1.0")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Authenticate user and generate JWT token
    /// </summary>
    /// <param name="dto">Login data</param>
    /// <returns>JWT token</returns>
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto dto)
    {        
        if (dto.Username != "admin" || dto.Password != "123")
            return Unauthorized(new
            {
                message = "Invalid credentials"
            });

        var token = _authService.GenerateToken(dto.Username);

        return Ok(new
        {
            access_token = token
        });
    }
}