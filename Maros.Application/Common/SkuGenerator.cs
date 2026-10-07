using System.Text;
using System.Text.RegularExpressions;

namespace Maros.Application.Common;

/// <summary>
/// Motor de SKUs determinista y ultracompacto, espejo del generador del admin
/// (features/products/utils/sku-generator.ts):
///
///   [CAT]-[PROD_HASH]-[ESTILO]-[COL]-[TAL]-[SUFIJO_OPT]
///
/// Mismos datos de entrada producen el mismo SKU en cliente y servidor.
/// </summary>
public static partial class SkuGenerator
{
    public record SkuInput(
        string? CategoryName,
        string? ProductName,
        string? Size,
        string? ColorName,
        string? StyleName,
        string? MaterialName
    );

    public static string Build(SkuInput input) => string.Join("-",
        CategoryCode(input.CategoryName),
        ProductCode(input.ProductName),
        LineCode(input.StyleName),
        ColorCode(input.ColorName),
        SizeCode(input.Size));

    /// <summary>
    /// Asignador incremental anticolisión O(1) por llamada: la primera
    /// ocurrencia de cada combinación queda sin sufijo; los duplicados reciben
    /// correlativo de 2 dígitos (-01, -02…). Se comparte entre ítems del lote.
    /// </summary>
    public sealed class BatchAllocator
    {
        private readonly HashSet<string> _used;
        private readonly Dictionary<string, int> _counters = new();

        public BatchAllocator(IEnumerable<string>? reservedSkus = null)
        {
            _used = new HashSet<string>(reservedSkus ?? Enumerable.Empty<string>(), StringComparer.Ordinal);
        }

        public string Allocate(SkuInput input)
        {
            var baseSku = Build(input);
            if (_used.Add(baseSku))
            {
                _counters[baseSku] = 0;
                return baseSku;
            }

            var suffix = _counters.GetValueOrDefault(baseSku) + 1;
            var candidate = $"{baseSku}-{suffix:00}";
            while (!_used.Add(candidate))
            {
                suffix++;
                candidate = $"{baseSku}-{suffix:00}";
            }
            _counters[baseSku] = suffix;
            return candidate;
        }
    }

    /// <summary>CAT: 3 letras de la primera palabra significativa de la categoría.</summary>
    public static string CategoryCode(string? categoryName)
    {
        var first = Words(categoryName).FirstOrDefault() ?? string.Empty;
        var code = first.Length >= 3 ? first[..3] : first;
        return string.IsNullOrEmpty(code) ? "GEN" : code.PadRight(3, 'X');
    }

    /// <summary>PROD_HASH: acrónimo inteligente de 4 letras a partir del nombre/slug.</summary>
    public static string ProductCode(string? productName)
    {
        var words = Words(productName);
        return words.Count switch
        {
            0 => "PROD",
            >= 4 => string.Concat(words.Take(4).Select(w => w[0])),
            3 => $"{words[0][0]}{words[1][0]}{words[2][..Math.Min(2, words[2].Length)]}",
            2 => $"{words[0][..Math.Min(2, words[0].Length)]}{words[1][..Math.Min(2, words[1].Length)]}",
            _ => words[0][..Math.Min(4, words[0].Length)].PadRight(4, 'X'),
        };
    }

    /// <summary>ESTILO: línea de la prenda. DAM / CAB / INF / UNI.</summary>
    public static string LineCode(string? styleName)
    {
        var text = Normalize(styleName);
        if (text.Contains("INFANTIL") || text.Contains("NINO") || text.Contains("BEBE")) return "INF";
        if (text.Contains("HOMBRE")) return "CAB";
        if (text.Contains("MUJER")) return "DAM";
        return "UNI";
    }

    /// <summary>COL: 2 letras de la 1ª palabra + inicial de la 2ª ("Amarillo Estampado" → AMP).</summary>
    public static string ColorCode(string? colorName)
    {
        var words = Words(colorName);
        if (words.Count == 0) return "VAR";
        if (words.Count == 1) return words[0][..Math.Min(3, words[0].Length)].PadRight(3, 'X');
        return $"{words[0][..Math.Min(2, words[0].Length)]}{words[1][0]}";
    }

    /// <summary>TAL: talla directa normalizada, sin guiones ni espacios ("0-3M" → "03M").</summary>
    public static string SizeCode(string? size)
    {
        var code = Normalize(size).Replace(" ", string.Empty);
        return string.IsNullOrEmpty(code) ? "DEF" : code;
    }

    private static List<string> Words(string? value) =>
        Normalize(value)
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .ToList();

    /// <summary>Sin tildes, mayúsculas, solo A-Z0-9 y espacios separadores.</summary>
    private static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;

        var decomposed = value.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(decomposed.Length);
        foreach (var c in decomposed)
        {
            if (char.IsAsciiLetterOrDigit(c)) sb.Append(char.ToUpperInvariant(c));
            else if (!NonSpacingMarkRegex().IsMatch(c.ToString())) sb.Append(' ');
        }
        return MultipleSpacesRegex().Replace(sb.ToString(), " ").Trim();
    }

    [GeneratedRegex(@"[\p{M}]")]
    private static partial Regex NonSpacingMarkRegex();

    [GeneratedRegex(@"\s+")]
    private static partial Regex MultipleSpacesRegex();
}
