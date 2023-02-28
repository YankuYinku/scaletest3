using apetito.meinapetito.Portal.Application.ProductCatalog.Services.Interfaces;
using apetito.meinapetito.Portal.Contracts.ProductCatalog.Models;
using apetito.meinapetito.Portal.Contracts.ProductCatalog.Models.Carts;
using HotChocolate;
using HotChocolate.Types;

// ReSharper disable once CheckNamespace
namespace apetito.meinapetito.Portal.Api.GraphQl
{
    [ExtendObjectType("Mutation")]
    public class BasketMutation
    {
        public async Task<AddItemsToBasketResultDto> AddItemToBasketAsync(
            [Service] IItemToBasketInserter itemToBasketInserter,
            AddItemToBasketRequestModel request)
        {
            var result = await itemToBasketInserter.AddItemsToBasketAsync(new List<AddItemToBasketRequestModel>()
            {
                request
            });
            
            return result;
        }
        
        public async Task<AddItemsToBasketResultDto> AddItemsToBasketAsync(
            [Service] IItemToBasketInserter itemToBasketInserter,
            IList<AddItemToBasketRequestModel> requests)
        {
            var result = await itemToBasketInserter.AddItemsToBasketAsync(requests);
            return result;
        }
    }
}