using Maros.Application.DTOs.Common;
using Maros.Application.Interfaces;

namespace Maros.Application.Services;

public class MediaService : IMediaService
{
    private readonly IImageStorageService _imageStorage;

    public MediaService(IImageStorageService imageStorage)
    {
        _imageStorage = imageStorage;
    }

    public Task<ImageUploadResultDto> UploadAsync(Stream fileStream, string fileName, string folder) =>
        _imageStorage.UploadAsync(fileStream, fileName, folder);
}