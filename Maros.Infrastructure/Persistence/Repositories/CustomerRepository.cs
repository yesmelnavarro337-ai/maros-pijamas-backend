using Maros.Domain.Entities;
using Maros.Domain.Interfaces;
using Maros.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Maros.Infrastructure.Persistence.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly MarosDbContext _context;

    public CustomerRepository(MarosDbContext context) => _context = context;

    public IQueryable<Customer> QueryAll() => _context.Customers;

    public Task<Customer?> GetByIdAsync(Guid id) =>
        _context.Customers.FirstOrDefaultAsync(c => c.Id == id);

    public Task<Customer?> GetByPhoneAsync(string phone) =>
        _context.Customers.FirstOrDefaultAsync(c => c.Phone == phone);

    public async Task AddAsync(Customer customer) =>
        await _context.Customers.AddAsync(customer);

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
}