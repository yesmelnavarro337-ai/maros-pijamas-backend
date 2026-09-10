using Maros.Application.DTOs.Customization;
using Maros.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maros.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomizationOptionsController : ControllerBase
{
    private readonly ICustomizationOptionService _service;

    public CustomizationOptionsController(ICustomizationOptionService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var options = await _service.GetAllAsync();
        return Ok(options);
    }

    [HttpPost]
    [Authorize(Policy = "RequireEditorOrAdmin")]
    public async Task<IActionResult> Create([FromBody] CustomizationOptionCreateDto request)
    {
        var created = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "RequireEditorOrAdmin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CustomizationOptionUpdateDto request)
    {
        var updated = await _service.UpdateAsync(id, request);
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "RequireEditorOrAdmin")]
    public async Task<IActionResult> Remove(Guid id)
    {
        await _service.RemoveAsync(id);
        return NoContent();
    }
}