using apetito.meinapetito.Cache.Articles.Contracts.ProductCatalog.ApiClient;
using apetito.meinapetito.Cache.Articles.Contracts.ProductCatalog.Models;
using apetito.meinapetito.Portal.Application.ProductCatalog.Consts;
using apetito.meinapetito.Portal.Application.ProductCatalog.Services.Interfaces;
using apetito.meinapetito.Portal.Contracts.ProductCatalog;
using apetito.meinapetito.Portal.Contracts.ProductCatalog.Models;
using AutoMapper;

namespace apetito.meinapetito.Portal.Application.ProductCatalog.Services.Implementations;

public class ApetitoProductsFromCacheProvider : IApetitoProductsFromCacheProvider
{
    private readonly IArticlesRestClient _articlesRestClient;
    private readonly ILogger<ApetitoProductsFromCacheProvider> _logger;
    private readonly IApetitoCategoriesProvider _apetitoCategoriesProvider;
    private readonly IMapper _mapper;

    public ApetitoProductsFromCacheProvider(IArticlesRestClient articlesRestClient,
        IApetitoCategoriesProvider apetitoCategoriesProvider,
        ILogger<ApetitoProductsFromCacheProvider> logger,
        IMapper mapper)
    {
        _articlesRestClient = articlesRestClient;
        _apetitoCategoriesProvider = apetitoCategoriesProvider;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<GetProductListResultDto> GetProductsAsync(GetCatalogProductsRequest request)
    {
        Task<IQueryable<ProductCatalogCategoryDto>> categoriesTask =
            _apetitoCategoriesProvider.GetCategoriesAsync(request.ExpectedArticleType);

        IDictionary<string, CategoriesWithCountDto> categories = new Dictionary<string, CategoriesWithCountDto>();


        var result = await _articlesRestClient.GetArticlesAsync(new GetProductsQuery()
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
            Seals = request.Seals,
            Search = request.Search,
            Sortiments = request.Sortiments,
            ArticleIds = request.ArticleIds,
            CustomerNumber = request.CustomerNumber,
            FoodForms = request.FoodForms,
            PageSize = request.PageSize,
            PriceGroups = request.PriceGroups,
            SourceApis = request.SourceApis,
            SortimentType = null,
            OutletArticleWithoutStock = request.OutletArticleWithoutStock,
            GetArticleInLastValidSortiment = request.GetArticleInLastValidSortiment,
            LanguageCode = request.LanguageCode,
        });

        IDictionary<string, ProductCatalogCategoryDto> categoriesDictionary =
            (await categoriesTask).ToDictionary(c => c.Code);

        var summarizationDictionary = result.CategoriesSummarization.ToDictionary(a => a.CategoryCode);

        foreach (var category in categoriesDictionary)
        {
            categories.Add(new KeyValuePair<string, CategoriesWithCountDto>(category.Key, new CategoriesWithCountDto
            {
                Amount = summarizationDictionary.TryGetValue(category.Key, out var categoryValue)
                    ? categoryValue.Amount
                    : 0,
                CategoryCode = category.Key,
                CategoryName = string.IsNullOrWhiteSpace(category.Value.Name) ?  category.Key : category.Value.Name
            }));
        }

        var articles = new List<ProductCatalogArticleDto>();
        try
        {
            articles = result.Articles.Select(a => _mapper.Map<ProductCatalogArticleDto>(a, opts
                => opts.Items.Add(MapperConstKeys.MapperQueryContext, request.ExpectedArticleType))).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, ex?.Message);
        }

        var categoriesItems = categories.Select(c => c.Value);

        if (request.ExpectedArticleType == ArticleTypeRequestEnum.Material)
        {
            categoriesItems = categoriesItems.Where(a => a.Amount != 0);
        }

        var articlesWithoutNutriScore = RemoveNutriscoreDiet(articles);

        return new GetProductListResultDto()
        {
            OverallPages = result.OverallPages,
            OverallResults = result.OverallResults,
            OverallItemsInAllCategories =result.OverallItemsInAllCategories,
            CategoriesSummarization = categoriesItems,
            Articles = articlesWithoutNutriScore
        };
    }

    private IList<ProductCatalogArticleDto> RemoveNutriscoreDiet(IList<ProductCatalogArticleDto> articles)
    {
        foreach (var article in articles)
        {
            var nutriScoreDiet = article.Details.Diets.FirstOrDefault(a => a.Code.StartsWith("SCORE"));
            if (nutriScoreDiet is not null)
            {
                article.Details.Diets.Remove(nutriScoreDiet);
            }
        }

        return articles;
    }
}