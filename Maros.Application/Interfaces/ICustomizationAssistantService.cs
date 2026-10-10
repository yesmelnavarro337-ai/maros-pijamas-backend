using Maros.Application.DTOs.Customization;

namespace Maros.Application.Interfaces;

/// <summary>
/// Asistente de personalización: recomienda una combinación de tela, color,
/// estampado y bordado a partir del catálogo real y del lenguaje natural del
/// cliente. No genera imágenes; solo selecciona opciones existentes.
/// </summary>
public interface ICustomizationAssistantService
{
    /// <summary>True si el proveedor de IA está configurado y habilitado.</summary>
    bool IsEnabled { get; }

    /// <summary>Modelo de IA configurado (null si el asistente está deshabilitado).</summary>
    string? Model { get; }

    /// <summary>
    /// Devuelve una recomendación validada. Los ids retornados siempre existen en
    /// el catálogo vigente (y, si se indica producto, dentro de las opciones
    /// asignadas a su modelo de personalización).
    /// </summary>
    Task<CustomizationAssistantResponseDto> SuggestAsync(
        CustomizationAssistantRequestDto request,
        CancellationToken cancellationToken = default);
}
