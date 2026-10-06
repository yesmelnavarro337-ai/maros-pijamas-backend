using Maros.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maros.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class StylesController : ControllerBase
{
    private readonly IStyleRepository _styleRepository;

    public StylesController(IStyleRepository styleRepository)
    {
        _styleRepository = styleRepository;
    }

    /// <summary>Catálogo de estilos disponibles para maros-admin y maros-web.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
    {
        var styles = includeInactive
            ? await _styleRepository.GetAllAsync()
            : await _styleRepository.GetActiveAsync();

        var response = styles
            .Select(s => new
            {
                id = s.Id,
                name = s.Name,
                slug = s.Slug,
                hexCode = s.HexCode,
                displayOrder = s.DisplayOrder,
                line = s.Line.ToString()
            })
            .ToList();

        return Ok(response);
    }
}