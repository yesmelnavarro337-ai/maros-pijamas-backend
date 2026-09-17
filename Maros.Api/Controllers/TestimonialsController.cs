using Maros.Application.DTOs.Testimonials;
using Maros.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maros.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TestimonialsController : ControllerBase
{
    private readonly ITestimonialService _testimonialService;

    public TestimonialsController(ITestimonialService testimonialService)
    {
        _testimonialService = testimonialService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] TestimonialQueryParams query)
    {
        var testimonials = await _testimonialService.GetAllAsync(query);
        return Ok(testimonials);
    }

    [HttpPost]
    [Authorize(Policy = "RequireEditorOrAdmin")]
    public async Task<IActionResult> Create([FromBody] TestimonialCreateDto request)
    {
        var created = await _testimonialService.CreateAsync(request);
        return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "RequireEditorOrAdmin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] TestimonialUpdateDto request)
    {
        var updated = await _testimonialService.UpdateAsync(id, request);
        return Ok(updated);
    }

    [HttpPut("{id:guid}/status")]
    [Authorize(Policy = "RequireEditorOrAdmin")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] TestimonialUpdateDto request)
    {
        var updated = await _testimonialService.UpdateStatusAsync(id, request);
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "RequireEditorOrAdmin")]
    public async Task<IActionResult> Remove(Guid id)
    {
        await _testimonialService.RemoveAsync(id);
        return NoContent();
    }
}