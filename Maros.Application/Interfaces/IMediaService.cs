using Maros.Application.DTOs.Common;

namespace Maros.Application.Interfaces;

public interface IMediaService
{
    Task<ImageUploadResultDto> UploadAsync(Stream fileStream, string fileName, string folder);
}