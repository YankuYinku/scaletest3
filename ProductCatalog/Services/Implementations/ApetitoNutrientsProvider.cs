using apetito.ArticleGateway.Contracts.ApiClient.V0.Components;
using apetito.meinapetito.Portal.Application.ProductCatalog.Services.Interfaces;
using apetito.meinapetito.Portal.Contracts.ProductCatalog.Models.Components;
using AutoMapper;

namespace apetito.meinapetito.Portal.Application.ProductCatalog.Services.Implementations;

public class ApetitoNutrientsProvider : IApetitoNutrientsProvider
{
    private readonly IArticleGatewayComponentsRestApi _api;
    private readonly IMapper _mapper;

    public ApetitoNutrientsProvider(IArticleGatewayComponentsRestApi api, IMapper mapper)
    {
        _api = api;
        _mapper = mapper;
    }

    public async Task<IEnumerable<NutrientComponentDto>> GetNutrientsAsync()
    {
        var nutrient = await _api.RetrieveNutrientsAsync();

        return nutrient.Select(n => _mapper.Map<NutrientComponentDto>(n));
    }
}