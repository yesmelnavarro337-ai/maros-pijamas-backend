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

    public CloudinaryImageStorageService(IConfiguration configuration, ILogger<CloudinaryImageStorageService> logger)
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

    public async Task<ImageUploadResultDto> UploadAsync(Stream fileStream, string fileName, string folder)
    {
        try
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, fileStream),
                Folder = $"maros-pijamas/{folder}",
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

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