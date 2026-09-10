using Maros.Application.Common;
using Maros.Application.DTOs.ContactMessages;
using Maros.Application.Interfaces;
using Maros.Domain.Entities;
using Maros.Domain.Interfaces;

namespace Maros.Application.Services;

public class ContactMessageService : IContactMessageService
{
    private readonly IContactMessageRepository _repository;

    public ContactMessageService(IContactMessageRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ContactMessageResponseDto>> GetAllAsync()
    {
        var messages = await _repository.GetAllAsync();
        return messages.OrderByDescending(m => m.CreatedAt).Select(ToDto).ToList();
    }

    public async Task<ContactMessageResponseDto> CreateAsync(ContactMessageCreateDto request)
    {
        var message = new ContactMessage
        {
            FullName = request.FullName,
            Phone = request.Phone,
            Email = request.Email,
            Message = request.Message,
        };

        await _repository.AddAsync(message);
        await _repository.SaveChangesAsync();

        return ToDto(message);
    }

    public async Task<ContactMessageResponseDto> ToggleReadAsync(Guid id)
    {
        var message = await _repository.GetByIdAsync(id)
            ?? throw new AppException("Mensaje no encontrado.", 404);

        message.IsRead = !message.IsRead;
        message.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();
        return ToDto(message);
    }

    public async Task RemoveAsync(Guid id)
    {
        var message = await _repository.GetByIdAsync(id)
            ?? throw new AppException("Mensaje no encontrado.", 404);

        _repository.Remove(message);
        await _repository.SaveChangesAsync();
    }

    private static ContactMessageResponseDto ToDto(ContactMessage m) =>
        new(m.Id, m.FullName, m.Phone, m.Email, m.Message, m.IsRead, m.CreatedAt);
}