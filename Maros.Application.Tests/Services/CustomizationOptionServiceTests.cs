using Maros.Application.Common;
using Maros.Application.DTOs.Customization;
using Maros.Application.Services;
using Maros.Domain.Interfaces;
using Moq;
using Xunit;

namespace Maros.Application.Tests.Services;

public class CustomizationOptionServiceTests
{
    private readonly Mock<ICustomizationOptionRepository> _repository = new();
    private readonly CustomizationOptionService _sut;

    public CustomizationOptionServiceTests()
    {
        _sut = new CustomizationOptionService(_repository.Object);
    }

    [Fact]
    public async Task CreateAsync_ColorSinColorHex_LanzaAppException()
    {
        var request = new CustomizationOptionCreateDto("Color", "Verde Oliva", null, null, null);

        var ex = await Assert.ThrowsAsync<AppException>(() => _sut.CreateAsync(request));
        Assert.Contains("color", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_TallaConRecargoDePrecio_LanzaAppException()
    {
        var request = new CustomizationOptionCreateDto("Talla", "M", null, null, 15000);

        var ex = await Assert.ThrowsAsync<AppException>(() => _sut.CreateAsync(request));
        Assert.Contains("recargo", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_TelaConRecargoDePrecio_SeCreaCorrectamente()
    {
        var request = new CustomizationOptionCreateDto("Tela", "Satín", null, null, 20000);

        var result = await _sut.CreateAsync(request);

        Assert.Equal("Satín", result.Name);
        Assert.Equal(20000, result.PriceModifier);
        _repository.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.CustomizationOption>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_TipoDeCatalogoInvalido_LanzaAppException()
    {
        var request = new CustomizationOptionCreateDto("TipoInventado", "X", null, null, null);

        await Assert.ThrowsAsync<AppException>(() => _sut.CreateAsync(request));
    }
}