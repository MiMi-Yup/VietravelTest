using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebAPI.Configurations;
using WebAPI.DTOs.Requests;
using WebAPI.Filters;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
[ServiceFilter(typeof(IpWhitelistFilter))]
public class AuthController : ControllerBase
{
    private readonly IJwtService _jwtService;
    private readonly AuthConfiguration _authConfig;

    public AuthController(IJwtService jwtService, IOptions<AuthConfiguration> authOptions)
    {
        _jwtService = jwtService;
        _authConfig = authOptions.Value;
    }

    /// <summary>POST /api/auth/login — Basic authentication</summary>
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (request.Username != _authConfig.Username || request.Password != _authConfig.Password)
        {
            return Unauthorized(new { Message = "Tên đăng nhập hoặc mật khẩu không đúng." });
        }

        var token = _jwtService.GenerateToken(
            userId: Guid.NewGuid().ToString(),
            username: request.Username
        );

        return Ok(new { Token = token, Username = request.Username });
    }

    /// <summary>GET /api/auth/login — Legacy auto-login (kept for backward compatibility)</summary>
    [HttpGet("login")]
    public IActionResult AutoLogin()
    {
        var token = _jwtService.GenerateToken(
            userId: Guid.NewGuid().ToString(),
            username: "Guest"
        );

        return Ok(new { Token = token, Username = "Guest" });
    }
}