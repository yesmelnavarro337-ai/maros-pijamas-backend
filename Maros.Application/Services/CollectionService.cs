using Maros.Application.Common;
using Maros.Application.DTOs.Collections;
using Maros.Application.DTOs.Common;
using Maros.Domain.Entities;
using Maros.Domain.Interfaces;
using Maros.Application.Interfaces;

namespace Maros.Application.Services;

public class CollectionService : ICollectionService
{
    private readonly ICollectionRepository _collectionRepository;

    public CollectionService(ICollectionRepository collectionRepository)
    {
        _collectionRepository = collectionRepository;
    }

    public async Task<List<CollectionResponseDto>> GetAllAsync()
    {
        var collections = await _collectionRepository.GetAllAsync();
        return collections.Select(ToDto).ToList();
    }

    public async Task<CollectionResponseDto> GetByIdAsync(Guid id)
    {
        var collection = await _collectionRepository.GetByIdAsync(id)
            ?? throw new AppException("Colección no encontrada.", 404);
        return ToDto(collection);
    }

    public async Task<CollectionResponseDto> CreateAsync(CollectionCreateDto request)
    {
        var collection = new Collection
        {
            Name = request.Name,
            Description = request.Description,
            CoverImageUrl = request.CoverImageUrl,
            AccentHex = request.AccentHex,
            IsDefault = false,
        };

        await _collectionRepository.AddAsync(collection);
        await _collectionRepository.SaveChangesAsync();

        return ToDto(collection);
    }

    public async Task<CollectionResponseDto> UpdateAsync(Guid id, CollectionUpdateDto request)
    {
        var collection = await _collectionRepository.GetByIdAsync(id)
            ?? throw new AppException("Colección no encontrada.", 404);

        collection.Name = request.Name;
        collection.Description = request.Description;
        collection.CoverImageUrl = request.CoverImageUrl;
        collection.AccentHex = request.AccentHex;
        collection.UpdatedAt = DateTime.UtcNow;

        // Reemplazo completo de la relación de productos de ESTA colección,
        // sin tocar las asociaciones de otras colecciones.
        collection.ProductCollections.Clear();
        foreach (var productId in request.ProductIds.Distinct())
        {
            collection.ProductCollections.Add(new ProductCollection
            {
                CollectionId = collection.Id,
                ProductId = productId,
            });
        }

        await _collectionRepository.SaveChangesAsync();
        return ToDto(collection);
    }

    public async Task<CollectionResponseDto> SetDefaultAsync(Guid id)
    {
        var collection = await _collectionRepository.GetByIdAsync(id)
            ?? throw new AppException("Colección no encontrada.", 404);

        // Regla exclusiva: solo una colección puede ser la predeterminada.
        await _collectionRepository.ClearDefaultFlagsAsync();
        collection.IsDefault = true;
        collection.UpdatedAt = DateTime.UtcNow;

        await _collectionRepository.SaveChangesAsync();
        return ToDto(collection);
    }

    public async Task RemoveAsync(Guid id)
    {
        var collection = await _collectionRepository.GetByIdAsync(id)
            ?? throw new AppException("Colección no encontrada.", 404);

        if (await _collectionRepository.HasSeasonsAsync(id))
            throw new AppException("No se puede eliminar: hay temporadas que dependen de esta colección.", 409);

        _collectionRepository.Remove(collection);
        await _collectionRepository.SaveChangesAsync();
    }

    public async Task<CollectionPublicResponseDto> GetActivePublicAsync()
    {
        var collection = await _collectionRepository.GetActiveViaSeasonAsync()
            ?? await _collectionRepository.GetDefaultAsync()
            ?? throw new AppException("No hay ninguna colección activa configurada.", 404);

        var products = collection.ProductCollections
            .Select(pc => pc.Product)
            .Where(p => p is not null)
            .Select(p => new ProductSummaryDto(
                p!.Id,
                p.Name,
                p.Slug,
                p.BasePrice,
                p.Images.OrderBy(i => i.Order).Select(i => i.Url).FirstOrDefault(),
                p.Images.OrderBy(i => i.Order).Select(i => i.Url).ToList()
            ))
            .ToList();

        return new CollectionPublicResponseDto(
            collection.Name,
            collection.Description,
            collection.CoverImageUrl,
            collection.AccentHex,
            products
        );
    }

    public async Task<List<CollectionListPublicDto>> GetAllPublicAsync()
    {
        var collections = await _collectionRepository.GetAllAsync();

        return collections.Select(c => new CollectionListPublicDto(
            c.Id,
            c.Name,
            c.Description,
            c.CoverImageUrl,
            c.AccentHex,
            c.ProductCollections.Count
        )).ToList();
    }

    private static CollectionResponseDto ToDto(Collection c) => new(
        c.Id,
        c.Name,
        c.Description,
        c.CoverImageUrl,
        c.AccentHex,
        c.IsDefault,
        c.ProductCollections.Select(pc => pc.ProductId).ToList()
    );
}