using Maros.Application.DTOs.Home;
using Maros.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maros.Api.Controllers;

[ApiController]
[Route("api/home-content")]
[Authorize]
public class HomeContentController : ControllerBase
{
    private readonly IHomeSectionContentService _service;

    public HomeContentController(IHomeSectionContentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var sections = await _service.GetAllAsync();
        return Ok(sections);
    }

    [HttpPut("{sectionKey}")]
    [Authorize(Policy = "RequireEditorOrAdmin")]
    public async Task<IActionResult> Update(string sectionKey, [FromBody] HomeSectionContentUpdateDto request)
    {
        var updated = await _service.UpdateAsync(sectionKey, request);
        return Ok(updated);
    }
}
