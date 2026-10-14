using Maros.Application.DTOs.Customization;
using Maros.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maros.Api.Controllers;

[ApiController]
[Route("api/public/[controller]")]
[AllowAnonymous]
public class CustomizationController : ControllerBase
{
    private readonly ICustomizationOptionService _service;

    // ============================================================
    // FUTURA IMPLEMENTACIÓN: ASISTENTE DE PERSONALIZACIÓN CON IA (GEMINI)
    // El asistente está comentado. Para reactivar:
    //   1. Descomentar las líneas marcadas con "// [IA]"
    //   2. Reactivar los endpoints GET/POST "assistant"
    //   3. Reactivar los registros de DI en Application y Infrastructure
    // ============================================================

    // [IA] private readonly ICustomizationAssistantService _assistant;

    public CustomizationController(
        ICustomizationOptionService service
        // [IA] , ICustomizationAssistantService assistant
        )
    {
        _service = service;
        // [IA] _assistant = assistant;
    }

    [HttpGet("options")]
    public async Task<IActionResult> GetOptions()
    {
        var catalog = await _service.GetPublicCatalogAsync();
        return Ok(catalog);
    }

    // ─── Asistente IA (Gemini) — DESACTIVADO ──────────────────────────
    // Para reactivar: descomentar los dos endpoints siguientes y restaurar
    // los registros de IGeminiClient, ICustomizationAssistantService y
    // el rate limiter "CustomizationAssistant" en Program.cs.

    // [IA] /// <summary>Permite al frontend saber si el asistente de IA está disponible.</summary>
    // [IA] [HttpGet("assistant")]
    // [IA] public IActionResult GetAssistantStatus()
    // [IA] {
    // [IA]     return Ok(new CustomizationAssistantStatusDto(_assistant.IsEnabled, _assistant.Model));
    // [IA] }

    // [IA] /// <summary>
    // [IA] /// Recomienda una combinación (tela/color/estampado/bordado) a partir de la
    // [IA] /// petición en lenguaje natural del cliente. No genera imágenes.
    // [IA] /// </summary>
    // [IA] [HttpPost("assistant")]
    // [IA] [EnableRateLimiting("CustomizationAssistant")]
    // [IA] public async Task<IActionResult> Assist(
    // [IA]     [FromBody] CustomizationAssistantRequestDto request,
    // [IA]     CancellationToken cancellationToken)
    // [IA] {
    // [IA]     if (!_assistant.IsEnabled)
    // [IA]     {
    // [IA]         return StatusCode(
    // [IA]             StatusCodes.Status503ServiceUnavailable,
    // [IA]             new ApiErrorResponse(
    // [IA]                 StatusCodes.Status503ServiceUnavailable,
    // [IA]                 "El asistente de personalización no está disponible.",
    // [IA]                 null));
    // [IA]     }
    // [IA]
    // [IA]     var result = await _assistant.SuggestAsync(request, cancellationToken);
    // [IA]     return Ok(result);
    // [IA] }
}
