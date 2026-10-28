using Maros.Application.Common;
using Maros.Application.Services;
using Maros.Domain.Entities;
using Maros.Domain.Interfaces;
using Moq;
using Xunit;

namespace Maros.Application.Tests.Services;

public class QuotationPriceCalculatorTests
{
    private static readonly DateTime PromoDate = new(2026, 10, 20, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Infantil_SmallSize_AppliesClean5PercentDiscountWithoutPhantomSurcharge()
    {
        // Pijama infantil estilo Short (base $65.000) en talla pequeña "4"
        var category = new Category { Name = "Niños y Bebes" };
        var product = new Product
        {
            Name = "Pijama Infantil Short Estrellas",
            BasePrice = 65000m,
            ProductCategories = new List<ProductCategory>
            {
                new() { Category = category }
            }
        };

        var item = new QuotationItem
        {
            Product = product,
            Size = "4",
            Quantity = 2
        };

        var calculation = QuotationPriceCalculator.CalculateItemPrice(item, PromoDate);

        Assert.True(calculation.IsInfantil);
        Assert.Equal(65000m, calculation.BasePrice);
        Assert.Equal(3250m, calculation.DiscountAmount); // 5% de 65.000
        Assert.Equal(0m, calculation.SizeSurcharge); // No recargo de adulto
        Assert.Equal(61750m, calculation.UnitPrice); // 65.000 - 3.250 = 61.750
        Assert.Equal(123500m, calculation.LineTotal); // 61.750 * 2
    }

    [Fact]
    public void Infantil_YouthSize_NowHasNoSurcharge_SamePriceAsSmallSizes()
    {
        // Pijama infantil Pantalón MC (base $75.000) en talla juvenil "12"
        // Las tallas juveniles ya NO tienen recargo
        var category = new Category { Name = "Infantil" };
        var style = new Style { Name = "Pantalón - Camisa manga corta (Infantil)" };
        var product = new Product
        {
            Name = "Pijama Infantil Pantalón Nubes",
            BasePrice = 75000m,
            ProductCategories = new List<ProductCategory> { new() { Category = category } },
            ProductStyles = new List<ProductStyle> { new() { Style = style } }
        };

        var item = new QuotationItem
        {
            Product = product,
            Size = "12",
            Quantity = 1
        };

        var calculation = QuotationPriceCalculator.CalculateItemPrice(item, PromoDate);

        Assert.True(calculation.IsInfantil);
        Assert.Equal(75000m, calculation.BasePrice);
        Assert.Equal(3750m, calculation.DiscountAmount); // 5% de 75.000
        Assert.Equal(0m, calculation.SizeSurcharge); // Sin recargo
        Assert.Equal(71250m, calculation.UnitPrice); // 75.000 - 3.750 = 71.250
        Assert.Equal(71250m, calculation.LineTotal);
    }

    [Theory]
    [InlineData("XL", 10000)]
    [InlineData("2XL", 25000)]
    [InlineData("XXL", 25000)]
    [InlineData("3XL", 30000)]
    [InlineData("4XL", 30000)]
    public void Adult_LargeSizes_ApplyCorrectSurchargesWithoutYouthInterference(string size, decimal expectedSurcharge)
    {
        var category = new Category { Name = "Pijamas Dama" };
        var product = new Product
        {
            Name = "Pijama Pantalón Dama Seda",
            BasePrice = 115000m,
            ProductCategories = new List<ProductCategory> { new() { Category = category } }
        };

        var item = new QuotationItem
        {
            Product = product,
            Size = size,
            Quantity = 1
        };

        var calculation = QuotationPriceCalculator.CalculateItemPrice(item, PromoDate);

        Assert.False(calculation.IsInfantil);
        Assert.Equal(115000m, calculation.BasePrice);
        Assert.Equal(5750m, calculation.DiscountAmount); // 5% de 115.000
        Assert.Equal(expectedSurcharge, calculation.SizeSurcharge);
        Assert.Equal(115000m - 5750m + expectedSurcharge, calculation.UnitPrice);
    }

    [Fact]
    public async Task WhatsAppService_BuildMessageForQuotation_IncludesUnitPricesAndTotalEstimated()
    {
        var quotationRepo = new Mock<IQuotationRepository>();
        var settingsRepo = new Mock<ISiteSettingsRepository>();

        var customer = new Customer
        {
            Name = "Carolina Gómez",
            Phone = "3001234567",
            City = "Medellín"
        };

        var category = new Category { Name = "Niños y Bebes" };
        var product = new Product
        {
            Name = "Pijama Niños Dinosaurio",
            BasePrice = 65000m,
            ProductCategories = new List<ProductCategory> { new() { Category = category } }
        };

        var quotationId = Guid.NewGuid();
        var quotation = new Quotation
        {
            Id = quotationId,
            Customer = customer,
            Items = new List<QuotationItem>
            {
                new()
                {
                    Product = product,
                    Size = "6",
                    Quantity = 2
                }
            }
        };

        quotationRepo.Setup(r => r.GetByIdAsync(quotationId)).ReturnsAsync(quotation);
        settingsRepo.Setup(r => r.GetAsync()).ReturnsAsync(new SiteSettings { WhatsappNumber = "573013169974" });

        var service = new WhatsAppService(quotationRepo.Object, settingsRepo.Object);
        var result = await service.BuildMessageForQuotationAsync(quotationId);

        Assert.NotNull(result);
        Assert.Contains("Carolina Gómez", result.Message);
        Assert.Contains("Medellín", result.Message);
        Assert.Contains("Valor unitario:", result.Message);
        Assert.Contains("Cantidad: 2 unidades", result.Message);
        Assert.Contains("VALOR ESTIMADO TOTAL:", result.Message);
        Assert.Contains("https://wa.me/573013169974", result.Link);
    }
}
