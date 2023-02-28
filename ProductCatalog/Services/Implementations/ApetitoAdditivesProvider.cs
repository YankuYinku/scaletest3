using apetito.ArticleGateway.Contracts.ApiClient.V0.Components;
using apetito.meinapetito.Portal.Application.ProductCatalog.Services.Interfaces;
using apetito.meinapetito.Portal.Contracts.ProductCatalog.Models.Components;
using AutoMapper;

namespace apetito.meinapetito.Portal.Application.ProductCatalog.Services.Implementations;

public class ApetitoAdditivesProvider : IApetitoAdditivesProvider
{
    private readonly IArticleGatewayComponentsRestApi _api;
    private readonly IMapper _mapper;

    public ApetitoAdditivesProvider(IArticleGatewayComponentsRestApi api, IMapper mapper)
    {
        _api = api;
        _mapper = mapper;
    }
    public async Task<IEnumerable<AdditiveComponentDto>> GetAdditivesAsync()
    {
        var additives = await _api.RetrieveAdditivesAsync();

        return additives.Select(z => _mapper.Map<AdditiveComponentDto>(z));
    }
}