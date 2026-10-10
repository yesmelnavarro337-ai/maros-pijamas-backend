using System.Text;
using System.Text.Json;
using Maros.Application.Common;
using Maros.Application.DTOs.Customization;
using Maros.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Maros.Application.Services;

/// <summary>
/// Orquesta el asistente de personalización: reúne el catálogo real, lo restringe
/// al modelo del producto (si aplica), construye el prompt, valida la salida de
/// Gemini contra los ids existentes y la mapea al DTO público. Toda la lógica es
/// determinista y testeable; el acceso a la red vive en <see cref="IGeminiClient"/>.
/// </summary>
public sealed class CustomizationAssistantService : ICustomizationAssistantService
{
    private const string Tela = "Tela";
    private const string Color = "Color";
    private const string Estampado = "Estampado";
    private const string Bordado = "Bordado";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IGeminiClient _gemini;
    private readonly ICustomizationOptionService _customizationOptionService;
    private readonly IProductService _productService;
    private readonly ILogger<CustomizationAssistantService> _logger;

    public CustomizationAssistantService(
        IGeminiClient gemini,
        ICustomizationOptionService customizationOptionService,
        IProductService productService,
        ILogger<CustomizationAssistantService> logger)
    {
        _gemini = gemini;
        _customizationOptionService = customizationOptionService;
        _productService = productService;
        _logger = logger;
    }

    public bool IsEnabled => _gemini.IsEnabled;

    public string? Model => _gemini.Model;

    public async Task<CustomizationAssistantResponseDto> SuggestAsync(
        CustomizationAssistantRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
            throw new AppException("Cuéntanos qué estilo buscas para poder sugerirte una combinación.", 400);

        var catalog = await _customizationOptionService.GetPublicCatalogAsync();
        var allowed = BuildAllowedOptions(catalog);

        await RestrictToProductAsync(request.ProductSlug, allowed, cancellationToken);

        if (allowed.Values.All(list => list.Count == 0))
            throw new AppException("Todavía no hay opciones de personalización configuradas para este producto.", 409);

        var prompt = BuildPrompt(allowed, request);
        var raw = await _gemini.GenerateJsonAsync(prompt, cancellationToken);

        return ParseSuggestion(raw, allowed);
    }

    // ─── Catálogo ──────────────────────────────────────────────────

    private static Dictionary<string, List<CustomizationOptionPublicDto>> BuildAllowedOptions(
        CustomizationCatalogPublicDto catalog)
    {
        var result = new Dictionary<string, List<CustomizationOptionPublicDto>>(StringComparer.OrdinalIgnoreCase)
        {
            [Tela] = new(),
            [Color] = new(),
            [Estampado] = new(),
            [Bordado] = new(),
        };

        foreach (var (key, options) in catalog.Catalogs)
            result[key] = options.ToList();

        return result;
    }

    /// <summary>
    /// Si el producto tiene un modelo de personalización con opciones asignadas,
    /// el asistente solo puede elegir dentro de ese subconjunto. Sin producto (o
    /// sin asignaciones) se usa el catálogo completo.
    /// </summary>
    private async Task RestrictToProductAsync(
        string? productSlug,
        Dictionary<string, List<CustomizationOptionPublicDto>> allowed,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(productSlug))
            return;

