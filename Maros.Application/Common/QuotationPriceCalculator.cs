using System.Globalization;
using System.Text;
using Maros.Domain.Entities;

namespace Maros.Application.Common;

public record QuotationItemPriceCalculation(
    decimal BasePrice,
    decimal DiscountAmount,
    decimal SizeSurcharge,
    decimal OptionsSurcharge,
    decimal UnitPrice,
    decimal LineTotal,
    bool IsInfantil,
    bool HasDiscount,
    string BreakdownNote
);

public static class QuotationPriceCalculator
{
    public const decimal PromoDiscountPercent = 0.05m;
    // Nota: Las tallas juveniles (10 a 16) NO tienen recargo.
    // Todas las tallas infantiles comparten el mismo precio base del estilo.
    public const decimal AdultXlSurcharge = 10000m;
    public const decimal Adult2XlSurcharge = 25000m;
    public const decimal Adult3XlSurcharge = 30000m;
    public const decimal EmbroideryStandardSurcharge = 7000m;

    private static readonly HashSet<string> InfantilSmallSizes = new(StringComparer.OrdinalIgnoreCase)
    {
        "0-3M", "3-6M", "6-12M", "12-18M", "2T", "4", "6", "8"
    };

    private static readonly HashSet<string> InfantilYouthSizes = new(StringComparer.OrdinalIgnoreCase)
    {
        "10", "12", "14", "16"
    };

    public static bool IsPromoActive(DateTime? date = null)
    {
        var current = date ?? DateTime.UtcNow;
        var start = new DateTime(current.Year, 10, 4, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(current.Year, 11, 9, 23, 59, 59, DateTimeKind.Utc);
        return current >= start && current <= end;
    }

    public static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;
        var normalized = value.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var c in normalized)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }
        return sb.ToString().Normalize(NormalizationForm.FormC);
    }

    public static string NormalizeSize(string? size)
    {
        return (size ?? string.Empty).Trim().ToUpperInvariant().Replace(" ", string.Empty);
    }

    public static bool IsInfantilItem(QuotationItem item)
    {
        var categoryNames = item.Product?.ProductCategories
            .Select(pc => Normalize(pc.Category?.Name))
            .ToList() ?? new List<string>();

        var styleNames = item.Product?.ProductStyles
            .Select(ps => Normalize(ps.Style?.Name))
            .ToList() ?? new List<string>();

        var productName = Normalize(item.Product?.Name);
        var allText = $"{string.Join(" ", categoryNames)} {string.Join(" ", styleNames)} {productName}";

        var tokens = new[] { "nino", "nina", "infantil", "bebe", "kids", "child" };
        if (tokens.Any(t => allText.Contains(t))) return true;

        var size = NormalizeSize(item.Size);
        if (InfantilSmallSizes.Contains(size) || InfantilYouthSizes.Contains(size))
        {
            // Si la talla es claramente de bebé o numérica infantil y no tiene marcador adulto
            if (!allText.Contains("mujer") && !allText.Contains("hombre"))
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsInfantilYouthSize(string? size)
    {
        var s = NormalizeSize(size);
        if (InfantilYouthSizes.Contains(s)) return true;
        if (int.TryParse(s, out var num) && num >= 10 && num <= 16) return true;
        return false;
    }

    public static decimal GetAdultSizeSurcharge(string? size)
    {
        var s = NormalizeSize(size);
        if (s is "3XL" or "XXXL" or "4XL" or "XXXXL" or "5XL") return Adult3XlSurcharge;
        if (s is "2XL" or "XXL") return Adult2XlSurcharge;
        if (s is "XL" || (s.Length >= 2 && s.Contains("XL"))) return AdultXlSurcharge;
        return 0m;
    }

    public static decimal ResolveBasePrice(QuotationItem item, bool isInfantil)
    {
        var styleName = Normalize(item.Product?.ProductStyles.FirstOrDefault()?.Style?.Name);
        var productName = Normalize(item.Product?.Name);
        var categoryName = Normalize(item.Product?.ProductCategories.FirstOrDefault()?.Category?.Name);
        var combined = $"{styleName} {productName} {categoryName}";

        if (isInfantil)
        {
            if (combined.Contains("larga")) return 90000m;
            if (combined.Contains("short")) return 65000m;
            if (combined.Contains("pantalon")) return 75000m;
            if (item.Product != null && item.Product.BasePrice > 0) return item.Product.BasePrice;
            return 65000m;
        }

        var isHombre = combined.Contains("hombre");

        if (combined.Contains("camiseta"))
        {
            if (combined.Contains("short")) return 70000m;
            if (combined.Contains("pantalon")) return 85000m;
        }

        if (combined.Contains("bata")) return 85000m;

        if (combined.Contains("pantalon"))
        {
            if (combined.Contains("larga")) return isHombre ? 140000m : 135000m;
            return isHombre ? 120000m : 115000m;
        }

        if (combined.Contains("short") || combined.Contains("camisa"))
        {
            return isHombre ? 95000m : 85000m;
        }

        if (item.Product != null && item.Product.BasePrice > 0) return item.Product.BasePrice;

        return isHombre ? 120000m : 115000m;
    }

    public static QuotationItemPriceCalculation CalculateItemPrice(QuotationItem item, DateTime? currentDate = null)
    {
        var isInfantil = IsInfantilItem(item);
        var basePrice = ResolveBasePrice(item, isInfantil);

        var hasPromo = IsPromoActive(currentDate);
        var discountAmount = hasPromo ? Math.Round(basePrice * PromoDiscountPercent, 0) : 0m;
        var baseAfterDiscount = basePrice - discountAmount;

        var sizeSurcharge = 0m;

        if (!isInfantil)
        {
            sizeSurcharge = GetAdultSizeSurcharge(item.Size);
        }

        var optionsSurcharge = item.SelectedOptions.Sum(o => o.CustomizationOption?.PriceModifier ?? 0m);

        var hasEmbroideryOption = item.SelectedOptions.Any(o =>
            Normalize(o.CustomizationOption?.CatalogType.ToString()).Contains("bordado") &&
            (o.CustomizationOption?.PriceModifier ?? 0m) > 0);

        if (!hasEmbroideryOption && !string.IsNullOrWhiteSpace(item.EmbroideryText))
        {
            optionsSurcharge += EmbroideryStandardSurcharge;
        }

        var unitPrice = baseAfterDiscount + sizeSurcharge + optionsSurcharge;
        var lineTotal = unitPrice * Math.Max(1, item.Quantity);

        // Generar desglose legible
        var notes = new List<string>();
        if (discountAmount > 0)
        {
            notes.Add("5% OFF aplicado");
        }
        if (sizeSurcharge > 0)
        {
            notes.Add($"+${sizeSurcharge:N0} Recargo Talla {NormalizeSize(item.Size)}");
        }
        if (optionsSurcharge > 0)
        {
            notes.Add($"+${optionsSurcharge:N0} Personalización");
        }

        var breakdownNote = notes.Count > 0 ? $" ({string.Join(" · ", notes)})" : string.Empty;

        return new QuotationItemPriceCalculation(
            basePrice,
            discountAmount,
            sizeSurcharge,
            optionsSurcharge,
            unitPrice,
            lineTotal,
            isInfantil,
            hasPromo,
            breakdownNote
        );
    }
}
