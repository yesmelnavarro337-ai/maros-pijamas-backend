using Maros.Application.DTOs.Quotations;
using Maros.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Maros.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QuotationsController : ControllerBase
{
    private readonly IQuotationService _quotationService;
    private readonly IWhatsAppService _whatsAppService;

    public QuotationsController(IQuotationService quotationService, IWhatsAppService whatsAppService)
    {
        _quotationService = quotationService;
        _whatsAppService = whatsAppService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] QuotationQueryParams query)
    {
        var quotations = await _quotationService.GetAllAsync(query);
        return Ok(quotations);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var quotation = await _quotationService.GetByIdAsync(id);
        return Ok(quotation);
    }

    [HttpGet("{id:guid}/whatsapp")]
    public async Task<IActionResult> GetWhatsAppMessage(Guid id)
    {
        var message = await _whatsAppService.BuildMessageForQuotationAsync(id);
        return Ok(message);
    }

    [HttpPut("{id:guid}/status")]
    [Authorize(Policy = "RequireEditorOrAdmin")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] QuotationStatusUpdateDto request)
    {
        var updated = await _quotationService.UpdateStatusAsync(id, request);
        return Ok(updated);
    }
}