using apetito.meinapetito.Portal.Application.ProductCatalog.Enums;
using apetito.meinapetito.Portal.Application.ProductCatalog.Options;
using apetito.meinapetito.Portal.Application.ProductCatalog.Services.Interfaces;

namespace apetito.meinapetito.Portal.Application.ProductCatalog.Services.Implementations
{
    public class ProductCatalogPhotoPathBuilder : IProductCatalogPhotoPathBuilder
    {
        private readonly ProductCatalogPhotoBuilderOptions _options;

        public ProductCatalogPhotoPathBuilder(ProductCatalogPhotoBuilderOptions options)
        {
            _options = options;
        }

        public string BuildPhotoPath(string? articleId, ImageSize imageSize, ArticleType articleType)
        {
            return articleType switch
            {
                ArticleType.Product =>
                    BuildPhotoPathForProduct(articleId, imageSize),
                ArticleType.Material =>
                    BuildPhotoPathForMaterial(articleId, imageSize),
                _ => throw new ArgumentOutOfRangeException(nameof(articleType), articleType, null)
            };
        }

        private string BuildPhotoPathForProduct(string? articleId, ImageSize imageSize)
            => 
                !string.IsNullOrWhiteSpace(PathByImageSize(imageSize))?
                $"{PathByImageSize(imageSize) ?? string.Empty}{articleId}.jpg"
                : string.Empty;
            
        private string? PathByImageSize(ImageSize size)
            => size switch {
                ImageSize.Small => _options?.ProductImagePath?.Small,
                ImageSize.Middle => _options?.ProductImagePath?.Middle,
                ImageSize.Big => _options?.ProductImagePath?.Big,
                _ => throw new ArgumentOutOfRangeException(nameof(size), size, null)
            };
        

        private string BuildPhotoPathForMaterial(string? articleId, ImageSize imageSize)
        {
            return $"{string.Format(_options?.MaterialImagePath ?? string.Empty, MaterialSizeConverting[imageSize], articleId)}";
        }

        private static readonly Dictionary<ImageSize, string> MaterialSizeConverting = new()
        {
            {ImageSize.Big, "176x120"},
            {ImageSize.Middle, "176x120"},
            {ImageSize.Small, "176x120"}
        };

        public string? BuildNutriScorePhotoPath(string? dietCode)
            => dietCode is null || _options.NutriScoreImagePath is null
                ? null
                : string.Format(_options.NutriScoreImagePath, Mapping.TryGetValue(dietCode,out var value) ? value : "");

        private static readonly Dictionary<string, string> Mapping = new()
        {
            {"SCOREA", "A"},
            {"SCOREB", "B"},
            {"SCOREC", "C"},
            {"SCORED", "D"},
            {"SCOREE", "E"}
        };
    }
}