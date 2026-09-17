using Maros.Application.Common;
using Maros.Application.DTOs.Products;
using Maros.Application.Interfaces;
using Maros.Application.Services;
using Maros.Domain.Entities;
using Maros.Domain.Interfaces;
using Moq;
using Xunit;

namespace Maros.Application.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepository = new();
    private readonly Mock<ICategoryRepository> _categoryRepository = new();
    private readonly Mock<ISeasonRepository> _seasonRepository = new();
    private readonly Mock<IPaginationService> _paginationService = new();
    private readonly ProductService _sut;

    public ProductServiceTests()
    {
        _sut = new ProductService(_productRepository.Object, _categoryRepository.Object, _seasonRepository.Object, _paginationService.Object);
    }

    [Fact]
    public async Task CreateAsync_SkuDuplicadoDentroDelMismoRequest_LanzaAppException()
    {
        var category = new Category { Id = Guid.NewGuid() };
        _categoryRepository.Setup(r => r.GetByIdAsync(category.Id)).ReturnsAsync(category);

        var request = new ProductCreateDto(
            "Pijama Test", category.Id, "Descripción", 100000, "Activo",
            false, true, "5-7 días", "SEO", "SEO desc", null, null,
            new List<string>(),
            new List<ProductVariantInputDto>
            {
                new("S", "Beige", "#EFE8D8", "SKU-REPETIDO", 5, null),
                new("M", "Beige", "#EFE8D8", "SKU-REPETIDO", 5, null), // mismo SKU, dos veces
            },
            new List<Guid>()
        );

        var ex = await Assert.ThrowsAsync<AppException>(() => _sut.CreateAsync(request));
        Assert.Contains("repetido", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_SkuYaExisteEnOtroProducto_LanzaAppException409()
    {
        var category = new Category { Id = Guid.NewGuid() };
        _categoryRepository.Setup(r => r.GetByIdAsync(category.Id)).ReturnsAsync(category);
        _productRepository.Setup(r => r.SkuExistsAsync("SKU-EXISTENTE", null)).ReturnsAsync(true);

        var request = new ProductCreateDto(
            "Pijama Test", category.Id, "Descripción", 100000, "Activo",
            false, true, "5-7 días", "SEO", "SEO desc", null, null,
            new List<string>(),
            new List<ProductVariantInputDto> { new("S", "Beige", "#EFE8D8", "SKU-EXISTENTE", 5, null) },
            new List<Guid>()
        );

        var ex = await Assert.ThrowsAsync<AppException>(() => _sut.CreateAsync(request));
        Assert.Equal(409, ex.StatusCode);
    }

    [Fact]
    public async Task CreateAsync_CategoriaInexistente_LanzaAppException400()
    {
        _categoryRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Category?)null);

        var request = new ProductCreateDto(
            "Pijama Test", Guid.NewGuid(), "Descripción", 100000, "Activo",
            false, true, "5-7 días", "SEO", "SEO desc", null, null,
            new List<string>(), new List<ProductVariantInputDto>(), new List<Guid>()
        );

        var ex = await Assert.ThrowsAsync<AppException>(() => _sut.CreateAsync(request));
        Assert.Equal(400, ex.StatusCode);
    }
}