using Maros.Application.DTOs.Quotations;
using Maros.Application.Common;

namespace Maros.Application.Interfaces;

public interface IQuotationService
{
    Task<PagedResult<QuotationResponseDto>> GetAllAsync(QuotationQueryParams query);
    Task<QuotationResponseDto> GetByIdAsync(Guid id);
    Task<QuotationResponseDto> CreateAsync(QuotationCreateDto request);
    Task<QuotationResponseDto> UpdateStatusAsync(Guid id, QuotationStatusUpdateDto request);
}