using apetito.meinapetito.Portal.Contracts.ProductCatalog.Models;

namespace apetito.meinapetito.Portal.Application.ProductCatalog.Services.Interfaces
{
    public interface IApetitoProductsFromCacheProvider
    {
        Task<GetProductListResultDto> GetProductsAsync(GetCatalogProductsRequest request);
    }
}