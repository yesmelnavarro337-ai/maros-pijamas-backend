using Maros.Domain.Entities;

namespace Maros.Domain.Interfaces;

public interface IFaqRepository
{
    Task<List<Faq>> GetAllOrderedAsync();
    Task<Faq?> GetByIdAsync(Guid id);
    Task<List<Faq>> GetPublicOrderedAsync();
    Task<int> GetNextOrderAsync();
    Task AddAsync(Faq faq);
    void Remove(Faq faq);
    Task SaveChangesAsync();
}