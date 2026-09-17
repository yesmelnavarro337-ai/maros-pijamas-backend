using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Maros.Application.DTOs.Common;
using Maros.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Maros.Infrastructure.ExternalServices;

public class CloudinaryImageStorageService : IImageStorageService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryImageStorageService(IConfiguration configuration)
    {
        var cloudName = GetConfigValue(configuration, "CloudName", "CLOUDINARY_CLOUD_NAME");
        var apiKey = GetConfigValue(configuration, "ApiKey", "CLOUDINARY_API_KEY");
        var apiSecret = GetConfigValue(configuration, "ApiSecret", "CLOUDINARY_API_SECRET");

        if (string.IsNullOrWhiteSpace(cloudName) || string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(apiSecret))
            throw new InvalidOperationException("Faltan credenciales de Cloudinary en la configuración (Cloudinary:CloudName, Cloudinary:ApiKey, Cloudinary:ApiSecret).");

        var account = new Account(cloudName, apiKey, apiSecret);
        _cloudinary = new Cloudinary(account);
    }

    private static string GetConfigValue(IConfiguration config, string subKey, string envKey)
    {
        var raw = config[$"Cloudinary:{subKey}"] ?? config[envKey] ?? config[subKey];
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
                throw new InvalidOperationException($"Error al subir la imagen a Cloudinary: {result.Error.Message}");

            return new ImageUploadResultDto(result.SecureUrl.ToString(), result.PublicId);
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error de conexión o procesamiento al subir la imagen a Cloudinary: {ex.Message}", ex);
        }
    }

    public async Task DeleteAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);
        await _cloudinary.DestroyAsync(deleteParams);
    }
}