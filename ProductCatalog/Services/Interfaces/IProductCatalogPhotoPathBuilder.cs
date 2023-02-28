using apetito.meinapetito.Portal.Application.ProductCatalog.Enums;

namespace apetito.meinapetito.Portal.Application.ProductCatalog.Services.Interfaces
{
    public interface IProductCatalogPhotoPathBuilder
    {
        string BuildPhotoPath(string? articleId, ImageSize imageSize, ArticleType articleType);
        string? BuildNutriScorePhotoPath(string? dietCode);
    }
}