using Maros.Application.Common;
using Maros.Application.DTOs.Quotations;
using Maros.Application.Interfaces;
using Maros.Domain.Entities;
using Maros.Domain.Enums;
using Maros.Domain.Interfaces;

namespace Maros.Application.Services;

public class QuotationService : IQuotationService
{
    private readonly IQuotationRepository _quotationRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IPaginationService _paginationService;

    public QuotationService(
        IQuotationRepository quotationRepository,
        ICustomerRepository customerRepository,
        IPaginationService paginationService)
    {
        _quotationRepository = quotationRepository;
        _customerRepository = customerRepository;
        _paginationService = paginationService;
    }

    public async Task<PagedResult<QuotationResponseDto>> GetAllAsync(QuotationQueryParams query)
    {
        var quotations = _quotationRepository.QueryAll();

        if (!string.IsNullOrWhiteSpace(query.Status) &&
            Enum.TryParse<QuotationStatus>(query.Status, ignoreCase: true, out var status))
            quotations = quotations.Where(q => q.Status == status);

        if (!string.IsNullOrWhiteSpace(query.Search))
            quotations = quotations.Where(q => q.Customer!.Name.Contains(query.Search));

        quotations = query.SortDescending
            ? quotations.OrderByDescending(q => q.CreatedAt)
            : quotations.OrderBy(q => q.CreatedAt);

        var paged = await _paginationService.PaginateAsync(quotations, query.PageNumber, query.PageSize);
        return new PagedResult<QuotationResponseDto>(
            paged.Items.Select(ToDto).ToList(),
            paged.PageNumber,
            paged.PageSize,
            paged.TotalCount);
    }

    public async Task<QuotationResponseDto> GetByIdAsync(Guid id)
    {
        var quotation = await _quotationRepository.GetByIdAsync(id)
            ?? throw new AppException("Cotización no encontrada.", 404);
        return ToDto(quotation);
    }

    public async Task<QuotationResponseDto> CreateAsync(QuotationCreateDto request)
    {
        if (request.Items.Count == 0)
            throw new AppException("La cotización debe incluir al menos un producto.", 400);

        var customer = await _customerRepository.GetByPhoneAsync(request.CustomerPhone);
        if (customer is null)
        {
            customer = new Customer
            {
                Name = request.CustomerName,
                Phone = request.CustomerPhone,
                Email = request.CustomerEmail,
                City = request.CustomerCity,
            };
            await _customerRepository.AddAsync(customer);
        }

        var quotation = new Quotation
        {
            CustomerId = customer.Id,
            Customer = customer,
            Status = QuotationStatus.Nueva,
            Notes = request.Notes,
        };

        foreach (var itemInput in request.Items)
        {
            var item = new QuotationItem
            {
                ProductId = itemInput.ProductId,
                Size = itemInput.Size,
                Quantity = itemInput.Quantity,
                EmbroideryText = itemInput.EmbroideryText,
            };

            foreach (var optionId in itemInput.CustomizationOptionIds.Distinct())
                item.SelectedOptions.Add(new QuotationItemOption { CustomizationOptionId = optionId });

            quotation.Items.Add(item);
        }

        foreach (var url in request.ReferenceImageUrls)
            quotation.ReferenceImages.Add(new QuotationReferenceImage { Url = url });

        await _quotationRepository.AddAsync(quotation);
        await _quotationRepository.SaveChangesAsync();

        var created = await _quotationRepository.GetByIdAsync(quotation.Id);
        return ToDto(created!);
    }

    public async Task<QuotationResponseDto> UpdateStatusAsync(Guid id, QuotationStatusUpdateDto request)
    {
        var quotation = await _quotationRepository.GetByIdAsync(id)
            ?? throw new AppException("Cotización no encontrada.", 404);

        if (!Enum.TryParse<QuotationStatus>(request.Status, ignoreCase: true, out var status))
            throw new AppException("Estado de cotización inválido.", 400);

        quotation.Status = status;
        quotation.UpdatedAt = DateTime.UtcNow;

        await _quotationRepository.SaveChangesAsync();
        return ToDto(quotation);
    }

    private static QuotationResponseDto ToDto(Quotation q) => new(
        q.Id,
        q.CustomerId,
        q.Customer?.Name ?? string.Empty,
        q.Customer?.Phone ?? string.Empty,
        q.Customer?.City ?? string.Empty,
        q.Status.ToString(),
        q.Notes,
        q.Items.Select(i => new QuotationItemResponseDto(
            i.Id,
            i.ProductId,
            i.Product?.Name ?? "Producto eliminado",
            i.Product != null && i.Product.Images.Any()
                ? i.Product.Images.OrderBy(img => img.Order).FirstOrDefault()?.Url ?? i.Product.Images.First().Url
                : "/placeholder.png",
            i.Size,
            i.Quantity,
            i.SelectedOptions.Select(o => new QuotationItemOptionDto(
                o.CustomizationOption.Id,
                o.CustomizationOption.CatalogType.ToString(),
                o.CustomizationOption.Name
            )).ToList(),
            i.EmbroideryText,
            (i.Product?.BasePrice ?? 0)
                + i.SelectedOptions.Sum(o => o.CustomizationOption.PriceModifier ?? 0)
        )).ToList(),
        q.ReferenceImages.Select(r => r.Url).ToList(),
        q.CreatedAt,
        q.Customer?.Email,
        q.UpdatedAt ?? q.CreatedAt
    );
}
