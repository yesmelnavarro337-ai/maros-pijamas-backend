using System.ComponentModel.DataAnnotations;

namespace Maros.Application.DTOs.Categories;

public record CategoryCreateDto([Required, MaxLength(100)] string Name);