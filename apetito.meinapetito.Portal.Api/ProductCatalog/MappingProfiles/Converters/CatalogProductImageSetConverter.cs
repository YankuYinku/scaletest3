using apetito.meinapetito.Cache.Articles.Contracts.ProductCatalog.Models;
using apetito.meinapetito.Portal.Application.ProductCatalog.Consts;
using apetito.meinapetito.Portal.Application.ProductCatalog.Enums;
using apetito.meinapetito.Portal.Contracts.ProductCatalog.Models;
using AutoMapper;

namespace apetito.meinapetito.Portal.Api.ProductCatalog.MappingProfiles.Converters;

public class CatalogProductImageSetConverter : ITypeConverter<ProductCatalogImageSetCacheDto, ProductCatalogImagePathsDto>
{
    public ProductCatalogImagePathsDto Convert(ProductCatalogImageSetCacheDto source, ProductCatalogImagePathsDto destination,
        ResolutionContext context)
    {
        if (source is null)
        {
            return new ProductCatalogImagePathsDto();
        }
        
        var articleType = ArticleType.Product;

        if (context.Items.TryGetValue(MapperConstKeys.MapperQueryContext, out var contextItem))
        {
            articleType = (ArticleType) contextItem;
        }

        return articleType switch
        {
            ArticleType.Product => Convert(source.Product),
            ArticleType.Material => Convert(source.Material),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static ProductCatalogImagePathsDto Convert(ProductCatalogImageSetItemCacheDto item)
        =>
            item is null ? null :
            
            new ProductCatalogImagePathsDto
        {
            Square = item.SquareUrl,
            Rectangular = item.RectanguarUrl
        };
}