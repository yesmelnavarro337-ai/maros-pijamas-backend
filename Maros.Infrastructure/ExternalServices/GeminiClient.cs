using System.Text;
using System.Text.Json;
using Maros.Application.Common;
using Maros.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Maros.Infrastructure.ExternalServices;

/// <summary>
/// Implementación de <see cref="IGeminiClient"/> contra la REST API de Google
/// Gemini (generateContent). Es 100% texto: no se solicita generación de imágenes.
/// </summary>
/// <remarks>
/// Configuración (cualquiera de las dos formas):
/// <list type="bullet">
/// <item><c>Gemini:ApiKey</c>, <c>Gemini:Model</c>, <c>Gemini:Enabled</c>, <c>Gemini:BaseUrl</c></item>
/// <item>Variables de entorno: <c>GEMINI_API_KEY</c>, <c>GEMINI_MODEL</c>, <c>GEMINI_BASE_URL</c></item>
/// </list>
/// El asistente queda deshabilitado (sin romper nada) si <c>Enabled</c> es false
/// o si no hay API key.
/// </remarks>
public sealed class GeminiClient : IGeminiClient
{
    private const string DefaultModel = "gemini-2.5-flash";
    private const string DefaultBaseUrl = "https://generativelanguage.googleapis.com";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<GeminiClient> _logger;
    private readonly string _apiKey;
    private readonly string _model;
    private readonly string _baseUrl;

    public GeminiClient(
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        ILogger<GeminiClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;

        _apiKey = Resolve(configuration, "Gemini:ApiKey", "GEMINI_API_KEY");
        _model = Fallback(Resolve(configuration, "Gemini:Model", "GEMINI_MODEL"), DefaultModel);
        _baseUrl = Fallback(Resolve(configuration, "Gemini:BaseUrl", "GEMINI_BASE_URL"), DefaultBaseUrl).TrimEnd('/');

        IsEnabled = ReadBool(configuration, "Gemini:Enabled", "GEMINI_ENABLED")
                    && !string.IsNullOrWhiteSpace(_apiKey);
    }

    public bool IsEnabled { get; }

    public string? Model => IsEnabled ? _model : null;

    public async Task<string> GenerateJsonAsync(string prompt, CancellationToken cancellationToken = default)
    {
        if (!IsEnabled)
            throw new AppException("El asistente de personalización no está disponible.", 503);

        var url = $"{_baseUrl}/v1beta/models/{_model}:generateContent?key={Uri.EscapeDataString(_apiKey)}";

        var payload = new
        {
            contents = new[]
            {
                new { role = "user", parts = new[] { new { text = prompt } } }
            },
            generationConfig = new
            {
                temperature = 0.7,
                responseMimeType = "application/json",
                responseSchema = new
                {
                    type = "OBJECT",
                    properties = new
                    {
                        reply = new { type = "STRING" },
                        telaId = new { type = "STRING" },
                        colorId = new { type = "STRING" },
                        estampadoId = new { type = "STRING" },
                        bordadoId = new { type = "STRING" },
                        embroideryText = new { type = "STRING" }
                    },
                    required = new[] { "reply" }
                }
            }
        };

        var client = _httpClientFactory.CreateClient("GeminiClient");
        using var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        HttpResponseMessage response;
        try
        {
            response = await client.PostAsync(url, content, cancellationToken);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            _logger.LogError(ex, "Error de red al llamar a Gemini.");
            throw new AppException("No pudimos contactar al asistente. Intenta más tarde.", 502, ex);
        }

        using (response)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Gemini respondió {Status}: {Body}", (int)response.StatusCode, Truncate(body));
                throw new AppException("El asistente no está disponible en este momento.", 502);
            }

            var text = ExtractText(body);
            if (string.IsNullOrWhiteSpace(text))
                throw new AppException("El asistente no devolvió una respuesta válida.", 502);

            return text;
        }
    }

    /// <summary>Extrae el texto de candidates[0].content.parts[*].text.</summary>
    private static string ExtractText(string body)
    {
        try
        {
            using var doc = JsonDocument.Parse(body);
            if (!doc.RootElement.TryGetProperty("candidates", out var candidates)
                || candidates.ValueKind != JsonValueKind.Array
                || candidates.GetArrayLength() == 0)
            {
                return string.Empty;
            }

            if (!candidates[0].TryGetProperty("content", out var content)
                || !content.TryGetProperty("parts", out var parts)
                || parts.ValueKind != JsonValueKind.Array)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            foreach (var part in parts.EnumerateArray())
            {
                if (part.TryGetProperty("text", out var text) && text.ValueKind == JsonValueKind.String)
                    sb.Append(text.GetString());
            }

            return sb.ToString();
        }
        catch (JsonException)
        {
            return string.Empty;
        }
    }

    private static string Resolve(IConfiguration configuration, string configKey, string envKey)
    {
        // Soporta la notación con dos puntos y la doble barra baja (env vars de
        // contenedores), además de la variable de entorno explícita.
        var raw = configuration[configKey]
                  ?? configuration[configKey.Replace(":", "__")]
                  ?? configuration[envKey];

        return string.IsNullOrWhiteSpace(raw) ? string.Empty : raw.Trim().Trim('"', '\'');
    }

    private static bool ReadBool(IConfiguration configuration, string configKey, string envKey)
    {
        var raw = Resolve(configuration, configKey, envKey);
        return bool.TryParse(raw, out var value) && value;
    }

    private static string Fallback(string value, string fallback) =>
        string.IsNullOrWhiteSpace(value) ? fallback : value;

    private static string Truncate(string value) =>
        value.Length <= 500 ? value : value[..500];
}
