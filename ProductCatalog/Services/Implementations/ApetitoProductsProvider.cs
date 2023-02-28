using apetito.ArticleGateway.Contracts.Querying;
using apetito.ArticleGateway.Contracts.v1.Querying;
using apetito.iProDa3.Contracts.ApiClient.RequestModels;
using apetito.iProDa3.Contracts.ApiClient.V1;
using apetito.meinapetito.Portal.Application.ProductCatalog.Consts;
using apetito.meinapetito.Portal.Application.ProductCatalog.Options;
using apetito.meinapetito.Portal.Application.ProductCatalog.Services.Interfaces;
using apetito.meinapetito.Portal.Contracts.ProductCatalog.Models;
using AutoMapper;
using Microsoft.Extensions.Options;

namespace apetito.meinapetito.Portal.Application.ProductCatalog.Services.Implementations
{
    public class ApetitoProductsProvider : IApetitoProductsProvider
    {
        private readonly IiProDa3ArticlesV1RestClient _apetitoProductCatalogIProdaApi;
        private readonly IApetitoCategoriesProvider _apetitoCategoriesProvider;
        private readonly IMapper _mapper;
        private readonly ProductCatalogForbiddenArticlePrefixOptions _productCatalogForbiddenArticlePrefixOptions;
        private readonly ILogger<ApetitoProductsProvider> _logger;

        public ApetitoProductsProvider(
            IiProDa3ArticlesV1RestClient apetitoProductCatalogIProdaApi,
            IApetitoCategoriesProvider apetitoCategoriesProvider, IMapper mapper,
            ILogger<ApetitoProductsProvider> logger,
            IOptions<ProductCatalogForbiddenArticlePrefixOptions> productCatalogForbiddenArticlePrefixOptions)
        {
            _apetitoProductCatalogIProdaApi = apetitoProductCatalogIProdaApi;
            _apetitoCategoriesProvider = apetitoCategoriesProvider;
            _mapper = mapper;
            _logger = logger;
            _productCatalogForbiddenArticlePrefixOptions = productCatalogForbiddenArticlePrefixOptions.Value;
        }

        public async Task<GetProductListResultDto> GetProductsAsync(GetCatalogProductsRequest request)
        {
            try
            {
                _logger.LogInformation("Start GetCategoriesAsync");
                var getCategorieTask = _apetitoCategoriesProvider.GetCategoriesAsync(request.ExpectedArticleType);

                _logger.LogInformation("Start CallApiAsync");
                var getArticleResultTasks = CallApiAsync(request);

                _logger.LogInformation("Start SecondCallForGrabCountsFromApiAsync");
                var secondApiCallWithoutCategories = CallApiAsync(PrepareSameRequestWithoutCategories(request));

                Task.WaitAll(getCategorieTask, getArticleResultTasks, secondApiCallWithoutCategories);
                var allCategories = getCategorieTask.Result;
                var articleQueryResult = getArticleResultTasks.Result;

                _logger.LogInformation("SummarizeCategories");
                var summarization = await SummarizeCateogries(allCategories, secondApiCallWithoutCategories.Result);

                _logger.LogInformation("Map articles results");
                var articles = articleQueryResult.Items?.Select(z
                    => _mapper.Map<ProductCatalogArticleDto>(z, opts
                        => opts.Items.Add(MapperConstKeys.MapperQueryContext, request.ExpectedArticleType))).ToList();

                _logger.LogInformation("return GetProductListResultDto");
                return new GetProductListResultDto
                {
                    Articles = articles,
                    OverallPages = articleQueryResult.OverallPages,
                    OverallResults = articleQueryResult.OverallResults,
                    CategoriesSummarization = summarization
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message}", ex.Message);
                throw;
            }
        }


