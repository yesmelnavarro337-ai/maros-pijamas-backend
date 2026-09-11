using System.IdentityModel.Tokens.Jwt;
using Maros.Application.Common;
using Maros.Application.DTOs.Users;
using Maros.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Maros.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "RequireAdministrador")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    [HttpPost("invite")]
    public async Task<IActionResult> Invite([FromBody] InviteUserRequestDto request)
    {
        var created = await _userService.InviteAsync(request);
        return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequestDto request)
    {
        try
        {
            var updated = await _userService.UpdateAsync(id, request);
            return Ok(updated);
        }
        catch (AppException) { throw; }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Error de base de datos al editar el usuario {UserId}. Message={Message}, Inner={Inner}",
                id, dbEx.Message, dbEx.InnerException?.Message);
            return Conflict(new { status = 409, message = "No se pudo actualizar el usuario porque entra en conflicto con datos existentes." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al editar el usuario {UserId}. Message={Message}, Inner={Inner}",
                id, ex.Message, ex.InnerException?.Message);
            return StatusCode(500, new { status = 500, message = "Ocurrió un error al actualizar el usuario. Revisa los logs del servidor." });
        }
    }

    [HttpPut("{id:guid}/role")]
    public async Task<IActionResult> UpdateRole(Guid id, [FromBody] UpdateUserRoleRequestDto request)
    {
        try
        {
            var updated = await _userService.UpdateRoleAsync(id, request, CurrentUserId);
            return Ok(updated);
        }
        catch (AppException) { throw; }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Error de base de datos al cambiar el rol del usuario {UserId}. Message={Message}, Inner={Inner}",
                id, dbEx.Message, dbEx.InnerException?.Message);
            return Conflict(new { status = 409, message = "No se pudo cambiar el rol del usuario porque entra en conflicto con datos existentes." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar el rol del usuario {UserId}. Message={Message}, Inner={Inner}",
                id, ex.Message, ex.InnerException?.Message);
            return StatusCode(500, new { status = 500, message = "Ocurrió un error al cambiar el rol del usuario. Revisa los logs del servidor." });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remove(Guid id)
    {
        try
        {
            await _userService.RemoveAsync(id, CurrentUserId);
            return NoContent();
        }
        catch (AppException) { throw; }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Error de base de datos al eliminar el usuario {UserId}. Message={Message}, Inner={Inner}",
                id, dbEx.Message, dbEx.InnerException?.Message);
            return Conflict(new { status = 409, message = "No se puede eliminar el usuario porque tiene registros asociados." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el usuario {UserId}. Message={Message}, Inner={Inner}",
                id, ex.Message, ex.InnerException?.Message);
            return StatusCode(500, new { status = 500, message = "Ocurrió un error al eliminar el usuario. Revisa los logs del servidor." });
        }
    }
}