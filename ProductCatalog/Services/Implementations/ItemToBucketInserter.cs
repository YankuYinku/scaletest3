using apetito.iProDa3.Contracts.ArticleBrowser;
using apetito.iProDa3.Contracts.ArticleBrowser.Models;
using apetito.meinapetito.Portal.Application.ProductCatalog.Services.Interfaces;
using apetito.meinapetito.Portal.Contracts.ProductCatalog.Models.Carts;
using AddItemToBasketRequestModel = apetito.meinapetito.Portal.Contracts.ProductCatalog.Models.AddItemToBasketRequestModel;

namespace apetito.meinapetito.Portal.Application.ProductCatalog.Services.Implementations;

public class ItemToBucketInserter : IItemToBasketInserter
{
    private readonly IIProDa3ArticleBrowserRestClient _api;

    public ItemToBucketInserter(IIProDa3ArticleBrowserRestClient api)
    {
        _api = api;
    }
    
    public async Task<AddItemsToBasketResultDto> AddItemsToBasketAsync(IList<AddItemToBasketRequestModel> request)
    {
        var result = await _api.AddItemsToPartnerPortalBasket(request.Select(a => new BasketArticle()
        {
            Quantity = a.Quantity,
            Sortiment = string.IsNullOrWhiteSpace(a.Sortiment) ? string.Empty : a.Sortiment,
            ArticleNumber = a.ArticleId
        }).ToList());

        var succeeded = result.Successes;
        var failed = request.Select(a => a.ArticleId).Where(z => !succeeded.Contains(z)).ToList();

        return new AddItemsToBasketResultDto
        {
            FailedArticles = failed,
            SucceededArticles = succeeded
        };
    }
}