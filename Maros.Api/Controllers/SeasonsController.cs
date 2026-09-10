using Maros.Application.DTOs.Seasons;
using Maros.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maros.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SeasonsController : ControllerBase
{
    private readonly ISeasonService _seasonService;

    public SeasonsController(ISeasonService seasonService)
    {
        _seasonService = seasonService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var seasons = await _seasonService.GetAllAsync();
        return Ok(seasons);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var season = await _seasonService.GetByIdAsync(id);
        return Ok(season);
    }

    [HttpPost]
    [Authorize(Policy = "RequireEditorOrAdmin")]
    public async Task<IActionResult> Create([FromBody] SeasonCreateDto request)
    {
        var created = await _seasonService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "RequireEditorOrAdmin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] SeasonUpdateDto request)
    {
        var updated = await _seasonService.UpdateAsync(id, request);
        return Ok(updated);
    }

    /// <summary>
    /// Activa una temporada. Regla de negocio: solo puede haber una temporada activa
    /// a la vez — cualquier otra temporada que estuviera activa pasa automáticamente
    /// a estado "Finalizada".
    /// </summary>
    [HttpPost("{id:guid}/activate")]
    [Authorize(Policy = "RequireEditorOrAdmin")]
    public async Task<IActionResult> Activate(Guid id)
    {
        var activated = await _seasonService.ActivateAsync(id);
        return Ok(activated);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "RequireEditorOrAdmin")]
    public async Task<IActionResult> Remove(Guid id)
    {
        await _seasonService.RemoveAsync(id);
        return NoContent();
    }
}