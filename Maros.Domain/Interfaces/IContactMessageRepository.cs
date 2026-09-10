using Maros.Domain.Entities;

namespace Maros.Domain.Interfaces;

public interface IContactMessageRepository
{
    Task<List<ContactMessage>> GetAllAsync();
    Task<ContactMessage?> GetByIdAsync(Guid id);
    Task AddAsync(ContactMessage message);
    void Remove(ContactMessage message);
    Task SaveChangesAsync();
}