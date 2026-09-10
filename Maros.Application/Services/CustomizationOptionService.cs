using Maros.Application.Common;
using Maros.Application.DTOs.Customization;
using Maros.Application.Interfaces;
using Maros.Domain.Entities;
using Maros.Domain.Enums;
using Maros.Domain.Interfaces;

namespace Maros.Application.Services;

public class CustomizationOptionService : ICustomizationOptionService
{
    private readonly ICustomizationOptionRepository _repository;

    public CustomizationOptionService(ICustomizationOptionRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CustomizationOptionResponseDto>> GetAllAsync()
    {
        var options = await _repository.GetAllAsync();
        return options.Select(ToDto).ToList();
    }

    public async Task<CustomizationOptionResponseDto> CreateAsync(CustomizationOptionCreateDto request)
    {
        var catalogType = ParseCatalogType(request.CatalogType);
        var rules = CustomizationCatalogRules.For(catalogType);

        ValidateAgainstRules(rules, request.ImageUrl, request.ColorHex, request.PriceModifier);

        var option = new CustomizationOption
        {
            CatalogType = catalogType,
            Name = request.Name,
            ImageUrl = rules.HasImage ? request.ImageUrl : null,
            ColorHex = rules.HasColor ? request.ColorHex : null,
            PriceModifier = rules.HasPriceModifier ? request.PriceModifier : null,
            Active = true,
        };

        await _repository.AddAsync(option);
        await _repository.SaveChangesAsync();

        return ToDto(option);
    }

    public async Task<CustomizationOptionResponseDto> UpdateAsync(Guid id, CustomizationOptionUpdateDto request)
    {
        var option = await _repository.GetByIdAsync(id)
            ?? throw new AppException("Opción de personalización no encontrada.", 404);

        var rules = CustomizationCatalogRules.For(option.CatalogType);
        ValidateAgainstRules(rules, request.ImageUrl, request.ColorHex, request.PriceModifier);

        option.Name = request.Name;
        option.ImageUrl = rules.HasImage ? request.ImageUrl : null;
        option.ColorHex = rules.HasColor ? request.ColorHex : null;
        option.PriceModifier = rules.HasPriceModifier ? request.PriceModifier : null;
        option.Active = request.Active;
        option.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();
        return ToDto(option);
    }

    public async Task RemoveAsync(Guid id)
    {
        var option = await _repository.GetByIdAsync(id)
            ?? throw new AppException("Opción de personalización no encontrada.", 404);

        _repository.Remove(option);
        await _repository.SaveChangesAsync();
    }

    public async Task<CustomizationCatalogPublicDto> GetPublicCatalogAsync()
    {
        var options = await _repository.GetActiveAsync();

        var grouped = options
            .GroupBy(o => o.CatalogType.ToString())
            .ToDictionary(
                g => g.Key,
                g => g.Select(o => new CustomizationOptionPublicDto(o.Id, o.Name, o.ImageUrl, o.ColorHex, o.PriceModifier)).ToList()
            );

        return new CustomizationCatalogPublicDto(grouped);
    }

    // ─── Helpers privados ──────────────────────────────────────────

    private static CustomizationCatalogType ParseCatalogType(string input)
    {
        if (!Enum.TryParse<CustomizationCatalogType>(input, ignoreCase: true, out var type))
            throw new AppException("Tipo de catálogo inválido.", 400);
        return type;
    }

    private static void ValidateAgainstRules(
        Application.Common.CatalogTypeRules rules,
        string? imageUrl,
        string? colorHex,
        decimal? priceModifier)
    {
        if (rules.HasColor && string.IsNullOrWhiteSpace(colorHex))
            throw new AppException("Este tipo de catálogo requiere un color.", 400);

        if (!rules.HasColor && !string.IsNullOrWhiteSpace(colorHex))
            throw new AppException("Este tipo de catálogo no admite color.", 400);

        if (!rules.HasImage && !string.IsNullOrWhiteSpace(imageUrl))
            throw new AppException("Este tipo de catálogo no admite imagen.", 400);

        if (!rules.HasPriceModifier && priceModifier.HasValue && priceModifier.Value != 0)
            throw new AppException("Este tipo de catálogo no admite recargo de precio.", 400);
    }

    private static CustomizationOptionResponseDto ToDto(CustomizationOption o) => new(
        o.Id,
        o.CatalogType.ToString(),
        o.Name,
        o.ImageUrl,
        o.ColorHex,
        o.PriceModifier,
        o.Active
    );
}