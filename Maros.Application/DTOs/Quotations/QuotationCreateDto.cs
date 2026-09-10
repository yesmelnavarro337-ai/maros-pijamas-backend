using System.ComponentModel.DataAnnotations;

namespace Maros.Application.DTOs.Quotations;

public record QuotationCreateDto(
    [Required, MaxLength(150)] string CustomerName,
    [Required, MaxLength(30)] string CustomerPhone,
    [MaxLength(256)] string? CustomerEmail,
    [MaxLength(100)] string CustomerCity,
    [Required, MinLength(1)] List<QuotationItemInputDto> Items,
    List<string> ReferenceImageUrls,
    [MaxLength(1000)] string Notes
);