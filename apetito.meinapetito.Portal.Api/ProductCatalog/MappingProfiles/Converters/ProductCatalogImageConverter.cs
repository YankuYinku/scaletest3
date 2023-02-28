using apetito.meinapetito.Portal.Application.ProductCatalog.Consts;
using apetito.meinapetito.Portal.Application.ProductCatalog.Enums;
using apetito.meinapetito.Portal.Application.ProductCatalog.Services.Interfaces;
using apetito.meinapetito.Portal.Contracts.ProductCatalog.Models;
using AutoMapper;

namespace apetito.meinapetito.Portal.Api.ProductCatalog.MappingProfiles.Converters;

public class ProductCatalogImageConverter : ITypeConverter<string, ProductCatalogImageDto>
{
    private readonly IProductCatalogPhotoPathBuilder _productCatalogPhotoPathBuilder;

    public ProductCatalogImageConverter(IProductCatalogPhotoPathBuilder productCatalogPhotoPathBuilder)
    {
        _productCatalogPhotoPathBuilder = productCatalogPhotoPathBuilder;
    }

    public ProductCatalogImageDto Convert(string source, ProductCatalogImageDto destination,
        ResolutionContext context)
    {
        ArticleType articleType = ArticleType.Product;

        if (context.Items.TryGetValue(MapperConstKeys.MapperQueryContext, out var contextItem))
        {
            articleType = (ArticleType) contextItem;
        }
        
        return new ProductCatalogImageDto
        {
            Small = BuildPhotoPath(source, ImageSize.Small,articleType),
            Middle = BuildPhotoPath(source, ImageSize.Middle,articleType),
            Big = BuildPhotoPath(source, ImageSize.Big,articleType),
        };
    }

    private string BuildPhotoPath(string? articleId, ImageSize imageSize, ArticleType articleType) =>
        _productCatalogPhotoPathBuilder.BuildPhotoPath(articleId, imageSize, articleType);
}