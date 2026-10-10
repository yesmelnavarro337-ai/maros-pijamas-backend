namespace Maros.Application.Interfaces;

/// <summary>
/// Cliente de bajo nivel para Gemini. Recibe un prompt y devuelve el texto JSON
/// crudo generado por el modelo. La validación del contenido y la orquestación
/// vive en <see cref="ICustomizationAssistantService"/> para poder probarla sin
/// depender de la red.
/// </summary>
public interface IGeminiClient
{
    /// <summary>True solo si el proveedor está habilitado y tiene API key.</summary>
    bool IsEnabled { get; }

    /// <summary>
    /// Nombre del modelo configurado (p. ej. "gemini-2.5-flash"). Null si el
    /// proveedor no está habilitado.
    /// </summary>
    string? Model { get; }

    /// <summary>
    /// Envía el prompt a Gemini esperando una salida JSON y devuelve el texto.
    /// Lanza <see cref="Maros.Application.Common.AppException"/> si el proveedor
    /// no está disponible o responde con error.
    /// </summary>
    Task<string> GenerateJsonAsync(string prompt, CancellationToken cancellationToken = default);
}
