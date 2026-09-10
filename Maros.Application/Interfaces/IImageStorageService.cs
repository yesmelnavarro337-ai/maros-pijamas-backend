using Maros.Application.DTOs.Common;

namespace Maros.Application.Interfaces;

public interface IImageStorageService
{
    Task<ImageUploadResultDto> UploadAsync(Stream fileStream, string fileName, string folder);
    Task DeleteAsync(string publicId);
}