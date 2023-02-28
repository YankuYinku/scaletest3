using apetito.meinapetito.Portal.Application.ProductCatalog.Services.Interfaces;
using apetito.meinapetito.Portal.Contracts.ProductCatalog.Models;
using apetito.meinapetito.Portal.Contracts.ProductCatalog.Models.Components;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;

namespace apetito.meinapetito.Portal.Api.ProductCatalog.Queries;

[ExtendObjectType("Query")]
public class ComponentQuery : ExpectedItemsBasedQuery
{
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<ProductCatalogCategoryDto>> GetCategoriesAsync([Service] IApetitoCategoriesProvider
        apetitoCategoriesProvider,string sortimentType)
        => await apetitoCategoriesProvider.GetCategoriesAsync(GetTypeByParameter(sortimentType));

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IEnumerable<DietComponentDto>> GetDietsAsync([Service] IApetitoDietsProvider apetitoDietsProvider)
        => await apetitoDietsProvider.GetDietsAsync();

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IEnumerable<AllergenComponentDto>> GetAllergensAsync([Service] IApetitoAllergensProvider
        apetitoAllergensProvider)
        => await apetitoAllergensProvider.GetAllergensAsync();

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IEnumerable<AdditiveComponentDto>> GetAdditivesAsync(
        [Service] IApetitoAdditivesProvider apetitoAdditivesProvider)
        => await apetitoAdditivesProvider.GetAdditivesAsync();

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IEnumerable<FoodFormComponentDto>> GetFoodFormsAsync([Service] IApetitoFoodFormsProvider 
        apetitoFoodFormsProvider)
        => await apetitoFoodFormsProvider.GetFoodFormsAsync();

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IEnumerable<SealComponentDto>> GetSealsAsync([Service] IApetitoSealsProvider apetitoSealsProvider)
        => await apetitoSealsProvider.GetSealsAsync();

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IEnumerable<NutrientComponentDto>> GetNutrientsAsync([Service] IApetitoNutrientsProvider 
        apetitoNutrientsProvider)
        => await apetitoNutrientsProvider.GetNutrientsAsync();
    
}