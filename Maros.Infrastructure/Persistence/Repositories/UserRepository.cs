using Maros.Domain.Entities;
using Maros.Domain.Interfaces;
using Maros.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Maros.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MarosDbContext _context;

    public UserRepository(MarosDbContext context)
    {
        _context = context;
    }

    public Task<User?> GetByEmailAsync(string email) =>
        _context.Users.FirstOrDefaultAsync(u => u.Email == email);

    public Task<User?> GetByIdAsync(Guid id) =>
        _context.Users.FirstOrDefaultAsync(u => u.Id == id);

    public Task<User?> GetByInviteTokenAsync(string token) =>
        _context.Users.FirstOrDefaultAsync(u => u.InviteToken == token);

    public Task<List<User>> GetAllAsync() =>
        _context.Users.OrderBy(u => u.Name).ToListAsync();

    public Task<bool> EmailExistsAsync(string email, Guid? excludeUserId = null) =>
        _context.Users.AnyAsync(u => u.Email == email && (excludeUserId == null || u.Id != excludeUserId));

    public async Task AddAsync(User user) =>
        await _context.Users.AddAsync(user);

    public void Remove(User user) =>
        _context.Users.Remove(user);

    public Task<bool> AnyAsync() =>
        _context.Users.AnyAsync();

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
}