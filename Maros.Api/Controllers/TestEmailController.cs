using Microsoft.AspNetCore.Mvc;
using Maros.Application.Interfaces;

namespace Maros.Api.Controllers
{
    public record SendInvitationTestDto(string To, string Name);
    public record SendCodeTestDto(string To, string Name, string Code);

    [ApiController]
    [Route("api/[controller]")]
    public class TestEmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public TestEmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send-invitation-test")]
        public async Task<IActionResult> SendInvitationTest([FromBody] SendInvitationTestDto dto)
        {
            var testUrl = $"https://maros-admin.vercel.app/accept-invitation?token=test-123&email={Uri.EscapeDataString(dto.To)}";

            await _emailService.SendInvitationAsync(dto.To, dto.Name, testUrl);

            return Ok(new { message = $"Correo de invitación enviado correctamente a {dto.To}" });
        }

        [HttpPost("send-code-test")]
        public async Task<IActionResult> SendCodeTest([FromBody] SendCodeTestDto dto)
        {
            await _emailService.SendEmailChangeCodeAsync(dto.To, dto.Name, dto.Code, isNewEmail: true);

            return Ok(new { message = $"Código de verificación enviado correctamente a {dto.To}" });
        }
    }
}
