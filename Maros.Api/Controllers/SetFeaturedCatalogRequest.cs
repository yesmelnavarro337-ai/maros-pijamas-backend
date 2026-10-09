namespace Maros.Api.Controllers;

public class SetFeaturedCatalogRequest
{
    public List<Guid> ProductIds { get; set; } = new();
}