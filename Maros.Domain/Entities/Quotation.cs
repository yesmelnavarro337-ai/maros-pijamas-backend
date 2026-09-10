using Maros.Domain.Common;
using Maros.Domain.Enums;

namespace Maros.Domain.Entities;

public class Quotation : BaseEntity
{
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public QuotationStatus Status { get; set; } = QuotationStatus.Nueva;
    public string Notes { get; set; } = string.Empty;

    public ICollection<QuotationItem> Items { get; set; } = new List<QuotationItem>();
    public ICollection<QuotationReferenceImage> ReferenceImages { get; set; } = new List<QuotationReferenceImage>();
}