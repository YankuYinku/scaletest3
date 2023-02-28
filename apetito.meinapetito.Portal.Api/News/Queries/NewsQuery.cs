using apetito.meinapetito.Portal.Application.News.Services.Interfaces;
using apetito.meinapetito.Portal.Contracts.News;
using HotChocolate.Types;

namespace apetito.meinapetito.Portal.Api.News.Queries
{
    [ExtendObjectType("Query")]
    public class NewsQuery
    {
        public async Task<GetNewsItemsResult> GetNewsAsync([FromServices] INewsItemProvider newsItemProvider,
            NewsQueryRequest request)
            => await newsItemProvider.GetItemsAsync(request);


        public async Task<IList<NewsCategoryDto>> GetNewsCategoriesAsync(
            [FromServices] INewsItemProvider newsItemProvider, string languageCode)
            => await newsItemProvider.GetCategoriesAsync(string.IsNullOrWhiteSpace(languageCode) ? "de-de" : languageCode);
    }
}