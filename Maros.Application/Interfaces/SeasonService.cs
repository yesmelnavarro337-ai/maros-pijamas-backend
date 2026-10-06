using Maros.Application.Common;
using Maros.Application.DTOs.Common;
using Maros.Application.DTOs.Seasons;
using Maros.Application.Interfaces;
using Maros.Domain.Entities;
using Maros.Domain.Enums;
using Maros.Domain.Interfaces;

namespace Maros.Application.Services;

public class SeasonService : ISeasonService
{
    private readonly ISeasonRepository _seasonRepository;
    private readonly ICollectionRepository _collectionRepository;

    public SeasonService(ISeasonRepository seasonRepository, ICollectionRepository collectionRepository)
    {
        _seasonRepository = seasonRepository;
        _collectionRepository = collectionRepository;
    }

    public async Task<List<SeasonResponseDto>> GetAllAsync()
    {
        var seasons = await _seasonRepository.GetAllAsync();
        return seasons.Select(ToDto).ToList();
    }

    public async Task<SeasonResponseDto> GetByIdAsync(Guid id)
    {
        var season = await _seasonRepository.GetByIdReadOnlyAsync(id)
            ?? throw new AppException("Temporada no encontrada.", 404);
        return ToDto(season);
    }

    public async Task<SeasonResponseDto> CreateAsync(SeasonCreateDto request)
    {
        var startDateUtc = DateTime.SpecifyKind(request.StartDate, DateTimeKind.Utc);
        var endDateUtc = DateTime.SpecifyKind(request.EndDate, DateTimeKind.Utc);

        ValidateDateRange(startDateUtc, endDateUtc);
        var collection = await GetCollectionOrThrowAsync(request.CollectionId);

        var slug = SlugGenerator.Generate(request.Name);
        if (await _seasonRepository.SlugExistsAsync(slug))
            throw new AppException("Ya existe una temporada con un nombre equivalente.", 409);

        await ValidateFeaturedProductsBelongToCollectionAsync(collection, request.FeaturedProductIds);

        var status = ParseStatusOrDefault(request.Status, SeasonStatus.Borrador);
        if (status == SeasonStatus.Activa)
        {
            await _seasonRepository.DeactivateAllExceptAsync(Guid.Empty);
        }

        var season = new Season
        {
            Name = request.Name,
            Slug = slug,
            StartDate = startDateUtc,
            EndDate = endDateUtc,
            Status = status,
            CollectionId = request.CollectionId,
            HeroTitle = request.HeroTitle,
            HeroSubtitle = request.HeroSubtitle,
            HeroImageUrl = request.HeroImageUrl,
            BannerImageUrl = request.BannerImageUrl,
            ColorPrimary = request.Colors.Primary,
            ColorAccent = request.Colors.Accent,
            ColorBackground = request.Colors.Background,
            CtaText = request.CtaText,
            CtaLink = request.CtaLink,
            CreatedAt = DateTime.UtcNow,
        };

        ApplyFeaturedProducts(season, request.FeaturedProductIds);
        SyncSeasonImages(season, request.Images);

        await _seasonRepository.AddAsync(season);
        await _seasonRepository.SaveChangesAsync();

        var created = await _seasonRepository.GetByIdAsync(season.Id);
        return ToDto(created!);
    }

    public async Task<SeasonResponseDto> UpdateAsync(Guid id, SeasonUpdateDto request)
    {
        var season = await _seasonRepository.GetByIdAsync(id)
            ?? throw new AppException("Temporada no encontrada.", 404);

        var startDateUtc = DateTime.SpecifyKind(request.StartDate, DateTimeKind.Utc);
        var endDateUtc = DateTime.SpecifyKind(request.EndDate, DateTimeKind.Utc);

        ValidateDateRange(startDateUtc, endDateUtc);
        var collection = await GetCollectionOrThrowAsync(request.CollectionId);

        var slug = SlugGenerator.Generate(request.Name);
        if (await _seasonRepository.SlugExistsAsync(slug, excludeId: id))
            throw new AppException("Ya existe otra temporada con un nombre equivalente.", 409);

        await ValidateFeaturedProductsBelongToCollectionAsync(collection, request.FeaturedProductIds);

        var status = ParseStatusOrDefault(request.Status, season.Status);
        if (status == SeasonStatus.Activa)
        {
            await _seasonRepository.DeactivateAllExceptAsync(id);
        }

        season.Name = request.Name;
        season.Slug = slug;
        season.StartDate = startDateUtc;
        season.EndDate = endDateUtc;
        season.Status = status;
        season.CollectionId = request.CollectionId;
        season.HeroTitle = request.HeroTitle;
        season.HeroSubtitle = request.HeroSubtitle;
        season.HeroImageUrl = request.HeroImageUrl;
        season.BannerImageUrl = request.BannerImageUrl;
        season.ColorPrimary = request.Colors.Primary;
        season.ColorAccent = request.Colors.Accent;
        season.ColorBackground = request.Colors.Background;
        season.CtaText = request.CtaText;
        season.CtaLink = request.CtaLink;
        season.UpdatedAt = DateTime.UtcNow;

        SyncFeaturedProducts(season, request.FeaturedProductIds);
        SyncSeasonImages(season, request.Images);

        // El SaveChanges se delega al repositorio, que traduce cualquier
        // DbUpdateConcurrencyException a AppException(409) en la frontera EF.
        await _seasonRepository.SaveChangesAsync();

        // Relectura con AsNoTracking: el contexto ya guardó, solo queremos la
        // proyección final sin coste de seguimiento.
        var updated = await _seasonRepository.GetByIdReadOnlyAsync(id);
        return ToDto(updated!);
    }

