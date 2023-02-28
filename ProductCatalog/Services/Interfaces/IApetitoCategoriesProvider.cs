using apetito.meinapetito.Portal.Contracts.ProductCatalog;
using apetito.meinapetito.Portal.Contracts.ProductCatalog.Models;

namespace apetito.meinapetito.Portal.Application.ProductCatalog.Services.Interfaces
{
    public interface IApetitoCategoriesProvider
    {
        Task<IQueryable<ProductCatalogCategoryDto>> GetCategoriesAsync(ArticleTypeRequestEnum expectedArticleType);
    }
}