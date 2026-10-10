using Maros.Application.Common;
using Maros.Application.DTOs.Customization;
using Maros.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Maros.Api.Controllers;

[ApiController]
[Route("api/public/[controller]")]
[AllowAnonymous]
public class CustomizationController : ControllerBase
{
    private readonly ICustomizationOptionService _service;
    private readonly ICustomizationAssistantService _assistant;

    public CustomizationController(
        ICustomizationOptionService service,
        ICustomizationAssistantService assistant)
    {
        _service = service;
        _assistant = assistant;
    }

    [HttpGet("options")]
    public async Task<IActionResult> GetOptions()
    {
        var catalog = await _service.GetPublicCatalogAsync();
        return Ok(catalog);
    }

    /// <summary>Permite al frontend saber si el asistente de IA está disponible.</summary>
    [HttpGet("assistant")]
    public IActionResult GetAssistantStatus()
    {
        return Ok(new CustomizationAssistantStatusDto(_assistant.IsEnabled, _assistant.Model));
    }

    /// <summary>
    /// Recomienda una combinación (tela/color/estampado/bordado) a partir de la
    /// petición en lenguaje natural del cliente. No genera imágenes.
    /// </summary>
    [HttpPost("assistant")]
    [EnableRateLimiting("CustomizationAssistant")]
    public async Task<IActionResult> Assist(
        [FromBody] CustomizationAssistantRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!_assistant.IsEnabled)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                new ApiErrorResponse(
                    StatusCodes.Status503ServiceUnavailable,
                    "El asistente de personalización no está disponible.",
                    null));
        }

        var result = await _assistant.SuggestAsync(request, cancellationToken);
        return Ok(result);
    }
}
