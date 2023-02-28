using apetito.meinapetito.Cache.Faqs.Contracts.Faqs.Models;
using apetito.meinapetito.Portal.Contracts.Faqs.Models;
using AutoMapper;

namespace apetito.meinapetito.Portal.Api.Faqs;

public class FaqsAutomapperProfile : Profile
{
    public FaqsAutomapperProfile()
    {
        CreateMap<GetFaqsCacheItemsResult, GetFaqsItemsResult>();
        CreateMap<FaqCacheGroupDto, FaqGroupDto>();
        CreateMap<FaqCacheDto, FaqDto>();
        CreateMap<FaqCacheImage, FaqImage>();
        CreateMap<FaqCacheItemDto, FaqItemDto>();
        CreateMap<FaqCacheSimpleItem, FaqSimpleItem>();
        CreateMap<FaqCacheVideo, FaqVideo>();
        CreateMap<FaqCacheVideoItem, FaqVideoItem>();
    }
}