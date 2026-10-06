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

    [Fact]
    public async Task CreateAsync_ConvierteFechasAUtc_YSeteaCreatedAtUtc()
    {
        var collectionId = Guid.NewGuid();
        var collection = new Collection { Id = collectionId };

        _collectionRepository.Setup(r => r.GetByIdAsync(collectionId)).ReturnsAsync(collection);
        _seasonRepository.Setup(r => r.SlugExistsAsync(It.IsAny<string>(), null)).ReturnsAsync(false);

        Season? addedSeason = null;
        _seasonRepository.Setup(r => r.AddAsync(It.IsAny<Season>()))
            .Callback<Season>(s => addedSeason = s)
            .Returns(Task.CompletedTask);

        _seasonRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Guid id) => addedSeason ?? new Season { Id = id });

        var request = new SeasonCreateDto(
            "Verano", collectionId,
            new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Unspecified),
            new DateTime(2026, 8, 31, 23, 59, 59, DateTimeKind.Unspecified),
            "Verano 2026", "Subtítulo", null, null,
            new SeasonColorsDto("#FFFFFF", "#000000", "#CCCCCC"),
            "CTA", "/link", new List<Guid>()
        );

        await _sut.CreateAsync(request);

        Assert.NotNull(addedSeason);
        Assert.Equal(DateTimeKind.Utc, addedSeason.StartDate.Kind);
        Assert.Equal(DateTimeKind.Utc, addedSeason.EndDate.Kind);
        Assert.Equal(DateTimeKind.Utc, addedSeason.CreatedAt.Kind);
    }

    private (SeasonUpdateDto Request, Collection Collection, Season Season) SetUpUpdate()
    {
        var collection = new Collection { Id = Guid.NewGuid() };
        var season = new Season { Id = Guid.NewGuid(), Collection = collection };

        _collectionRepository.Setup(r => r.GetByIdAsync(collection.Id)).ReturnsAsync(collection);
        _seasonRepository.Setup(r => r.SlugExistsAsync(It.IsAny<string>(), It.IsAny<Guid?>())).ReturnsAsync(false);
        _seasonRepository.Setup(r => r.GetByIdAsync(season.Id)).ReturnsAsync(season);
        _seasonRepository.Setup(r => r.GetByIdReadOnlyAsync(season.Id)).ReturnsAsync(season);

        var request = new SeasonUpdateDto(
            "Verano", collection.Id,
            new DateTime(2026, 6, 1), new DateTime(2026, 8, 31),
            "Verano 2026", "Subtítulo", null, null,
            new SeasonColorsDto("#FFFFFF", "#000000", "#CCCCCC"),
            "CTA", "/link", new List<Guid>(), null, new List<SeasonImageInputDto>()
        );

        return (request, collection, season);
    }

    [Fact]
    public async Task UpdateAsync_ImagesDesconocidas_CreaEntidadesNuevasSinTocarFilasInexistentes()
    {
        var (request, _, season) = SetUpUpdate();
        var idFantasma = Guid.NewGuid();

        // El cliente manda un Id que no existe en la base de datos. Antes esto
        // terminaba en UPDATE sobre una fila inexistente -> DbUpdateConcurrencyException.
        request = request with
        {
            Images = new List<SeasonImageInputDto>
            {
                new(idFantasma, "https://cdn/a.jpg", 0, true),
                new(null, "https://cdn/b.jpg", 1, false),
            }
        };

        await _sut.UpdateAsync(season.Id, request);

        Assert.Equal(2, season.Images.Count);
        // Ninguna entidad debe adoptar el Id fantasma.
        Assert.DoesNotContain(season.Images, i => i.Id == idFantasma);
        Assert.All(season.Images, i => Assert.NotEqual(Guid.Empty, i.Id));
    }

    [Fact]
    public async Task UpdateAsync_ReutilizaIdsExistentesYEliminaLasAusentes()
    {
        var (request, _, season) = SetUpUpdate();

        var conservada = new SeasonImage { Id = Guid.NewGuid(), SeasonId = season.Id, ImageUrl = "https://cdn/vieja.jpg", Order = 0, IsPrimary = true };
        var descartada = new SeasonImage { Id = Guid.NewGuid(), SeasonId = season.Id, ImageUrl = "https://cdn/borrar.jpg", Order = 1 };
        season.Images.Add(conservada);
        season.Images.Add(descartada);

        request = request with
        {
            Images = new List<SeasonImageInputDto>
            {
                new(conservada.Id, "https://cdn/nueva.jpg", 0, true),
                new(null, "https://cdn/agregada.jpg", 1, false),
            }
        };

        await _sut.UpdateAsync(season.Id, request);

        // La existente se actualizó en el sitio (misma identidad de EF).
        Assert.Equal("https://cdn/nueva.jpg", conservada.ImageUrl);
        Assert.Contains(season.Images, i => i.ImageUrl == "https://cdn/agregada.jpg");
        Assert.DoesNotContain(season.Images, i => i.Id == descartada.Id);
        Assert.DoesNotContain(season.Images, i => i.ImageUrl == "https://cdn/borrar.jpg");

        // El borrado pasa explícitamente por el DbSet, no solo por la colección.
        _seasonRepository.Verify(r => r.RemoveImage(descartada), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_GarantizaUnaSolaPortadaYOrdenContiguo()
    {
        var (request, _, season) = SetUpUpdate();

        request = request with
        {
            Images = new List<SeasonImageInputDto>
            {
                new(null, "https://cdn/1.jpg", 5, false),
                new(null, "https://cdn/2.jpg", 9, false),
                new(null, "https://cdn/3.jpg", 1, true),
            }
        };

        await _sut.UpdateAsync(season.Id, request);

        // Orden 0..n-1 tras renumerar, y una sola portada.
        Assert.Equal(new[] { 0, 1, 2 }, season.Images.Select(i => i.Order).OrderBy(o => o).ToArray());
        Assert.Single(season.Images, i => i.IsPrimary);
    }

    [Fact]
    public async Task UpdateAsync_ImagesNull_NoTocaLaGaleriaExistente()
    {
        var (request, _, season) = SetUpUpdate();
        var existente = new SeasonImage { Id = Guid.NewGuid(), SeasonId = season.Id, ImageUrl = "https://cdn/keep.jpg", Order = 0, IsPrimary = true };
        season.Images.Add(existente);

        await _sut.UpdateAsync(season.Id, request with { Images = null });

        Assert.Single(season.Images);
        Assert.Equal("https://cdn/keep.jpg", existente.ImageUrl);
        _seasonRepository.Verify(r => r.RemoveImage(It.IsAny<SeasonImage>()), Times.Never);
    }
}