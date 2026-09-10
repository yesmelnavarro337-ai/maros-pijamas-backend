using Maros.Application.Common;

namespace Maros.Application.DTOs.Quotations;

public class QuotationQueryParams : PagedQueryParams
{
    public string? Status { get; set; }
}