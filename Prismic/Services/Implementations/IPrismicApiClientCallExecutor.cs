using prismic;

namespace apetito.meinapetito.Portal.Application.Infrastructure.Prismic.Services.Implementations
{
    public interface IPrismicApiClientCallExecutor
    {
        Task<Document?> GetSingletonDocumentAsync(string? documentType,string? languageCode);
    }
}