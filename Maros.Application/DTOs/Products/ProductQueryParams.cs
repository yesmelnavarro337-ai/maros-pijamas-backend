using Maros.Application.Common;

namespace Maros.Application.DTOs.Products;

public class ProductQueryParams : PagedQueryParams
{
    public Guid? CategoryId { get; set; }
    public string? Status { get; set; }
    public Guid? SeasonId { get; set; }
}