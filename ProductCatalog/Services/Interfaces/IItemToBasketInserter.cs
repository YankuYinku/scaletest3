using apetito.meinapetito.Portal.Contracts.ProductCatalog.Models;
using apetito.meinapetito.Portal.Contracts.ProductCatalog.Models.Carts;

namespace apetito.meinapetito.Portal.Application.ProductCatalog.Services.Interfaces;

public interface IItemToBasketInserter
{
    Task<AddItemsToBasketResultDto> AddItemsToBasketAsync(IList<AddItemToBasketRequestModel> request);
}