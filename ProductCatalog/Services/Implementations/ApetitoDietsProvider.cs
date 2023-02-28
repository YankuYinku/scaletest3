using apetito.ArticleGateway.Contracts.ApiClient.V0.Components;
using apetito.meinapetito.Portal.Application.ProductCatalog.Services.Interfaces;
using apetito.meinapetito.Portal.Contracts.ProductCatalog.Models.Components;

namespace apetito.meinapetito.Portal.Application.ProductCatalog.Services.Implementations
{
    public class ApetitoDietsProvider : IApetitoDietsProvider
    {
        private readonly IArticleGatewayComponentsRestApi _api;

        public ApetitoDietsProvider(IArticleGatewayComponentsRestApi api)
        {
            _api = api;
        }

        public async Task<IEnumerable<DietComponentDto>> GetDietsAsync()
        {
            var result = await _api.RetrieveDietsAsync();
            return result.Select(z => new DietComponentDto
            {
                Code = z.Code,
                Name = z.Name,
                SortOrder = z.SortOrder
            });
        }
    }
}