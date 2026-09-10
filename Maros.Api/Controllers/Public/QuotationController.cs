using Maros.Application.DTOs.Quotations;
using Maros.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maros.Api.Controllers;

[ApiController]
[Route("api/public/[controller]")]
[AllowAnonymous]
public class QuotationController : ControllerBase
{
    private readonly IQuotationService _quotationService;
    private readonly IWhatsAppService _whatsAppService;

    public QuotationController(IQuotationService quotationService, IWhatsAppService whatsAppService)
    {
        _quotationService = quotationService;
        _whatsAppService = whatsAppService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] QuotationCreateDto request)
    {
        var created = await _quotationService.CreateAsync(request);

        // Se genera el mensaje de WhatsApp en el mismo momento, usando el
        // servicio que ya existe (Fase 11) — el cliente anónimo recibe todo
        // en una sola respuesta, sin necesitar autenticación para consultarlo
        // después en el endpoint administrativo.
        var whatsapp = await _whatsAppService.BuildMessageForQuotationAsync(created.Id);

        return Ok(new
        {
            message = "Cotización recibida correctamente.",
            id = created.Id,
            whatsapp = new
            {
                phoneNumber = whatsapp.PhoneNumber,
                message = whatsapp.Message,
                link = whatsapp.Link,
            },
        });
    }
}