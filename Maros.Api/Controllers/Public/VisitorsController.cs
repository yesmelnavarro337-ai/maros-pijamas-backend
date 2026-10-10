using Maros.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maros.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class VisitorsController : ControllerBase
{
    private readonly ISiteSettingsRepository _repository;
    private readonly ILogger<VisitorsController> _logger;

    public VisitorsController(ISiteSettingsRepository repository, ILogger<VisitorsController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Contador global de visitas persistido en PostgreSQL.
    /// </summary>
    /// <param name="peek">
    /// Si es true (o ?peek=1), solo lee el valor sin incrementarlo — usado por el
    /// sondeo pasivo del frontend. Si es false o está ausente, representa una
    /// visita nueva y el contador se incrementa de forma atómica en +1.
    /// </param>
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] bool peek = false)
    {
        try
        {
            var visitors = peek
                ? await _repository.GetVisitorsCountAsync()
                : await _repository.IncrementVisitorsAsync();
            return Ok(new { visitors });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al gestionar el contador de visitas.");
            return StatusCode(500, new { message = "Error al obtener el contador de visitas." });
        }
    }
}