namespace Maros.Application.DTOs.Products;

public class SetFeaturedHomeRequest
{
    public List<Guid> ProductIds { get; set; } = new();
}

public class ToggleFeaturedHomeRequest
{
    public bool FeaturedHome { get; set; }
    public int? Order { get; set; }
}

