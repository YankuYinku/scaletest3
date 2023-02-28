using apetito.meinapetito.Portal.Application.ProductCatalog.Services.Interfaces;
using apetito.meinapetito.Portal.Contracts.ProductCatalog.Models.Filters;
using HotChocolate;
using HotChocolate.Types;

namespace apetito.meinapetito.Portal.Api.ProductCatalog.Queries;

[ExtendObjectType("Query")]
public class FiltersQuery
{
    public async Task<FilterSetDto> RetrieveFilterSetAsync(string languageCode,
        [Service] IApetitoFiltersProvider apetitoFiltersProvider)
    {
        return await apetitoFiltersProvider.GetFiltersAsync(languageCode);
    }
}