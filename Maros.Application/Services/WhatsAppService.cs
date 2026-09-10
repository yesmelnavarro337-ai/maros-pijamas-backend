using System.Text;
using Maros.Application.Common;
using Maros.Application.DTOs.WhatsApp;
using Maros.Application.Interfaces;
using Maros.Domain.Entities;
using Maros.Domain.Interfaces;

namespace Maros.Application.Services;

public class WhatsAppService : IWhatsAppService
{
    private readonly IQuotationRepository _quotationRepository;
    private readonly ISiteSettingsRepository _siteSettingsRepository;

    public WhatsAppService(IQuotationRepository quotationRepository, ISiteSettingsRepository siteSettingsRepository)
    {
        _quotationRepository = quotationRepository;
        _siteSettingsRepository = siteSettingsRepository;
    }

    public async Task<WhatsAppMessageResponseDto> BuildMessageForQuotationAsync(Guid quotationId)
    {
        var quotation = await _quotationRepository.GetByIdAsync(quotationId)
            ?? throw new AppException("Cotización no encontrada.", 404);

        if (quotation.Customer is null)
            throw new AppException("La cotización no tiene un cliente asociado.", 400);

        var settings = await _siteSettingsRepository.GetAsync();
        var greeting = string.IsNullOrWhiteSpace(settings?.WhatsappDefaultMessage)
            ? "¡Hola! Te escribimos de Maro's Pijamas sobre tu cotización."
            : settings.WhatsappDefaultMessage;

        var message = BuildMessageText(quotation, greeting);
        var cleanPhone = CleanPhone(quotation.Customer.Phone);
        var link = $"https://wa.me/57{cleanPhone}?text={Uri.EscapeDataString(message)}";

        return new WhatsAppMessageResponseDto(quotation.Customer.Phone, message, link);
    }

    private static string BuildMessageText(Quotation quotation, string greeting)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Hola {quotation.Customer!.Name}, {greeting.TrimStart('¡').TrimStart('H', 'o', 'l', 'a', '!', ' ')}");
        sb.AppendLine();

        foreach (var item in quotation.Items)
        {
            var productName = item.Product?.Name ?? "Producto personalizado";
            var details = item.SelectedOptions
                .Select(o => $"{o.CustomizationOption.CatalogType}: {o.CustomizationOption.Name}")
                .ToList();

            if (!string.IsNullOrWhiteSpace(item.Size))
                details.Insert(0, $"Talla: {item.Size}");

            if (!string.IsNullOrWhiteSpace(item.EmbroideryText))
                details.Add($"Bordado: \"{item.EmbroideryText}\"");

            sb.AppendLine($"- {productName} x{item.Quantity}");
            if (details.Count > 0)
                sb.AppendLine($"  ({string.Join(" · ", details)})");
        }

        if (!string.IsNullOrWhiteSpace(quotation.Notes))
        {
            sb.AppendLine();
            sb.AppendLine($"Notas: {quotation.Notes}");
        }

        return sb.ToString().TrimEnd();
    }

    private static string CleanPhone(string phone) =>
        new string(phone.Where(char.IsDigit).ToArray());
}