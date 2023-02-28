using apetito.meinapetito.Portal.Contracts.Downloads.Models;
using apetito.meinapetito.Portal.Contracts.News;

namespace apetito.meinapetito.Portal.Application.News.Services.Interfaces;

public interface INewsItemProvider
{
    Task<GetNewsItemsResult> GetItemsAsync(NewsQueryRequest request);
    Task<IList<NewsCategoryDto>> GetCategoriesAsync(string languageCodes);
}