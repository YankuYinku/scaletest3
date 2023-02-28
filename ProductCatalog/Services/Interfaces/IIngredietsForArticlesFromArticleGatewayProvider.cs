using apetito.ArticleGateway.Contracts.v0.Article;

namespace apetito.meinapetito.Portal.Application.ProductCatalog.Services.Interfaces;

public interface IIngredietsForArticlesFromArticleGatewayProvider
{
    Task<IDictionary<string, Tuple<IList<Ingredient>,IList<Category>>>> IngredientsAndCategoriesAsync();
}