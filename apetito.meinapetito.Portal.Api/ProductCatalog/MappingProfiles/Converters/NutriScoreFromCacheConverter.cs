using apetito.meinapetito.Cache.Articles.Contracts.ProductCatalog.Models;
using apetito.meinapetito.Portal.Application.ProductCatalog.Services.Interfaces;
using apetito.meinapetito.Portal.Contracts.ProductCatalog.Models;
using AutoMapper;

namespace apetito.meinapetito.Portal.Api.ProductCatalog.MappingProfiles.Converters;

public class NutriScoreFromCacheConverter : ITypeConverter<IList<ProductCatalogDietCacheDto>, NutriScorePhotoDto>
{
    private readonly IProductCatalogPhotoPathBuilder _productCatalogPhotoPathBuilder;
    private const string NutriScoreDietKey = "Nutri-Score";

    public NutriScoreFromCacheConverter(IProductCatalogPhotoPathBuilder productCatalogPhotoPathBuilder)
    {
        _productCatalogPhotoPathBuilder = productCatalogPhotoPathBuilder;
    }

    public NutriScorePhotoDto Convert(IList<ProductCatalogDietCacheDto> source, NutriScorePhotoDto destination, ResolutionContext context)
    {
        var nutriCode = source?.FirstOrDefault(d => d.Description == NutriScoreDietKey)?.Code;

        return new NutriScorePhotoDto
        {
            Path = _productCatalogPhotoPathBuilder.BuildNutriScorePhotoPath(nutriCode) ?? string.Empty
        };
    }
}