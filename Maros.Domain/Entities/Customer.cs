using Maros.Domain.Common;

namespace Maros.Domain.Entities;

public class Customer : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string City { get; set; } = string.Empty;

    public ICollection<Quotation> Quotations { get; set; } = new List<Quotation>();
}