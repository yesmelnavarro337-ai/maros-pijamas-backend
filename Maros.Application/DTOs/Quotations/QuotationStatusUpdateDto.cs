using System.ComponentModel.DataAnnotations;

namespace Maros.Application.DTOs.Quotations;

public record QuotationStatusUpdateDto([Required] string Status);