    public async Task<SeasonResponseDto> ActivateAsync(Guid id)
    {
        var season = await _seasonRepository.GetByIdAsync(id)
            ?? throw new AppException("Temporada no encontrada.", 404);

        // Regla exclusiva: al activar una, cualquier otra "Activa" pasa a "Finalizada".
        // Misma regla que ya validamos en el frontend con activateSeason().
        await _seasonRepository.DeactivateAllExceptAsync(id);

        season.Status = SeasonStatus.Activa;
        season.UpdatedAt = DateTime.UtcNow;

        await _seasonRepository.SaveChangesAsync();

        var activated = await _seasonRepository.GetByIdAsync(id);
        return ToDto(activated!);
    }

    public async Task RemoveAsync(Guid id)
    {
        var season = await _seasonRepository.GetByIdAsync(id)
            ?? throw new AppException("Temporada no encontrada.", 404);

        _seasonRepository.Remove(season);
        await _seasonRepository.SaveChangesAsync();
    }

    public async Task<SeasonPublicResponseDto?> GetActivePublicAsync()
    {
        var season = await _seasonRepository.GetActiveAsync();
        if (season is null) return null;

        var featuredProducts = season.FeaturedProducts
            .Select(fp => fp.Product)
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

        var images = season.Images
            .OrderBy(i => i.Order)
            .Select(ToImageDto)
            .ToList();

        return new SeasonPublicResponseDto(
            season.Name,
            season.HeroTitle,
            season.HeroSubtitle,
            season.HeroImageUrl,
            season.BannerImageUrl,
            new SeasonColorsDto(season.ColorPrimary, season.ColorAccent, season.ColorBackground),
            season.CtaText,
            season.CtaLink,
            featuredProducts,
            images
        );
    }

    // ─── Helpers privados ──────────────────────────────────────────

    private static void ValidateDateRange(DateTime start, DateTime end)
    {
        if (end < start)
            throw new AppException("La fecha de fin debe ser posterior a la fecha de inicio.", 400);
    }

    private async Task<Collection> GetCollectionOrThrowAsync(Guid collectionId)
    {
        var collection = await _collectionRepository.GetByIdAsync(collectionId);
        if (collection is null)
            throw new AppException("La colección seleccionada no existe.", 400);
        return collection;
    }

    private static Task ValidateFeaturedProductsBelongToCollectionAsync(Collection collection, List<Guid> featuredProductIds)
    {
        if (featuredProductIds.Count == 0) return Task.CompletedTask;

        var validIds = collection.ProductCollections.Select(pc => pc.ProductId).ToHashSet();
        var invalid = featuredProductIds.Where(id => !validIds.Contains(id)).ToList();

        if (invalid.Count > 0)
            throw new AppException("Solo puedes destacar productos que pertenecen a la colección de la temporada.", 400);

        return Task.CompletedTask;
    }

    private static SeasonStatus ParseStatusOrDefault(string? input, SeasonStatus fallback)
    {
        if (string.IsNullOrWhiteSpace(input))
            return fallback;

        if (!Enum.TryParse<SeasonStatus>(input, ignoreCase: true, out var status))
            throw new AppException("Estado de temporada inválido.", 400);

        return status;
    }

    private static void ApplyFeaturedProducts(Season season, List<Guid> productIds)
    {
        foreach (var productId in productIds.Distinct())
        {
            season.FeaturedProducts.Add(new SeasonFeaturedProduct
            {
                SeasonId = season.Id,
                ProductId = productId,
            });
        }
    }

    private static void SyncFeaturedProducts(Season season, List<Guid> productIds)
    {
        var desiredIds = productIds.Distinct().ToHashSet();
        var existing = season.FeaturedProducts.ToList();

        foreach (var featured in existing)
        {
            if (!desiredIds.Contains(featured.ProductId))
            {
                season.FeaturedProducts.Remove(featured);
            }
        }

        var currentIds = season.FeaturedProducts.Select(fp => fp.ProductId).ToHashSet();
        foreach (var productId in desiredIds)
        {
            if (!currentIds.Contains(productId))
            {
                season.FeaturedProducts.Add(new SeasonFeaturedProduct
                {
                    SeasonId = season.Id,
                    ProductId = productId,
                });
            }
        }
    }

