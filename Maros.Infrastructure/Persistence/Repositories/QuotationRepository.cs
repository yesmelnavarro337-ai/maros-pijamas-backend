using Maros.Domain.Entities;
using Maros.Domain.Interfaces;
using Maros.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Maros.Infrastructure.Persistence.Repositories;

public class QuotationRepository : IQuotationRepository
{
    private readonly MarosDbContext _context;

    public QuotationRepository(MarosDbContext context) => _context = context;

    private IQueryable<Quotation> QueryWithIncludes() =>
        _context.Quotations
            .Include(q => q.Customer)
            .Include(q => q.ReferenceImages)
            .Include(q => q.Items)
                .ThenInclude(i => i.Product!)
                    .ThenInclude(p => p.Images)
            .Include(q => q.Items)
                .ThenInclude(i => i.SelectedOptions)
                    .ThenInclude(o => o.CustomizationOption);

    public IQueryable<Quotation> QueryAll() => QueryWithIncludes();

    public Task<Quotation?> GetByIdAsync(Guid id) =>
        QueryWithIncludes().FirstOrDefaultAsync(q => q.Id == id);

    public Task<List<Quotation>> GetByCustomerIdAsync(Guid customerId) =>
        QueryWithIncludes().Where(q => q.CustomerId == customerId).OrderByDescending(q => q.CreatedAt).ToListAsync();

    public async Task AddAsync(Quotation quotation) =>
        await _context.Quotations.AddAsync(quotation);

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
}