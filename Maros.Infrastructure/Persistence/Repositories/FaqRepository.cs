using Maros.Domain.Entities;
using Maros.Domain.Enums;
using Maros.Domain.Interfaces;
using Maros.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Maros.Infrastructure.Persistence.Repositories;

public class FaqRepository : IFaqRepository
{
    private readonly MarosDbContext _context;

    public FaqRepository(MarosDbContext context) => _context = context;

    public Task<List<Faq>> GetAllOrderedAsync() =>
        _context.Faqs.OrderBy(f => f.Order).ToListAsync();

    public Task<Faq?> GetByIdAsync(Guid id) =>
        _context.Faqs.FirstOrDefaultAsync(f => f.Id == id);

    public Task<List<Faq>> GetPublicOrderedAsync() =>
        _context.Faqs
            .Where(f => f.Status == FaqStatus.Publicada)
            .OrderBy(f => f.Order)
            .ToListAsync();

    public async Task<int> GetNextOrderAsync()
    {
        var maxOrder = await _context.Faqs.Select(f => (int?)f.Order).MaxAsync();
        return (maxOrder ?? -1) + 1;
    }

    public async Task AddAsync(Faq faq) =>
        await _context.Faqs.AddAsync(faq);

    public void Remove(Faq faq) =>
        _context.Faqs.Remove(faq);

    public Task<int> DeleteByIdAsync(Guid id) =>
        _context.Faqs.Where(f => f.Id == id).ExecuteDeleteAsync();

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
}
