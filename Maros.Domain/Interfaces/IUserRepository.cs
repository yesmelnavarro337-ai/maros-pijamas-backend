using Maros.Domain.Entities;

namespace Maros.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByInviteTokenAsync(string token);
    Task<List<User>> GetAllAsync();
    Task<bool> EmailExistsAsync(string email, Guid? excludeUserId = null);
    Task AddAsync(User user);
    void Remove(User user);
    Task<bool> AnyAsync();
    Task SaveChangesAsync();
}