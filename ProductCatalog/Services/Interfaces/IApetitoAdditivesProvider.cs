using apetito.meinapetito.Portal.Contracts.ProductCatalog.Models.Components;

namespace apetito.meinapetito.Portal.Application.ProductCatalog.Services.Interfaces;

public interface IApetitoAdditivesProvider
{
    Task<IEnumerable<AdditiveComponentDto>> GetAdditivesAsync();
}