        private Task<List<CategoriesWithCountDto>> SummarizeCateogries(
            IEnumerable<ProductCatalogCategoryDto> allCategories, ArticleQueryResult articleQueryResult)
        {
            _logger.LogInformation("BuildCategoriesDictionary");
            var queryResultCategories = BuildCategoriesDictionary(articleQueryResult.FilterSummary.Categories);

            _logger.LogInformation("JoinResultCategoriesWithEmptyCategories");
            var joinedCategories = JoinResultCategoriesWithEmptyCategories(allCategories, queryResultCategories);

            return Task.FromResult(joinedCategories);
        }

        private static List<CategoriesWithCountDto> JoinResultCategoriesWithEmptyCategories(
            IEnumerable<ProductCatalogCategoryDto> categories,
            IDictionary<string, ArticleFilter> categoriesInQueryResult)
        {
            var summarization = new List<CategoriesWithCountDto>();

            foreach (var category in categories)
            {
                if (categoriesInQueryResult.ContainsKey(category.Code ?? string.Empty))
                {
                    var item = categoriesInQueryResult[category?.Code ?? string.Empty];

                    summarization.Add(new CategoriesWithCountDto
                    {
                        Amount = item.ArticleCount,
                        CategoryCode = item.Code,
                        CategoryName = string.IsNullOrWhiteSpace(category?.Name) ? item.Code : category?.Name
                    });
                    continue;
                }

                summarization.Add(new CategoriesWithCountDto
                {
                    Amount = 0,
                    CategoryCode = category?.Code ?? string.Empty,
                    CategoryName = string.IsNullOrWhiteSpace(category?.Name) ? category?.Code : category?.Name
                });
            }

            return summarization;
        }

        private IDictionary<string, ArticleFilter> BuildCategoriesDictionary(IEnumerable<ArticleFilter> categories)
            => categories.ToDictionary(k => k.Code);

        private async Task<ArticleQueryResult> CallApiAsync(GetCatalogProductsRequest request)
        {
            var api = _apetitoProductCatalogIProdaApi;

            var articleIdWithForbidden = request.ArticleIds.ToList();

            articleIdWithForbidden.AddRange(ForbiddenCategoriesFilters());

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
                ArticleIds = articleIdWithForbidden,
                CustomerNumber = request.CustomerNumber,
                FoodForms = request.FoodForms,
                PriceGroups = request.PriceGroups,
                OutletArticleWithoutStock = request.OutletArticleWithoutStock,
                GetArticleInLastValidSortiment = request.GetArticleInLastValidSortiment
            };

            return await api.RetrieveArticlesAsync<ArticleQueryResult>(query);
        }

        private GetCatalogProductsRequest PrepareSameRequestWithoutCategories(GetCatalogProductsRequest request)
        {
            var articleIdWithForbidden = request.ArticleIds.ToList();

            articleIdWithForbidden.AddRange(ForbiddenCategoriesFilters());

            return new GetCatalogProductsRequest
            {
                Acs = request.Acs,
                Additives = request.Additives,
                Allergens = request.Allergens,
                Categories = new List<string>(),
                Diets = request.Diets,
                Distinct = request.Distinct,
                Expand = request.Expand,
                Filter = request.Filter,
                Search = request.Search,
                Ids = request.Ids,
                Page = request.Page,
                Seals = request.Seals,
                Sortiments = request.Sortiments,
                ArticleIds = articleIdWithForbidden,
                CustomerNumber = request.CustomerNumber,
                ExpectedArticleType = request.ExpectedArticleType,
                FoodForms = request.FoodForms,
                PageSize = request.PageSize,
                PriceGroups = request.PriceGroups,
                SourceApis = request.SourceApis,
                OutletArticleWithoutStock = request.OutletArticleWithoutStock,
                GetArticleInLastValidSortiment = request.GetArticleInLastValidSortiment
            };
        }

        private IList<string> ForbiddenCategoriesFilters()
            => _productCatalogForbiddenArticlePrefixOptions.Select(fc
                => $"!{fc}*").ToList();
    }
}