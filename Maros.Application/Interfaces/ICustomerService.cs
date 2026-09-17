using Maros.Application.DTOs.Customers;
using Maros.Application.Common;

namespace Maros.Application.Interfaces;

public interface ICustomerService
{
    Task<PagedResult<CustomerResponseDto>> GetAllAsync(CustomerQueryParams query);
    Task<CustomerWithQuotationsDto> GetByIdWithQuotationsAsync(Guid id);
    Task<CustomerResponseDto> CreateAsync(CustomerCreateDto request);
    Task<CustomerResponseDto> UpdateAsync(Guid id, CustomerUpdateDto request);
    Task<List<string>> GetCitiesAsync();
}