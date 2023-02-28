using apetito.meinapetito.Portal.Contracts.ProductCatalog.Models;
using RestEase;

namespace apetito.meinapetito.Portal.Application.ProductCatalog.Services.Interfaces;

public interface IMeinApetitoApi
{
    [Get("root/authentication/userAccessTokens/legacy")]
    [Header("accept","*/*")]
    public Task<GetTokenFromApetitoLegacyModel> ExchangeAsync([Header("Authorization")] string authorization);

}