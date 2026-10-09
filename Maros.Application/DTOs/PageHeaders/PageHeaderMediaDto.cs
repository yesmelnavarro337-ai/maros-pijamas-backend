using System.ComponentModel.DataAnnotations;

namespace Maros.Application.DTOs.PageHeaders;

/// <summary>
/// Elemento multimedia de un header: imagen o video con su orden de reproducción.
/// </summary>
public record PageHeaderMediaDto(
    [Required, MaxLength(1000)] string Url,
    [Required, MaxLength(10)] string MediaType,
    int Order
);