        try
        {
            var detail = await _productService.GetPublicDetailBySlugAsync(productSlug);
            var ids = detail.CustomizationOptionIds;
            if (ids is not { Count: > 0 })
                return;

            var idSet = ids.ToHashSet();
            foreach (var key in allowed.Keys.ToList())
                allowed[key] = allowed[key].Where(o => idSet.Contains(o.Id)).ToList();
        }
        catch (AppException ex)
        {
            _logger.LogWarning(ex,
                "No se pudo restringir el asistente al producto {Slug}; se usa el catálogo completo.", productSlug);
        }
    }

    // ─── Prompt ────────────────────────────────────────────────────

    private static string BuildPrompt(
        Dictionary<string, List<CustomizationOptionPublicDto>> allowed,
        CustomizationAssistantRequestDto request)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Eres el asesor experto en personalización de pijamas de \"Maro's Pijamas\" (Colombia).");
        sb.AppendLine("Recomienda UNA combinación coherente de tela, color, estampado y bordado para la prenda que el cliente está personalizando.");
        sb.AppendLine();
        sb.AppendLine("REGLAS ESTRICTAS:");
        sb.AppendLine("- Solo puedes elegir ids que aparezcan en el CATÁLOGO de abajo. NUNCA inventes ids.");
        sb.AppendLine("- Elige como máximo una opción por categoría.");
        sb.AppendLine("- Si una categoría no tiene opciones, deja su id como cadena vacía (\"\").");
        sb.AppendLine("- Prioriza coherencia cromática y de uso (por ejemplo, telas frescas para clima cálido).");
        sb.AppendLine("- 'reply': máximo 2 frases, tono cercano, en español.");
        sb.AppendLine("- 'embroideryText': máximo 3 palabras, solo si recomiendas bordado; si no, cadena vacía.");
        sb.AppendLine();
        sb.AppendLine("CATÁLOGO (elige los ids de aquí):");

        AppendCategory(sb, Tela, "telaId", allowed);
        AppendCategory(sb, Color, "colorId", allowed);
        AppendCategory(sb, Estampado, "estampadoId", allowed);
        AppendCategory(sb, Bordado, "bordadoId", allowed);

        if (request.CurrentSelection is { Count: > 0 })
        {
            sb.AppendLine();
            sb.AppendLine("SELECCIÓN ACTUAL DEL CLIENTE (respétala salvo que tengas una razón clara para cambiarla):");
            foreach (var (key, value) in request.CurrentSelection)
                sb.AppendLine($"- {key}: {value}");
        }

        sb.AppendLine();
        sb.AppendLine($"PETICIÓN DEL CLIENTE: \"{request.Message.Trim()}\"");
        return sb.ToString();
    }

    private static void AppendCategory(
        StringBuilder sb,
        string category,
        string jsonField,
        Dictionary<string, List<CustomizationOptionPublicDto>> allowed)
    {
        sb.AppendLine($"[{category}] (campo JSON: {jsonField})");
        if (!allowed.TryGetValue(category, out var options) || options.Count == 0)
        {
            sb.AppendLine("  (sin opciones disponibles — deja el campo vacío)");
            return;
        }

        foreach (var option in options)
        {
            var surcharge = option.PriceModifier is > 0
                ? $" (+${option.PriceModifier.Value:N0})"
                : string.Empty;
            sb.AppendLine($"  - {option.Id} = {option.Name}{surcharge}");
        }
    }

    // ─── Parseo y validación de la salida ──────────────────────────

    private static CustomizationAssistantResponseDto ParseSuggestion(
        string raw,
        Dictionary<string, List<CustomizationOptionPublicDto>> allowed)
    {
        GeminiSuggestion? parsed;
        try
        {
            parsed = JsonSerializer.Deserialize<GeminiSuggestion>(StripCodeFences(raw), JsonOptions);
        }
        catch (JsonException)
        {
            parsed = null;
        }

        if (parsed is null)
            throw new AppException("El asistente no pudo generar una recomendación. Intenta de nuevo.", 502);

        var reply = string.IsNullOrWhiteSpace(parsed.Reply)
            ? "Aquí tienes una combinación que podría encajar contigo."
            : parsed.Reply.Trim();

        var suggestion = new CustomizationAssistantSuggestionDto(
            Resolve(parsed.TelaId, Tela, allowed),
            Resolve(parsed.ColorId, Color, allowed),
            Resolve(parsed.EstampadoId, Estampado, allowed),
            Resolve(parsed.BordadoId, Bordado, allowed));

        return new CustomizationAssistantResponseDto(reply, suggestion, Clean(parsed.EmbroideryText));
    }

    private static Guid? Resolve(
        string? rawId,
        string category,
        Dictionary<string, List<CustomizationOptionPublicDto>> allowed)
    {
        if (string.IsNullOrWhiteSpace(rawId))
            return null;

        if (!Guid.TryParse(rawId.Trim(), out var id))
            return null;

        if (!allowed.TryGetValue(category, out var options))
            return null;

        return options.Any(o => o.Id == id) ? id : null;
    }

    private static string? Clean(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string StripCodeFences(string raw)
    {
        var text = raw.Trim();
        if (!text.StartsWith("```", StringComparison.Ordinal))
            return text;

        var firstNewLine = text.IndexOf('\n');
        if (firstNewLine < 0)
            return text;

        var body = text[(firstNewLine + 1)..];
        var lastFence = body.LastIndexOf("```", StringComparison.Ordinal);
        return (lastFence >= 0 ? body[..lastFence] : body).Trim();
    }

    private sealed record GeminiSuggestion(
        string? Reply,
        string? TelaId,
        string? ColorId,
        string? EstampadoId,
        string? BordadoId,
        string? EmbroideryText);
}
