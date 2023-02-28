
using apetito.meinapetito.Portal.Application.Infrastructure.Prismic.Options;
using apetito.meinapetito.Portal.Application.Infrastructure.Prismic.Services.Implementations;
using prismic;

namespace apetito.meinapetito.Portal.Application.Infrastructure.Prismic.Services.Interfaces
{
    public class PrismicApiClientCallExecutor : IPrismicApiClientCallExecutor
    {
        private readonly PrismicOptions _prismicOptions;

        public PrismicApiClientCallExecutor(PrismicOptions options)
        {
            _prismicOptions = options;
        }

        public async Task<Document?> GetSingletonDocumentAsync(string? documentType,string? languageCode)
        {
            var api = await Api.Get(_prismicOptions.Endpoint, _prismicOptions.AccessToken);
            
            var result = await api.Query(Predicates.at("document.type",documentType)).Set("lang",languageCode ).Submit();

            var documents = result.Results;
            
            return documents.FirstOrDefault();
        }
    }
}