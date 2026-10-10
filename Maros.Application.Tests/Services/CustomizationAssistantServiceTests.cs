using Maros.Application.Common;
using Maros.Application.DTOs.Customization;
using Maros.Application.Interfaces;
using Maros.Application.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Maros.Application.Tests.Services;

public class CustomizationAssistantServiceTests
{
    private readonly Mock<IGeminiClient> _gemini = new();
    private readonly Mock<ICustomizationOptionService> _customization = new();
    private readonly Mock<IProductService> _products = new();
    private readonly CustomizationAssistantService _sut;

    private static readonly Guid TelaId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid ColorId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    public CustomizationAssistantServiceTests()
    {
        _gemini.SetupGet(g => g.IsEnabled).Returns(true);

        _customization
            .Setup(s => s.GetPublicCatalogAsync())
            .ReturnsAsync(new CustomizationCatalogPublicDto(new Dictionary<string, List<CustomizationOptionPublicDto>>
            {
                ["Tela"] = new() { new CustomizationOptionPublicDto(TelaId, "Satín", null, null, 20000) },
                ["Color"] = new() { new CustomizationOptionPublicDto(ColorId, "Verde Oliva", null, "#6B6832", null) },
                ["Estampado"] = new(),
                ["Bordado"] = new(),
            }));

        _sut = new CustomizationAssistantService(
            _gemini.Object,
            _customization.Object,
            _products.Object,
            NullLogger<CustomizationAssistantService>.Instance);
    }

    private void SetupGeminiResponse(string json) =>
        _gemini
            .Setup(g => g.GenerateJsonAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(json);

    [Fact]
    public void IsEnabled_DelegaEnElClienteGemini()
    {
        Assert.True(_sut.IsEnabled);
    }

    [Fact]
    public void Model_DelegaEnElClienteGemini()
    {
        _gemini.SetupGet(g => g.Model).Returns("gemini-2.5-flash");

        Assert.Equal("gemini-2.5-flash", _sut.Model);
    }

    [Fact]
    public async Task SuggestAsync_MensajeVacio_LanzaAppException()
    {
        var ex = await Assert.ThrowsAsync<AppException>(() =>
            _sut.SuggestAsync(new CustomizationAssistantRequestDto("   ")));

        Assert.Equal(400, ex.StatusCode);
        _gemini.Verify(g => g.GenerateJsonAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SuggestAsync_IdsValidos_SeMapeanEnLaRespuesta()
    {
        SetupGeminiResponse($$"""
            {"reply":"Va bien con verde.","telaId":"{{TelaId}}","colorId":"{{ColorId}}","estampadoId":"","bordadoId":"","embroideryText":"Luna"}
            """);

        var result = await _sut.SuggestAsync(new CustomizationAssistantRequestDto("algo elegante"));

        Assert.Equal(TelaId, result.Suggestion.TelaId);
        Assert.Equal(ColorId, result.Suggestion.ColorId);
        Assert.Null(result.Suggestion.EstampadoId);
        Assert.Equal("Luna", result.EmbroideryText);
    }

    [Fact]
    public async Task SuggestAsync_IdInexistente_SeDescarta()
    {
        var inventado = Guid.Parse("99999999-9999-9999-9999-999999999999");
        SetupGeminiResponse($$"""
            {"reply":"ok","telaId":"{{inventado}}","colorId":"","estampadoId":"","bordadoId":"","embroideryText":""}
            """);

        var result = await _sut.SuggestAsync(new CustomizationAssistantRequestDto("algo"));

        Assert.Null(result.Suggestion.TelaId);
        Assert.Null(result.EmbroideryText);
    }

    [Fact]
    public async Task SuggestAsync_RespuestaEntreCercasDeCodigo_SeParsea()
    {
        SetupGeminiResponse($$"""
            ```json
            {"reply":"ok","telaId":"{{TelaId}}","colorId":"","estampadoId":"","bordadoId":"","embroideryText":""}
            ```
            """);

        var result = await _sut.SuggestAsync(new CustomizationAssistantRequestDto("algo"));

        Assert.Equal(TelaId, result.Suggestion.TelaId);
    }

    [Fact]
    public async Task SuggestAsync_ProductoNoEncontrado_UsaCatalogoCompleto()
    {
        _products
            .Setup(p => p.GetPublicDetailBySlugAsync("no-existe"))
            .ThrowsAsync(new AppException("Producto no encontrado.", 404));

        SetupGeminiResponse($$"""
            {"reply":"ok","telaId":"{{TelaId}}","colorId":"","estampadoId":"","bordadoId":"","embroideryText":""}
            """);

        var result = await _sut.SuggestAsync(
            new CustomizationAssistantRequestDto("algo", ProductSlug: "no-existe"));

        Assert.Equal(TelaId, result.Suggestion.TelaId);
    }

    [Fact]
    public async Task SuggestAsync_SinOpciones_LanzaAppException()
    {
        _customization
            .Setup(s => s.GetPublicCatalogAsync())
            .ReturnsAsync(new CustomizationCatalogPublicDto(new Dictionary<string, List<CustomizationOptionPublicDto>>()));

        var ex = await Assert.ThrowsAsync<AppException>(() =>
            _sut.SuggestAsync(new CustomizationAssistantRequestDto("algo")));

        Assert.Equal(409, ex.StatusCode);
    }
}
