using apetito.meinapetito.Cache.News.Contracts.News.Models;
using apetito.meinapetito.Portal.Contracts.News;
using AutoMapper;

namespace apetito.meinapetito.Portal.Api.News;

public class NewsAutomapperProfile : Profile
{
    public NewsAutomapperProfile()
    {
        CreateMap<NewsCacheDto, NewsDto>();
        CreateMap<NewsCategoryCacheDto, NewsCategoryDto>()
            .ForMember(a=> a.Name, opts => opts.MapFrom(a => a.Title));
        CreateMap<NewsCacheFileSortimentDto, NewsSortimentDto>();
        CreateMap<GetNewsCacheResult, GetNewsItemsResult>();
        CreateMap<NewsImageCacheDto, NewsImageDto>();
        CreateMap<NewsCacheCategoriesWithCountDto, NewsCategoriesWithCountDto>();
        
        
    }
    
}