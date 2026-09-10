using Maros.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maros.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "RequireEditorOrAdmin")]
public class ContactMessagesController : ControllerBase
{
    private readonly IContactMessageService _service;

    public ContactMessagesController(IContactMessageService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var messages = await _service.GetAllAsync();
        return Ok(messages);
    }

    [HttpPut("{id:guid}/read")]
    public async Task<IActionResult> ToggleRead(Guid id)
    {
        var updated = await _service.ToggleReadAsync(id);
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remove(Guid id)
    {
        await _service.RemoveAsync(id);
        return NoContent();
    }
}