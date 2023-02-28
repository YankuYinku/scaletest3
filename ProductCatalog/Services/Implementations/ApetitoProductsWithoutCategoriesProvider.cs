using apetito.ArticleGateway.Contracts.v1.Querying;
using apetito.iProDa3.Contracts.ApiClient.RequestModels;
using apetito.iProDa3.Contracts.ApiClient.V1;
using apetito.meinapetito.Portal.Application.ProductCatalog.Services.Interfaces;
using apetito.meinapetito.Portal.Contracts.ProductCatalog.Models;
using AutoMapper;

namespace apetito.meinapetito.Portal.Application.ProductCatalog.Services.Implementations
{
    public class ApetitoProductsWithoutCategoriesProvider : IApetitoProductsWithoutCategoriesProvider
    {
        private readonly IiProDa3ArticlesV1RestClient _apetitoProductCatalogIProdaApi;
        private readonly IMapper _mapper;

        public ApetitoProductsWithoutCategoriesProvider(IiProDa3ArticlesV1RestClient apetitoProductCatalogIProdaApi,
            IMapper mapper)
        {
            _apetitoProductCatalogIProdaApi = apetitoProductCatalogIProdaApi;
            _mapper = mapper;
        }

        public async Task<GetProductListResultDto> GetProductsWithoutMergedCategoriesAsync(GetCatalogProductsRequest request)
        {
            var articleQueryResult = await CallApiAsync(request);
            
            return new GetProductListResultDto
            {
                Articles =articleQueryResult.Items?.Select(z
                    => _mapper.Map<ProductCatalogArticleDto>(z)).ToList(),
                OverallPages = articleQueryResult.OverallPages,
                OverallResults = articleQueryResult.OverallResults,
                CategoriesSummarization = new List<CategoriesWithCountDto>()
            };
        }
        private async Task<ArticleQueryResult> CallApiAsync(GetCatalogProductsRequest request)
        {
            var api = _apetitoProductCatalogIProdaApi;

            var query = new RetrieveArticlesQuery
            {
                Acs = request.Acs,
                Additives = request.Additives,
                Allergens = request.Allergens,
                Categories = request.Categories,
                Diets = request.Diets,
                Distinct = request.Distinct,
                Expand = request.Expand,
                Filter = request.Filter,
                Ids = request.Ids,
                Page = request.Page,
                PageSize = request.PageSize,
                Seals = request.Seals,
                Search = request.Search,
                Sortiments = request.Sortiments,
                Summarization = 1,
                ArticleIds = request.ArticleIds,
                CustomerNumber = request.CustomerNumber,
                FoodForms = request.FoodForms,
                PriceGroups = request.PriceGroups,
                OutletArticleWithoutStock = request.OutletArticleWithoutStock,
                GetArticleInLastValidSortiment = request.GetArticleInLastValidSortiment
            };


            return await api.RetrieveArticlesAsync<ArticleQueryResult>(query);
        }
    }
}