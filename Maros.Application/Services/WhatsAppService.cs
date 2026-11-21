using System.Text;
using Maros.Application.Common;
using Maros.Application.DTOs.WhatsApp;
using Maros.Application.Interfaces;
using Maros.Domain.Entities;
using Maros.Domain.Interfaces;

namespace Maros.Application.Services;

public class WhatsAppService : IWhatsAppService
{
    /// <summary>
    /// Respaldo del número oficial de la tienda por si la configuración está vacía.
    /// Coincide con el default de <see cref="SiteSettings.WhatsappNumber"/>.
    /// </summary>
    private const string DefaultStorePhone = "573013169974";

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

        var message = BuildMessageText(quotation);

        // El chat se abre con la tienda oficial: el teléfono del cliente viaja
        // únicamente como dato dentro del cuerpo del mensaje.
        var storePhone = NormalizeStorePhone(settings?.WhatsappNumber);
        var link = $"https://wa.me/{storePhone}?text={Uri.EscapeDataString(message)}";

        return new WhatsAppMessageResponseDto(quotation.Customer.Phone, message, link);
    }

    private static string BuildMessageText(Quotation quotation)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Hola Maro's Pijamas, acabo de enviar una solicitud de cotización en la web.");
        sb.AppendLine($"Cliente: {quotation.Customer!.Name}");
        sb.AppendLine($"Teléfono: {quotation.Customer.Phone}");
        if (!string.IsNullOrWhiteSpace(quotation.Customer.City))
        {
            sb.AppendLine($"Ciudad: {quotation.Customer.City}");
        }
        sb.Append("Detalle:");

        decimal totalEstimated = 0m;

        foreach (var item in quotation.Items)
        {
            var productName = item.Product?.Name ?? "Producto personalizado";
            var calc = QuotationPriceCalculator.CalculateItemPrice(item);
            totalEstimated += calc.LineTotal;

            var details = item.SelectedOptions
                .Select(o => $"{o.CustomizationOption.CatalogType}: {o.CustomizationOption.Name}")
                .ToList();

            if (!string.IsNullOrWhiteSpace(item.Size))
                details.Insert(0, $"Talla: {item.Size}");

            if (!string.IsNullOrWhiteSpace(item.EmbroideryText))
                details.Add($"Bordado: \"{item.EmbroideryText}\"");

            sb.AppendLine();
            sb.Append($"- {productName} x{item.Quantity}");
            if (details.Count > 0)
                sb.Append($" ({string.Join(" · ", details)})");

            sb.AppendLine();
            sb.Append($"  Valor unitario: ${calc.UnitPrice:N0} COP | Cantidad: {item.Quantity} unidades | Subtotal: ${calc.LineTotal:N0} COP{calc.BreakdownNote}");
        }

        if (totalEstimated > 0)
        {
            sb.AppendLine();
            sb.AppendLine($"VALOR ESTIMADO TOTAL: ${totalEstimated:N0} COP");
        }

        if (!string.IsNullOrWhiteSpace(quotation.Notes))
        {
            sb.AppendLine();
            sb.Append($"Notas: {quotation.Notes}");
        }

        return sb.ToString().TrimEnd();
    }

    /// <summary>
    /// Normaliza el número oficial de la tienda a dígitos con código de país.
    /// Acepta formatos como "+57 301 316 9974" o el local "3013169974".
    /// </summary>
    private static string NormalizeStorePhone(string? phone)
    {
        var digits = new string((phone ?? string.Empty).Where(char.IsDigit).ToArray());
        if (digits.Length == 0) return DefaultStorePhone;
        // Móvil colombiano en formato local (10 dígitos, inicia en 3).
        if (digits.Length == 10 && digits.StartsWith('3')) return $"57{digits}";
        return digits;
    }
}
