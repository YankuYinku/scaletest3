using apetito.meinapetito.Portal.Contracts.ProductCatalog.Models.Filters;

namespace apetito.meinapetito.Portal.Application.ProductCatalog.Services.Interfaces;

public interface IApetitoFiltersProvider
{
    Task<FilterSetDto> GetFiltersAsync(string languageCode);
}