using Maros.Application.Common;

namespace Maros.Application.DTOs.Customers;

public class CustomerQueryParams : PagedQueryParams
{
    public string? City { get; set; }
    public string? Status { get; set; } // "active", "inactive", "todos"
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}