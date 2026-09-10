using Maros.Application.DTOs.ContactMessages;

namespace Maros.Application.Interfaces;

public interface IContactMessageService
{
    Task<List<ContactMessageResponseDto>> GetAllAsync();
    Task<ContactMessageResponseDto> CreateAsync(ContactMessageCreateDto request);
    Task<ContactMessageResponseDto> ToggleReadAsync(Guid id);
    Task RemoveAsync(Guid id);
}