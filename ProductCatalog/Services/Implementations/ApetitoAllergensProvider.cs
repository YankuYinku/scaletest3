using apetito.ArticleGateway.Contracts.ApiClient.V0.Components;
using apetito.meinapetito.Portal.Application.ProductCatalog.Services.Interfaces;
using apetito.meinapetito.Portal.Contracts.ProductCatalog.Models.Components;
using AutoMapper;

namespace apetito.meinapetito.Portal.Application.ProductCatalog.Services.Implementations;

public class ApetitoAllergensProvider : IApetitoAllergensProvider
{
    private readonly IArticleGatewayComponentsRestApi _api;
    private readonly IMapper _mapper;
    public ApetitoAllergensProvider(IArticleGatewayComponentsRestApi api, IMapper mapper)
    {
        _api = api;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AllergenComponentDto>> GetAllergensAsync()
    {
        var diets = await _api.RetrieveAllergensAsync();

        return diets.Select(d => _mapper.Map<AllergenComponentDto>(d));
    }
}