using Maros.Application.Common;
using Maros.Application.DTOs.Customers;
using Maros.Application.Interfaces;
using Maros.Domain.Entities;
using Maros.Domain.Enums;
using Maros.Domain.Interfaces;

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

    public async Task<PagedResult<CustomerResponseDto>> GetAllAsync(CustomerQueryParams query)
    {
        var customers = _customerRepository.QueryAll();

        // 1. Search filter
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            customers = customers.Where(c =>
                c.Name.ToLower().Contains(search) ||
                c.Phone.ToLower().Contains(search) ||
                (c.Email != null && c.Email.ToLower().Contains(search)) ||
                c.City.ToLower().Contains(search));
        }

        // 2. City filter
        if (!string.IsNullOrWhiteSpace(query.City) && query.City != "todas")
        {
            customers = customers.Where(c => c.City.ToLower() == query.City.ToLower());
        }

        // 3. Date Range filter
        if (query.FromDate.HasValue)
        {
            customers = customers.Where(c => c.CreatedAt >= query.FromDate.Value);
        }
        if (query.ToDate.HasValue)
        {
            customers = customers.Where(c => c.CreatedAt <= query.ToDate.Value.AddDays(1));
        }

        // 4. Sorting
        var sortBy = query.SortBy?.ToLowerInvariant();
        customers = sortBy switch
        {
            "name" => query.SortDescending
                ? customers.OrderByDescending(c => c.Name)
                : customers.OrderBy(c => c.Name),
            "date" => query.SortDescending
                ? customers.OrderByDescending(c => c.CreatedAt)
                : customers.OrderBy(c => c.CreatedAt),
            _ => query.SortDescending
                ? customers.OrderByDescending(c => c.CreatedAt)
                : customers.OrderByDescending(c => c.CreatedAt)
        };

        var paged = await _paginationService.PaginateAsync(customers, query.PageNumber, query.PageSize);

        var dtos = paged.Items.Select(c =>
        {
            var totalQuotations = c.Quotations.Count;
            var lastQuotation = c.Quotations.OrderByDescending(q => q.CreatedAt).FirstOrDefault();
            var lastActivity = lastQuotation?.CreatedAt ?? c.CreatedAt;
            var isActive = (DateTime.UtcNow - lastActivity).TotalDays <= 90;

            return new CustomerResponseDto(
                c.Id,
                c.Name,
                c.Phone,
                c.Email,
                c.City,
                c.CreatedAt,
                isActive,
                totalQuotations,
                lastActivity
            );
        }).ToList();

        // Status post-filtering if requested
        if (!string.IsNullOrWhiteSpace(query.Status) && query.Status != "todos")
        {
            var wantActive = query.Status.ToLower() == "active" || query.Status.ToLower() == "activo";
            dtos = dtos.Where(d => d.IsActive == wantActive).ToList();
        }

        return new PagedResult<CustomerResponseDto>(
            dtos,
            paged.PageNumber,
            paged.PageSize,
            dtos.Count);
    }

    public async Task<CustomerWithQuotationsDto> GetByIdWithQuotationsAsync(Guid id)
    {
        var customer = await _customerRepository.GetByIdAsync(id)
            ?? throw new AppException("Cliente no encontrado.", 404);

        var quotations = await _quotationRepository.GetByCustomerIdAsync(id);

        var quotationSummaries = quotations.Select(q =>
        {
            var folio = $"COT-{q.Id.ToString()[..8].ToUpper()}";
            var productNames = q.Items
                .Select(i => i.Product?.Name ?? "Producto personalizado")
                .Distinct()
                .ToList();

            var badges = new HashSet<string>();
            foreach (var item in q.Items)
            {
                if (!string.IsNullOrWhiteSpace(item.EmbroideryText)) badges.Add("Bordado");
                foreach (var opt in item.SelectedOptions)
                {
                    if (opt.CustomizationOption != null)
                    {
                        var type = opt.CustomizationOption.CatalogType.ToString();
                        if (type == "Bordado") badges.Add("Bordado");
                        else if (type == "Estampado") badges.Add("Estampado");
                        else if (type == "Modelo") badges.Add("Modelo");
                        else badges.Add(opt.CustomizationOption.Name);
                    }
                }
            }
            if (badges.Count == 0) badges.Add("Estándar");

            var totalAmount = q.Items.Sum(i =>
                ((i.Product?.BasePrice ?? 0) + i.SelectedOptions.Sum(o => o.CustomizationOption?.PriceModifier ?? 0)) * i.Quantity);

            return new CustomerQuotationSummaryDto(
                q.Id,
                folio,
                q.Status.ToString(),
                q.CreatedAt,
                productNames,
                badges.ToList(),
                totalAmount
            );
        }).OrderByDescending(q => q.CreatedAt).ToList();

        // Build Activity Logs Timeline
        var activityLogs = new List<CustomerActivityLogDto>
        {
            new CustomerActivityLogDto(
                $"reg-{customer.Id}",
                "registered",
                "Cliente registrado",
                $"Alta inicial de {customer.Name} en la plataforma",
                customer.CreatedAt
            )
        };

        foreach (var q in quotations)
        {
            var folio = $"COT-{q.Id.ToString()[..8].ToUpper()}";
            activityLogs.Add(new CustomerActivityLogDto(
                $"quot-{q.Id}",
                "quotation_sent",
                $"Solicitud de cotización {folio}",
                $"Productos: {string.Join(", ", q.Items.Select(i => i.Product?.Name ?? "Producto"))}",
                q.CreatedAt
            ));

            if (q.Status == QuotationStatus.Cotizada || q.Status == QuotationStatus.Aceptada)
            {
                activityLogs.Add(new CustomerActivityLogDto(
                    $"appr-{q.Id}",
                    "quotation_approved",
                    $"Cotización {folio} enviada/aprobada",
                    $"Estado actualizado a {q.Status}",
                    q.UpdatedAt ?? q.CreatedAt
                ));
            }
        }

        activityLogs = activityLogs.OrderByDescending(a => a.Timestamp).ToList();

        var lastActivityAt = activityLogs.FirstOrDefault()?.Timestamp ?? customer.CreatedAt;
        var isActive = (DateTime.UtcNow - lastActivityAt).TotalDays <= 90;

        return new CustomerWithQuotationsDto(
            customer.Id,
            customer.Name,
            customer.Phone,
            customer.Email,
            customer.City,
            customer.CreatedAt,
            isActive,
            quotations.Count,
            0, // Total messages
            lastActivityAt,
            quotationSummaries,
            activityLogs
        );
    }

    public async Task<CustomerResponseDto> CreateAsync(CustomerCreateDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new AppException("El nombre del cliente es obligatorio.", 400);

        if (string.IsNullOrWhiteSpace(request.Phone))
            throw new AppException("El teléfono del cliente es obligatorio.", 400);

        var existing = await _customerRepository.GetByPhoneAsync(request.Phone);
        if (existing != null)
            throw new AppException("Ya existe un cliente registrado con ese número de teléfono.", 400);

        var customer = new Customer
        {
            Name = request.Name.Trim(),
            Phone = request.Phone.Trim(),
            Email = request.Email?.Trim(),
            City = string.IsNullOrWhiteSpace(request.City) ? "Valledupar" : request.City.Trim(),
        };

        await _customerRepository.AddAsync(customer);
        await _customerRepository.SaveChangesAsync();

        return new CustomerResponseDto(
            customer.Id,
            customer.Name,
            customer.Phone,
            customer.Email,
            customer.City,
            customer.CreatedAt,
            true,
            0,
            customer.CreatedAt
        );
    }

    public async Task<CustomerResponseDto> UpdateAsync(Guid id, CustomerUpdateDto request)
    {
        var customer = await _customerRepository.GetByIdAsync(id)
            ?? throw new AppException("Cliente no encontrado.", 404);

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new AppException("El nombre del cliente es obligatorio.", 400);

        if (string.IsNullOrWhiteSpace(request.Phone))
            throw new AppException("El teléfono del cliente es obligatorio.", 400);

        customer.Name = request.Name.Trim();
        customer.Phone = request.Phone.Trim();
        customer.Email = request.Email?.Trim();
        customer.City = string.IsNullOrWhiteSpace(request.City) ? customer.City : request.City.Trim();

        await _customerRepository.SaveChangesAsync();

        return new CustomerResponseDto(
            customer.Id,
            customer.Name,
            customer.Phone,
            customer.Email,
            customer.City,
            customer.CreatedAt,
            true,
            customer.Quotations.Count,
            customer.CreatedAt
        );
    }

    public async Task<List<string>> GetCitiesAsync()
    {
        var cities = _customerRepository.QueryAll()
            .Select(c => c.City)
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Distinct()
            .OrderBy(c => c)
            .ToList();

        if (!cities.Contains("Valledupar"))
            cities.Insert(0, "Valledupar");

        return cities;
    }
}