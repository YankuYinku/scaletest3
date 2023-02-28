using apetito.meinapetito.Portal.Application.ProductCatalog.Services.Interfaces;
using apetito.meinapetito.Portal.Contracts.ProductCatalog;
using apetito.meinapetito.Portal.Contracts.ProductCatalog.Consts;
using apetito.meinapetito.Portal.Contracts.ProductCatalog.Models;
using HotChocolate;
using HotChocolate.Types;

namespace apetito.meinapetito.Portal.Api.ProductCatalog.Queries;

[ExtendObjectType("Query")]
public class ProductCatalogArticleQuery : ExpectedItemsBasedQuery
{
    public async Task<GetProductListResultDto> GetProductsAsync(
        [Service] IApetitoProductsFromCacheProvider apetitoProductsProvider,
        [Service] ILogger<ProductCatalogArticleQuery> logger,
        GetProductsGraphQlQueryModel graphQlQueryModel)
    {
            logger.LogInformation("GetProductsAsync");

            var request = new GetCatalogProductsRequest
            {
                ExpectedArticleType = GetTypeByParameter(graphQlQueryModel.SortimentType),
                PageSize = graphQlQueryModel.PageSize ?? DefaultParameterValues.DefaultPageSize,
                Page = graphQlQueryModel.Page ?? DefaultParameterValues.DefaultPage,
                Search = graphQlQueryModel.Search,
                Categories = ListOrEmpty(graphQlQueryModel.Categories),
                Distinct = graphQlQueryModel.Distinct,
                Sortiments = ListOrEmpty(graphQlQueryModel.Sortiments),
                OutletArticleWithoutStock = graphQlQueryModel.OutletArticleWithoutStock,
                GetArticleInLastValidSortiment = graphQlQueryModel.GetArticleInLastValidSortiment,
                Filter = graphQlQueryModel.Filter,
                CustomerNumber = graphQlQueryModel.CustomerNumber,
                Acs = ListOrEmpty(graphQlQueryModel.Acs),
                Additives = ListOrEmpty(graphQlQueryModel.Additives),
                Allergens = ListOrEmpty(graphQlQueryModel.Allergens),
                Diets = ListOrEmpty(graphQlQueryModel.Diets),
                Expand = ListOrEmpty(graphQlQueryModel.Expand),
                Ids = ListOrEmpty(graphQlQueryModel.Ids),
                Seals = ListOrEmpty(graphQlQueryModel.Seals),
                ArticleIds = ListOrEmpty(graphQlQueryModel.ArticleIds),
                FoodForms = ListOrEmpty(graphQlQueryModel.FoodForms),
                PriceGroups = ListOrEmpty(graphQlQueryModel.PriceGroups),
                SourceApis = ListOrEmpty(graphQlQueryModel.SourceApis),
                LanguageCode = graphQlQueryModel.LanguageCode,
            };

            return await apetitoProductsProvider.GetProductsAsync(request);
    }
    
    private static IList<T> ListOrEmpty<T>(IList<T>? objects)
        => objects ?? new List<T>();
}