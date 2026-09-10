using Maros.Domain.Entities;

namespace Maros.Domain.Interfaces;

public interface ICustomerRepository
{
    IQueryable<Customer> QueryAll();
    Task<Customer?> GetByIdAsync(Guid id);
    Task<Customer?> GetByPhoneAsync(string phone);
    Task AddAsync(Customer customer);
    Task SaveChangesAsync();
}