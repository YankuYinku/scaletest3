using apetito.ArticleGateway.Contracts.ApiClient.V0.Components;
using apetito.meinapetito.Portal.Application.ProductCatalog.Services.Interfaces;
using apetito.meinapetito.Portal.Contracts.ProductCatalog.Models.Components;
using AutoMapper;

namespace apetito.meinapetito.Portal.Application.ProductCatalog.Services.Implementations;

public class ApetitoSealsProvider : IApetitoSealsProvider
{
    private readonly IArticleGatewayComponentsRestApi _api;
    private readonly IMapper _mapper;
    public ApetitoSealsProvider(IArticleGatewayComponentsRestApi api, IMapper mapper)
    {
        _api = api;
        _mapper = mapper;
    }
    public async Task<IEnumerable<SealComponentDto>> GetSealsAsync()
    {
        var seals = await _api.RetrieveSealsAsync();

        return seals.Select(z => _mapper.Map<SealComponentDto>(z));
    }
}