namespace Maros.Application.DTOs.Customization;

/// <summary>
/// Petición del asistente de personalización. <see cref="Message"/> es lo que el
/// cliente escribe ("algo elegante, tonos tierra"). <see cref="ProductSlug"/> y
/// <see cref="CurrentSelection"/> son contexto opcional del paso del asistente.
/// </summary>
public record CustomizationAssistantRequestDto(
    string Message,
    string? ProductSlug = null,
    Dictionary<string, string>? CurrentSelection = null
);

/// <summary>Ids sugeridos por categoría. Null cuando no se recomienda cambio.</summary>
public record CustomizationAssistantSuggestionDto(
    Guid? TelaId = null,
    Guid? ColorId = null,
    Guid? EstampadoId = null,
    Guid? BordadoId = null
);

/// <summary>Respuesta del asistente: texto + ids validados contra el catálogo real.</summary>
public record CustomizationAssistantResponseDto(
    string Reply,
    CustomizationAssistantSuggestionDto Suggestion,
    string? EmbroideryText = null
);

/// <summary>
/// Estado del asistente. <see cref="Model"/> es el modelo configurado cuando el
/// asistente está habilitado, o null cuando no lo está.
/// </summary>
public record CustomizationAssistantStatusDto(bool Enabled, string? Model = null);
