using System.Net;
using apetito.meinapetito.Cache.News.Contracts.News.ApiClients;
using apetito.meinapetito.Cache.News.Contracts.News.Models;
using apetito.meinapetito.Portal.Application.News.Services.Interfaces;
using apetito.meinapetito.Portal.Contracts.News;
using AutoMapper;

namespace apetito.meinapetito.Portal.Application.News.Services.Implementations;

public class NewsItemProvider : INewsItemProvider
{
    private readonly INewsCacheApiClient _newsCacheApiClient;
    private readonly IMapper _mapper;

    public NewsItemProvider(INewsCacheApiClient newsCacheApiClient, IMapper mapper)
    {
        _newsCacheApiClient = newsCacheApiClient;
        _mapper = mapper;
    }

    public async Task<GetNewsItemsResult> GetItemsAsync(NewsQueryRequest request)
    {
        var requestRestEase = new NewsCacheQueryRequest()
        {
            DistributionChannels = request.Areas,
            Keywords = request.Keywords,
            Page = request.Page,
            Search = request.SearchTerm,
            Sortiments = request.Sortiments,
            CustomerNumbers = request.CustomerNumbers?.Select(a => a.ToString()).ToList(),
            Categories = new List<string>()
            {
                WebUtility.UrlEncode(request.Category) ?? string.Empty
            }.Where(a => !string.IsNullOrWhiteSpace(a)).ToList(),
            OrderSystems = request.OrderSystems,
            PageSize = request.PageSize,
            LanguageCode = string.IsNullOrWhiteSpace(request.LanguageCode) ? "de-de" : request.LanguageCode,
            SortOrder = request.SortOrder ?? string.Empty
        };
        
        
        var result = await _newsCacheApiClient.GetNewsAsync(
            requestRestEase);

        return _mapper.Map<GetNewsItemsResult>(result);
    }

    public async Task<IList<NewsCategoryDto>> GetCategoriesAsync(string languageCode)
    {
        var result = await _newsCacheApiClient.GetNewsCategoriesAsync(languageCode);

        return result.Select(a => _mapper.Map<NewsCategoryDto>(a)).ToList();
    }
}