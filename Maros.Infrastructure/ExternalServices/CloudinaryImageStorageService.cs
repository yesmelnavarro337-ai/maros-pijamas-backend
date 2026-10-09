using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Maros.Application.DTOs.Common;
using Maros.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Maros.Infrastructure.ExternalServices;

public class CloudinaryImageStorageService : IImageStorageService
{
    private readonly Cloudinary _cloudinary;
    private readonly ILogger<CloudinaryImageStorageService> _logger;

    public CloudinaryImageStorageService(
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        ILogger<CloudinaryImageStorageService> logger)
    {
        _logger = logger;

        var cloudName = GetConfigValue(configuration, "CloudName", "CLOUDINARY_CLOUD_NAME");
        var apiKey = GetConfigValue(configuration, "ApiKey", "CLOUDINARY_API_KEY");
        var apiSecret = GetConfigValue(configuration, "ApiSecret", "CLOUDINARY_API_SECRET");

        if (string.IsNullOrWhiteSpace(cloudName) || string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(apiSecret))
        {
            _logger.LogError("Faltan credenciales de Cloudinary en la configuración (CloudName configurado: {HasName}, ApiKey configurado: {HasKey}, ApiSecret configurado: {HasSecret})",
                !string.IsNullOrWhiteSpace(cloudName), !string.IsNullOrWhiteSpace(apiKey), !string.IsNullOrWhiteSpace(apiSecret));
            throw new InvalidOperationException("Faltan credenciales de Cloudinary en la configuración (Cloudinary:CloudName, Cloudinary:ApiKey, Cloudinary:ApiSecret).");
        }

        var account = new Account(cloudName, apiKey, apiSecret);
        _cloudinary = new Cloudinary(account);

        // Extiende el timeout de la API de Cloudinary a 10 minutos: las subidas
        // pesadas (.mov de iPhone, videos de alta resolución) superaban el límite
        // por defecto de 100 s y se cancelaban con "HttpClient.Timeout ... elapsing".
        _cloudinary.Api.Client = httpClientFactory.CreateClient("CloudinaryClient");
        _cloudinary.Api.Timeout = (int)TimeSpan.FromMinutes(10).TotalMilliseconds;
    }

    private static string GetConfigValue(IConfiguration config, string subKey, string envKey)
    {
        var raw = config[$"Cloudinary:{subKey}"]
               ?? config[$"Cloudinary__{subKey}"]
               ?? config[envKey]
               ?? config[subKey];

        if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
        return raw.Trim().Trim('"', '\'');
    }

    private static readonly HashSet<string> VideoExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".mp4", ".webm", ".mov", ".avi", ".mkv", ".m4v"
    };

    // Cloudinary no tiene un resource type "audio": los archivos de audio se
    // suben como tipo "raw" (/raw/upload/) y la SecureUrl resultante se sirve
    // tal cual al reproductor <audio> de maros-web.
    private static readonly HashSet<string> AudioExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".mp3", ".wav", ".ogg", ".m4a", ".aac", ".flac"
    };

    public async Task<ImageUploadResultDto> UploadAsync(Stream fileStream, string fileName, string folder)
    {
        try
        {
            var extension = Path.GetExtension(fileName);
            var isAudio = AudioExtensions.Contains(extension);
            var isVideo = VideoExtensions.Contains(extension);

            UploadResult result;
            if (isAudio)
            {
                // Audio ambiental (MP3): subida "raw" sin transformaciones. La
                // URL devuelta ya es reproducible directamente en <audio>.
                var rawParams = new RawUploadParams
                {
                    File = new FileDescription(fileName, fileStream),
                    Folder = $"maros-pijamas/{folder}",
                };
                result = await _cloudinary.UploadAsync(rawParams);
            }
            else if (isVideo)
            {
                // Cloudinary clasifica los videos como resource type "video"
                // (/video/upload); subirlos como imagen fallaría o quedaría
                // inutilizable en el reproductor del slider.
                //
                // Los .mov de iPhone usan codec HEVC (H.265) que la mayoría de
                // navegadores NO reproducen: se aplica la transformación q_auto
                // + f_mp4 para que Cloudinary entregue una derivación MP4 (H.264)
                // universalmente compatible con <video> en maros-web.
                var videoParams = new VideoUploadParams
                {
                    File = new FileDescription(fileName, fileStream),
                    Folder = $"maros-pijamas/{folder}",
                    Transformation = new Transformation().Quality("auto").FetchFormat("mp4"),
                };
                result = await _cloudinary.UploadAsync(videoParams);
            }
            else
            {
                var imageParams = new ImageUploadParams
                {
                    File = new FileDescription(fileName, fileStream),
                    Folder = $"maros-pijamas/{folder}",
                };

                // HEIC/HEIF (iPhone): los navegadores no renderizan el formato, así
                // que Cloudinary lo convierte a JPEG al almacenarlo y la SecureUrl
                // devuelta queda servible en cualquier cliente sin transformaciones.
                if (extension.Equals(".heic", StringComparison.OrdinalIgnoreCase)
                    || extension.Equals(".heif", StringComparison.OrdinalIgnoreCase))
                {
                    imageParams.Format = "jpg";
                }

                result = await _cloudinary.UploadAsync(imageParams);
            }

            if (result.Error is not null)
            {
                var errorMsg = !string.IsNullOrWhiteSpace(result.Error.Message)
                    ? result.Error.Message
                    : "Error desconocido retornado por Cloudinary.";
                _logger.LogError("Error al subir imagen '{FileName}' a Cloudinary: {ErrorMessage}", fileName, errorMsg);
                throw new InvalidOperationException($"Error al subir la imagen a Cloudinary: {errorMsg}");
            }

            _logger.LogInformation("Imagen '{FileName}' subida con éxito a Cloudinary (PublicId: {PublicId})", fileName, result.PublicId);
            return new ImageUploadResultDto(result.SecureUrl.ToString(), result.PublicId);
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excepción al transmitir imagen '{FileName}' a Cloudinary", fileName);
            throw new InvalidOperationException($"Error de conexión o procesamiento al subir la imagen a Cloudinary: {ex.Message}", ex);
        }
    }

    public async Task DeleteAsync(string publicId)
    {
        try
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);
            _logger.LogInformation("Eliminación de imagen {PublicId} en Cloudinary finalizada con resultado {Result}", publicId, result.Result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la imagen {PublicId} de Cloudinary", publicId);
            throw;
        }
    }
}