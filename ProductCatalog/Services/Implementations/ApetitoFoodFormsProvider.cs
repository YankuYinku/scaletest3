using apetito.ArticleGateway.Contracts.ApiClient.V0.Components;
using apetito.meinapetito.Portal.Application.ProductCatalog.Services.Interfaces;
using apetito.meinapetito.Portal.Contracts.ProductCatalog.Models.Components;
using AutoMapper;

namespace apetito.meinapetito.Portal.Application.ProductCatalog.Services.Implementations;

public class ApetitoFoodFormsProvider : IApetitoFoodFormsProvider
{
    private readonly IArticleGatewayComponentsRestApi _api;
    private readonly IMapper _mapper;

    public ApetitoFoodFormsProvider(IArticleGatewayComponentsRestApi api, IMapper mapper)
    {
        _api = api;
        _mapper = mapper;
    }
    public async Task<IEnumerable<FoodFormComponentDto>> GetFoodFormsAsync()
    {
        var foodForms = await _api.RetrieveFoodFormsAsync();

        return foodForms.Select(z => _mapper.Map<FoodFormComponentDto>(z));
    }
}