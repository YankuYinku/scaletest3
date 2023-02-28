using apetito.ArticleGateway.Contracts.v1.Structure;
using AutoMapper;

namespace apetito.meinapetito.Portal.Api.ProductCatalog.MappingProfiles.Converters;

public class OnionConverter<TSource, TDestination, TArticlePart> : ITypeConverter<TSource, IList<TDestination>>
    where TSource : ArticlePartList<TArticlePart> where TArticlePart : ArticlePart
{
    private readonly IMapper _mapper;

    public OnionConverter(IMapper mapper)
    {
        _mapper = mapper;
    }

    public IList<TDestination> Convert(TSource source, IList<TDestination> destination, ResolutionContext context)
    {
        return source.Items.Select(item =>Map(item)).ToList();
    }

    private TDestination Map(TArticlePart part)
    {
        var item = _mapper.Map<TDestination>(part);
        return item;
    }
}