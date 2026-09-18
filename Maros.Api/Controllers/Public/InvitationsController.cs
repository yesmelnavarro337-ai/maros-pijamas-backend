using Maros.Application.DTOs.Invitations;
using Maros.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maros.Api.Controllers;

[ApiController]
[Route("api/public/invitations")]
[AllowAnonymous]
public class PublicInvitationsController : ControllerBase
{
    private readonly IInvitationService _invitationService;

    public PublicInvitationsController(IInvitationService invitationService)
    {
        _invitationService = invitationService;
    }

    [HttpGet("{token}")]
    public async Task<IActionResult> GetSummary(string token)
    {
        var summary = await _invitationService.GetSummaryAsync(token);
        return Ok(summary);
    }

    [HttpPost("accept")]
    public async Task<IActionResult> Accept([FromBody] AcceptInvitationRequestDto request)
    {
        await _invitationService.AcceptAsync(request.Token, request.NewPassword, request.Email);
        return Ok(new { message = "Cuenta activada con éxito. Ya puedes iniciar sesión." });
    }
}
