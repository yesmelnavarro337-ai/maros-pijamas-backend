using Maros.Application.DTOs.ContactMessages;
using Maros.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maros.Api.Controllers;

[ApiController]
[Route("api/public/contact-message")]
[AllowAnonymous]
public class ContactMessageController : ControllerBase
{
    private readonly IContactMessageService _service;

    public ContactMessageController(IContactMessageService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ContactMessageCreateDto request)
    {
        await _service.CreateAsync(request);
        return Ok(new { message = "Mensaje recibido correctamente." });
    }
}