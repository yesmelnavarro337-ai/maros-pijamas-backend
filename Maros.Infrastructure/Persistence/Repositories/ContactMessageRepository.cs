using Maros.Domain.Entities;
using Maros.Domain.Interfaces;
using Maros.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Maros.Infrastructure.Persistence.Repositories;

public class ContactMessageRepository : IContactMessageRepository
{
    private readonly MarosDbContext _context;

    public ContactMessageRepository(MarosDbContext context) => _context = context;

    public Task<List<ContactMessage>> GetAllAsync() =>
        _context.ContactMessages.ToListAsync();

    public Task<ContactMessage?> GetByIdAsync(Guid id) =>
        _context.ContactMessages.FirstOrDefaultAsync(m => m.Id == id);

    public async Task AddAsync(ContactMessage message) =>
        await _context.ContactMessages.AddAsync(message);

    public void Remove(ContactMessage message) =>
        _context.ContactMessages.Remove(message);

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
}