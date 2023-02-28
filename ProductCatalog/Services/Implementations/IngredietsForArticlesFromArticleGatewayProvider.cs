using apetito.ArticleGateway.Contracts.ApiClient.V0;
using apetito.ArticleGateway.Contracts.Querying;
using apetito.ArticleGateway.Contracts.v0.Article;
using apetito.ArticleGateway.Contracts.v0.Querying;
using apetito.meinapetito.Portal.Application.ProductCatalog.Services.Interfaces;

namespace apetito.meinapetito.Portal.Application.ProductCatalog.Services.Implementations;

public class IngredietsForArticlesFromArticleGatewayProvider : IIngredietsForArticlesFromArticleGatewayProvider
{
    private readonly IArticleGatewayArticlesV0RestApi _api;
    private readonly ILogger<IngredietsForArticlesFromArticleGatewayProvider> _logger;

    public IngredietsForArticlesFromArticleGatewayProvider(IArticleGatewayArticlesV0RestApi api, ILogger<IngredietsForArticlesFromArticleGatewayProvider> logger)
    {
        _api = api;
        _logger = logger;
    }

    public async Task<IDictionary<string, Tuple<IList<Ingredient>,IList<Category>>>> IngredientsAndCategoriesAsync()
    {
        var dictionary = new Dictionary<string, Tuple<IList<Ingredient>, IList<Category>>>();

        var z = await _api.RetrieveArticlesAsync(PrepareQuery(1));

        foreach (var article in z.Items)
        {
            dictionary.Add(article.ArticleNumber, new Tuple<IList<Ingredient>, IList<Category>>(article.Ingredients,article.CategorieExtended));
        }
        
        var pages = z.OverallPages;

        var calls = new List<Task<ArticleQueryResult>>();

        for (var i = 2; i < pages; i++)
        {
            calls.Add(_api.RetrieveArticlesAsync(PrepareQuery(i)));
        }

        foreach (var task in calls)
        {
            var p = await task;

            foreach (var article in p.Items)
            {
                if (dictionary.ContainsKey(article.ArticleNumber))
                {
                    _logger.Log(LogLevel.Warning,"Item already exists");
                    continue;
                }
                dictionary.Add(article.ArticleNumber, new Tuple<IList<Ingredient>, IList<Category>>(article.Ingredients,article.CategorieExtended));
            }
        }

        return dictionary;
    }

    private static ArticleQuery PrepareQuery(int i)
    {
        return new ArticleQuery
        {
            Query = new Query
            {
                Page = i,
                PageSize = PageSize
            }
        };
    }
    private const int PageSize = 50;
}