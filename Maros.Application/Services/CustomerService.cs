using Maros.Application.Common;
using Maros.Application.DTOs.Customers;
using Maros.Application.Interfaces;
using Maros.Domain.Interfaces;
using Maros.Domain.Entities;

namespace Maros.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IQuotationRepository _quotationRepository;
    private readonly IPaginationService _paginationService;

    public CustomerService(
        ICustomerRepository customerRepository,
        IQuotationRepository quotationRepository,
        IPaginationService paginationService)
    {
        _customerRepository = customerRepository;
        _quotationRepository = quotationRepository;
        _paginationService = paginationService;
    }

    public async Task<PagedResult<CustomerResponseDto>> GetAllAsync(
        CustomerQueryParams query)
    {
        var customers = _customerRepository.QueryAll();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            customers = customers.Where(c =>
                c.Name.Contains(query.Search) ||
                c.Phone.Contains(query.Search));
        }

        customers = query.SortBy?.ToLowerInvariant() == "name"
            ? (query.SortDescending
                ? customers.OrderByDescending(c => c.Name)
                : customers.OrderBy(c => c.Name))
            : (query.SortDescending
                ? customers.OrderByDescending(c => c.CreatedAt)
                : customers.OrderBy(c => c.CreatedAt));

        var paged = await _paginationService.PaginateAsync(
            customers,
            query.PageNumber,
            query.PageSize);

        return new PagedResult<CustomerResponseDto>(
            paged.Items.Select(ToDto).ToList(),
            paged.PageNumber,
            paged.PageSize,
            paged.TotalCount);
    }

    public async Task<CustomerWithQuotationsDto> GetByIdWithQuotationsAsync(
        Guid id)
    {
        var customer = await _customerRepository.GetByIdAsync(id)
            ?? throw new Common.AppException(
                "Cliente no encontrado.",
                404);

        var quotations = await _quotationRepository.GetByCustomerIdAsync(id);

        var summaries = quotations.Select(q =>
            new CustomerQuotationSummaryDto(
                q.Id,
                q.Status.ToString(),
                q.CreatedAt,
                q.Items
                    .Select(i => i.Product?.Name ?? "Producto eliminado")
                    .ToList()
            )
        ).ToList();

        return new CustomerWithQuotationsDto(
            customer.Id,
            customer.Name,
            customer.Phone,
            customer.Email,
            customer.City,
            customer.CreatedAt,
            summaries
        );
    }

    private static CustomerResponseDto ToDto(Customer customer)
    {
        return new CustomerResponseDto(
            customer.Id,
            customer.Name,
            customer.Phone,
            customer.Email,
            customer.City,
            customer.CreatedAt
        );
    }
}