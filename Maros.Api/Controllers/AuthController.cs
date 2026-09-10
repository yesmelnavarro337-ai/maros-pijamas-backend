using Maros.Application.DTOs.Auth;
using Maros.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Maros.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
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
}