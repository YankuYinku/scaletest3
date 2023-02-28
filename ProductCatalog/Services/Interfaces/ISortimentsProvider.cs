using apetito.meinapetito.Portal.Contracts.IntegrationFields;

namespace apetito.meinapetito.Portal.Application.ProductCatalog.Services.Interfaces;

public interface ISortimentsProvider
{
    Task<IList<Sortiment>> RetrieveAsync();
}