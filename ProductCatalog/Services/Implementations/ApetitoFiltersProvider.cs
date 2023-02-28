using apetito.meinapetito.Cache.Articles.Contracts.ProductCatalog.ApiClient;
using apetito.meinapetito.Portal.Application.ProductCatalog.Services.Interfaces;
using apetito.meinapetito.Portal.Contracts.ProductCatalog.Models.Filters;
using AutoMapper;

namespace apetito.meinapetito.Portal.Application.ProductCatalog.Services.Implementations;

public class ApetitoFiltersProvider : IApetitoFiltersProvider
{
    private readonly IFiltersRestClient _filtersRestClient;
    private readonly IMapper _mapper;

    public ApetitoFiltersProvider(IFiltersRestClient filtersRestClient, IMapper mapper)
    {
        _filtersRestClient = filtersRestClient;
        _mapper = mapper;
    }

    public async Task<FilterSetDto> GetFiltersAsync(string languageCode)
    {
        var result = await _filtersRestClient.GetAsync(languageCode);

        var mappedResult = _mapper.Map<FilterSetDto>(result);

        return mappedResult;
    }
}