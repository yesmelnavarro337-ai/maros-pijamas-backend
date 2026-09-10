using Maros.Application.Common;
using Maros.Application.Services;
using Maros.Domain.Entities;
using Maros.Domain.Interfaces;
using Moq;
using Xunit;

namespace Maros.Application.Tests.Services;

public class CollectionServiceTests
{
    private readonly Mock<ICollectionRepository> _collectionRepository = new();
    private readonly CollectionService _sut;

    public CollectionServiceTests()
    {
        _sut = new CollectionService(_collectionRepository.Object);
    }

    [Fact]
    public async Task GetActivePublicAsync_HayTemporadaActiva_UsaSuColeccion()
    {
        var collectionDeTemporada = new Collection { Id = Guid.NewGuid(), Name = "Navidad" };
        var collectionPredeterminada = new Collection { Id = Guid.NewGuid(), Name = "General", IsDefault = true };

        _collectionRepository.Setup(r => r.GetActiveViaSeasonAsync()).ReturnsAsync(collectionDeTemporada);
        _collectionRepository.Setup(r => r.GetDefaultAsync()).ReturnsAsync(collectionPredeterminada);

        var result = await _sut.GetActivePublicAsync();

        Assert.Equal("Navidad", result.Name);
        // Confirma que NI SIQUIERA se consultó la predeterminada, porque ya había una vía temporada.
        _collectionRepository.Verify(r => r.GetDefaultAsync(), Times.Never);
    }

    [Fact]
    public async Task GetActivePublicAsync_SinTemporadaActiva_UsaLaPredeterminada()
    {
        var collectionPredeterminada = new Collection { Id = Guid.NewGuid(), Name = "General", IsDefault = true };

        _collectionRepository.Setup(r => r.GetActiveViaSeasonAsync()).ReturnsAsync((Collection?)null);
        _collectionRepository.Setup(r => r.GetDefaultAsync()).ReturnsAsync(collectionPredeterminada);

        var result = await _sut.GetActivePublicAsync();

        Assert.Equal("General", result.Name);
    }

    [Fact]
    public async Task GetActivePublicAsync_SinTemporadaNiPredeterminada_LanzaAppException404()
    {
        _collectionRepository.Setup(r => r.GetActiveViaSeasonAsync()).ReturnsAsync((Collection?)null);
        _collectionRepository.Setup(r => r.GetDefaultAsync()).ReturnsAsync((Collection?)null);

        var ex = await Assert.ThrowsAsync<AppException>(() => _sut.GetActivePublicAsync());
        Assert.Equal(404, ex.StatusCode);
    }
}