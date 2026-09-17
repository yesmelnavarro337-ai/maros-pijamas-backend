using Maros.Application.DTOs.Auth;
using Maros.Application.Interfaces;
using Maros.Domain.Entities;
using Maros.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Maros.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly MarosDbContext _dbContext;

    public AuthController(IAuthService authService, MarosDbContext dbContext)
    {
        _authService = authService;
        _dbContext = dbContext;
    }

    /// <summary>
    /// Autentica a un usuario administrativo y devuelve un token JWT.
    /// </summary>
    /// <param name="request">Correo y contraseña del usuario.</param>
    /// <returns>Token JWT válido por el tiempo configurado en Jwt:ExpiryMinutes, junto con los datos básicos del usuario.</returns>
    /// <response code="200">Login exitoso.</response>
    /// <response code="401">Correo o contraseña incorrectos.</response>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var result = await _authService.LoginAsync(request);

        if (result is null)
            return Unauthorized(new { message = "Correo o contraseña incorrectos." });

        try
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "190.216.45.12";
            if (ip == "::1" || ip == "127.0.0.1")
                ip = "190.216.45.12";

            var userAgent = Request.Headers.UserAgent.ToString();
            var deviceType = ParseDeviceType(userAgent);

            _dbContext.UserAuditLogs.Add(new UserAuditLog
            {
                Id = Guid.NewGuid(),
                UserId = result.UserId,
                IpAddress = ip,
                UserAgent = string.IsNullOrWhiteSpace(userAgent) ? "Mozilla/5.0 (Windows NT 10.0; Win64; x64)" : userAgent,
                DeviceType = deviceType,
                CreatedAt = DateTime.UtcNow
            });
            await _dbContext.SaveChangesAsync();
        }
        catch
        {
            // Evitar bloquear el login si falla el log de auditoría
        }

        return Ok(result);
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var name = User.FindFirstValue(ClaimTypes.Name);
        var email = User.FindFirstValue(ClaimTypes.Email);
        var role = User.FindFirstValue(ClaimTypes.Role);

        return Ok(new { name, email, role });
    }

    private static string ParseDeviceType(string userAgent)
    {
        if (string.IsNullOrWhiteSpace(userAgent))
            return "Windows • Chrome";

        string os = "Windows";
        if (userAgent.Contains("Android", StringComparison.OrdinalIgnoreCase)) os = "Android";
        else if (userAgent.Contains("iPhone", StringComparison.OrdinalIgnoreCase) || userAgent.Contains("iPad", StringComparison.OrdinalIgnoreCase)) os = "iOS";
        else if (userAgent.Contains("Macintosh", StringComparison.OrdinalIgnoreCase) || userAgent.Contains("Mac OS", StringComparison.OrdinalIgnoreCase)) os = "macOS";
        else if (userAgent.Contains("Linux", StringComparison.OrdinalIgnoreCase)) os = "Linux";
        else if (userAgent.Contains("Windows", StringComparison.OrdinalIgnoreCase)) os = "Windows";

        string browser = "Chrome";
        if (userAgent.Contains("Edg", StringComparison.OrdinalIgnoreCase)) browser = "Edge";
        else if (userAgent.Contains("Firefox", StringComparison.OrdinalIgnoreCase)) browser = "Firefox";
        else if (userAgent.Contains("Safari", StringComparison.OrdinalIgnoreCase) && !userAgent.Contains("Chrome", StringComparison.OrdinalIgnoreCase)) browser = "Safari";

        return $"{os} • {browser}";
    }
}