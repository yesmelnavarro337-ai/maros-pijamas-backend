using Maros.Domain.Common;

namespace Maros.Domain.Entities;

public class QuotationReferenceImage : BaseEntity
{
    public Guid QuotationId { get; set; }
    public Quotation Quotation { get; set; } = null!;
    public string Url { get; set; } = string.Empty;
}