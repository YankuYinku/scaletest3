using apetito.meinapetito.Portal.Contracts.ProductCatalog.Models;

namespace apetito.meinapetito.Portal.Application.ProductCatalog.Services.Interfaces
{
    public interface IApetitoProductsWithoutCategoriesProvider
    {
        Task<GetProductListResultDto> GetProductsWithoutMergedCategoriesAsync(GetCatalogProductsRequest request);
    }
}