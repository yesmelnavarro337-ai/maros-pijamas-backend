using System.ComponentModel.DataAnnotations;

namespace Maros.Application.DTOs.Quotations;

public record QuotationItemInputDto(
    Guid? ProductId,
    [MaxLength(20)] string Size,
    [Range(1, int.MaxValue)] int Quantity,
    List<Guid> CustomizationOptionIds,
    string? EmbroideryText
);