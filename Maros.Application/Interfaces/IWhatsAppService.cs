using Maros.Application.DTOs.WhatsApp;

namespace Maros.Application.Interfaces;

public interface IWhatsAppService
{
    Task<WhatsAppMessageResponseDto> BuildMessageForQuotationAsync(Guid quotationId);
}