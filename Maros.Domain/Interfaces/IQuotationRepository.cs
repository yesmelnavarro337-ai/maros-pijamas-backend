using Maros.Domain.Entities;

namespace Maros.Domain.Interfaces;

public interface IQuotationRepository
{
    IQueryable<Quotation> QueryAll();
    Task<Quotation?> GetByIdAsync(Guid id);
    Task<List<Quotation>> GetByCustomerIdAsync(Guid customerId);
    Task AddAsync(Quotation quotation);
    Task SaveChangesAsync();
}