    /// <summary>
    /// Reconcilia la colección de imágenes con lo que envía el admin.
    ///
    /// Reglas:
    /// - <c>null</c> significa "no tocar": una actualización parcial no borra imágenes.
    /// - Se reutiliza la fila existente cuando viene su Id, y se elimina la que sobra.
    /// - Order se renumera denso (0..n-1) para que el carrusel nunca tenga huecos.
    /// - Solo una imagen puede ser portada: si el cliente marca varias gana la primera,
    ///   y si no marca ninguna la primera de la lista asume el papel.
    /// </summary>
    private void SyncSeasonImages(Season season, List<SeasonImageInputDto>? inputs)
    {
        if (inputs is null) return;

        // 1) Normaliza la entrada: descarta URLs vacías y ordena por Order (o por
        //    la posición en el DTO cuando Order no viene).
        var desired = inputs
            .Where(i => !string.IsNullOrWhiteSpace(i.ImageUrl))
            .Select((input, index) => new { input, index })
            .OrderBy(x => x.input.Order ?? x.index)
            .ThenBy(x => x.index)
            .Select(x => x.input)
            .ToList();

        // 2) Invariante de portada: exactamente una IsPrimary. Si el DTO no marca
        //    ninguna, la primera de la lista la asume.
        if (desired.Count > 0 && !desired.Any(i => i.IsPrimary))
        {
            desired[0] = desired[0] with { IsPrimary = true };
        }

        // 3) Solo se reutilizan imágenes que EF ya tiene rastreadas y que existen en
        //    la base de datos. Un Id desconocido genera una entidad nueva con Id
        //    propio. Se fuerza EntityState.Added a continuación para evitar que EF
        //    marque la imagen como Modified y emita un UPDATE que afecta 0 filas
        //    (lo que provocaría 409).
        var existingById = season.Images.ToDictionary(i => i.Id);
        var keptIds = new HashSet<Guid>();
        var primaryAssigned = false;

        for (var position = 0; position < desired.Count; position++)
        {
            var input = desired[position];

            var isPrimary = input.IsPrimary && !primaryAssigned;
            if (isPrimary) primaryAssigned = true;

            SeasonImage entity;
            if (input.Id.HasValue
                && input.Id.Value != Guid.Empty
                && existingById.TryGetValue(input.Id.Value, out var found))
            {
                // Imagen existente: se actualizan sus propiedades en el sitio.
                entity = found;
            }
            else
            {
                // Imagen nueva: se registra explícitamente en el DbSet como Added
                // para que EF emita un INSERT en lugar de un UPDATE que afectaría
                // 0 filas y provocaría DbUpdateConcurrencyException (409).
                entity = new SeasonImage
                {
                    Id = Guid.NewGuid(),
                    SeasonId = season.Id,
                    CreatedAt = DateTime.UtcNow,
                };
                _seasonRepository.AddImage(entity);
                if (!season.Images.Contains(entity))
                {
                    season.Images.Add(entity);
                }
            }

            entity.ImageUrl = input.ImageUrl.Trim();
            entity.Order = position;
            entity.IsPrimary = isPrimary;
            entity.UpdatedAt = DateTime.UtcNow;

            keptIds.Add(entity.Id);
        }

        // 4) Las imágenes que el DTO ya no incluye se eliminan explícitamente del
        //    DbSet para que EF las borre y no las deje huérfanas.
        foreach (var orphan in season.Images.Where(i => !keptIds.Contains(i.Id)).ToList())
        {
            _seasonRepository.RemoveImage(orphan);
            season.Images.Remove(orphan);
        }
    }

    private static SeasonImageDto ToImageDto(SeasonImage i) => new(i.Id, i.ImageUrl, i.Order, i.IsPrimary);

    private static SeasonResponseDto ToDto(Season s) => new(
        s.Id,
        s.Name,
        s.Slug,
        s.StartDate,
        s.EndDate,
        s.Status.ToString(),
        s.CollectionId,
        s.Collection?.Name ?? string.Empty,
        s.HeroTitle,
        s.HeroSubtitle,
        s.HeroImageUrl,
        s.BannerImageUrl,
        new SeasonColorsDto(s.ColorPrimary, s.ColorAccent, s.ColorBackground),
        s.CtaText,
        s.CtaLink,
        s.FeaturedProducts != null ? s.FeaturedProducts.Select(fp => fp.ProductId).ToList() : new List<Guid>(),
        s.Status == Domain.Enums.SeasonStatus.Activa,
        // La portada multi-imagen manda; los campos legacy siguen siendo el fallback.
        s.Images?.FirstOrDefault(i => i.IsPrimary)?.ImageUrl ?? s.BannerImageUrl ?? s.HeroImageUrl,
        s.FeaturedProducts != null ? s.FeaturedProducts.Count : 0,
        s.Images != null
            ? s.Images.OrderBy(i => i.Order).Select(ToImageDto).ToList()
            : new List<SeasonImageDto>()
    );
}
