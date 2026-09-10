using Maros.Application.DTOs.Faq;
using Maros.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maros.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FaqController : ControllerBase
{
    private readonly IFaqService _faqService;

    public FaqController(IFaqService faqService)
    {
        _faqService = faqService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var faqs = await _faqService.GetAllAsync();
        return Ok(faqs);
    }

    [HttpPost]
    [Authorize(Policy = "RequireEditorOrAdmin")]
    public async Task<IActionResult> Create([FromBody] FaqCreateDto request)
    {
        var created = await _faqService.CreateAsync(request);
        return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "RequireEditorOrAdmin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] FaqUpdateDto request)
    {
        var updated = await _faqService.UpdateAsync(id, request);
        return Ok(updated);
    }

    [HttpPut("{id:guid}/reorder")]
    [Authorize(Policy = "RequireEditorOrAdmin")]
    public async Task<IActionResult> Reorder(Guid id, [FromQuery] string direction)
    {
        await _faqService.ReorderAsync(id, direction);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "RequireEditorOrAdmin")]
    public async Task<IActionResult> Remove(Guid id)
    {
        await _faqService.RemoveAsync(id);
        return NoContent();
    }
}