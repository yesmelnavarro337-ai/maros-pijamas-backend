using Maros.Domain.Enums;

namespace Maros.Application.Common;

public record CatalogTypeRules(bool HasImage, bool HasColor, bool HasPriceModifier);

public static class CustomizationCatalogRules
{
    private static readonly Dictionary<CustomizationCatalogType, CatalogTypeRules> Rules = new()
    {
        [CustomizationCatalogType.Modelo] = new(HasImage: true, HasColor: false, HasPriceModifier: false),
        [CustomizationCatalogType.Tela] = new(HasImage: true, HasColor: false, HasPriceModifier: true),
        [CustomizationCatalogType.Color] = new(HasImage: false, HasColor: true, HasPriceModifier: false),
        [CustomizationCatalogType.Estampado] = new(HasImage: true, HasColor: false, HasPriceModifier: true),
        [CustomizationCatalogType.Bordado] = new(HasImage: true, HasColor: false, HasPriceModifier: true),
        [CustomizationCatalogType.Talla] = new(HasImage: false, HasColor: false, HasPriceModifier: false),
    };

    public static CatalogTypeRules For(CustomizationCatalogType type) => Rules[type];
}