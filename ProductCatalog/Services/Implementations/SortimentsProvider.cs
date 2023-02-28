using apetito.meinapetito.Portal.Application.ProductCatalog.Services.Interfaces;
using apetito.meinapetito.Portal.Contracts.IntegrationFields;

namespace apetito.meinapetito.Portal.Application.ProductCatalog.Services.Implementations;

public class SortimentsProvider : ISortimentsProvider
{
    private readonly IHttpClientFactory _httpClientFactory;

    public SortimentsProvider(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IList<Sortiment>> RetrieveAsync()
    {
        var client = _httpClientFactory.CreateClient();
        var result = await client.GetFromJsonAsync<List<Sortiment>>("https://api.apetito.de/article/iproda/sortiments") ?? new List<Sortiment>();

        return result;
    }
}