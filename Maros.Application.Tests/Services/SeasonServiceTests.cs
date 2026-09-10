using Maros.Application.Common;
using Maros.Application.DTOs.Seasons;
using Maros.Application.Services;
using Maros.Domain.Entities;
using Maros.Domain.Enums;
using Maros.Domain.Interfaces;
using Moq;
using Xunit;

namespace Maros.Application.Tests.Services;

public class SeasonServiceTests
{
    private readonly Mock<ISeasonRepository> _seasonRepository = new();
    private readonly Mock<ICollectionRepository> _collectionRepository = new();
    private readonly SeasonService _sut;

    public SeasonServiceTests()
    {
        _sut = new SeasonService(_seasonRepository.Object, _collectionRepository.Object);
    }

    [Fact]
    public async Task CreateAsync_FechaFinAnteriorAInicio_LanzaAppException()
    {
        var request = new SeasonCreateDto(
            "Navidad", Guid.NewGuid(),
            new DateTime(2026, 12, 31), new DateTime(2026, 11, 15), // fin ANTES que inicio
            "Título", "Subtítulo", null, null,
            new SeasonColorsDto("#000000", "#111111", "#222222"),
            "CTA", "/link", new List<Guid>()
        );

        var ex = await Assert.ThrowsAsync<AppException>(() => _sut.CreateAsync(request));
        Assert.Equal(400, ex.StatusCode);
    }

    [Fact]
    public async Task CreateAsync_ColeccionInexistente_LanzaAppException()
    {
        _collectionRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Collection?)null);

        var request = new SeasonCreateDto(
            "Navidad", Guid.NewGuid(),
            new DateTime(2026, 11, 15), new DateTime(2026, 12, 31),
            "Título", "Subtítulo", null, null,
            new SeasonColorsDto("#000000", "#111111", "#222222"),
            "CTA", "/link", new List<Guid>()
        );

        var ex = await Assert.ThrowsAsync<AppException>(() => _sut.CreateAsync(request));
        Assert.Equal(400, ex.StatusCode);
    }

    [Fact]
    public async Task CreateAsync_ProductoDestacadoFueraDeLaColeccion_LanzaAppException()
    {
        var productoFueraDeColeccion = Guid.NewGuid();
        var collection = new Collection { Id = Guid.NewGuid() }; // sin ProductCollections asociados

        _collectionRepository.Setup(r => r.GetByIdAsync(collection.Id)).ReturnsAsync(collection);
        _seasonRepository.Setup(r => r.SlugExistsAsync(It.IsAny<string>(), null)).ReturnsAsync(false);

        var request = new SeasonCreateDto(
            "Navidad", collection.Id,
            new DateTime(2026, 11, 15), new DateTime(2026, 12, 31),
            "Título", "Subtítulo", null, null,
            new SeasonColorsDto("#000000", "#111111", "#222222"),
            "CTA", "/link", new List<Guid> { productoFueraDeColeccion }
        );

        var ex = await Assert.ThrowsAsync<AppException>(() => _sut.CreateAsync(request));
        Assert.Contains("colección", ex.Message);
    }

    [Fact]
    public async Task ActivateAsync_DesactivaLasDemasTemporadasActivas()
    {
        var seasonId = Guid.NewGuid();
        var season = new Season { Id = seasonId, Status = SeasonStatus.Programada, Collection = new Collection() };

        _seasonRepository.Setup(r => r.GetByIdAsync(seasonId)).ReturnsAsync(season);

        await _sut.ActivateAsync(seasonId);

        // La regla exclusiva se delega al repositorio (DeactivateAllExceptAsync),
        // así que verificamos que el servicio SIEMPRE la invoca antes de activar.
        _seasonRepository.Verify(r => r.DeactivateAllExceptAsync(seasonId), Times.Once);
        Assert.Equal(SeasonStatus.Activa, season.Status);
    }
}