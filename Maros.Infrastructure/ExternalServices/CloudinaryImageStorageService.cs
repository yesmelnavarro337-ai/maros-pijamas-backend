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
        var cloudName = configuration["Cloudinary:CloudName"];
        var apiKey = configuration["Cloudinary:ApiKey"];
        var apiSecret = configuration["Cloudinary:ApiSecret"];

        if (string.IsNullOrWhiteSpace(cloudName) || string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(apiSecret))
            throw new InvalidOperationException("Faltan credenciales de Cloudinary en la configuración.");

        var account = new Account(cloudName, apiKey, apiSecret);
        _cloudinary = new Cloudinary(account);
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