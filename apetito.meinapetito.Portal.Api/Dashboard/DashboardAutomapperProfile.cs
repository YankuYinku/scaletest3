using apetito.ArticleGateway.Contracts.v1.Article;
using apetito.ArticleGateway.Contracts.v1.Structure;
using apetito.iProDa3.Contracts.ApiClient.RequestModels;
using apetito.meinapetito.Portal.Application.Dashboard.Enums;
using apetito.meinapetito.Portal.Application.Dashboard.Services.Interfaces;
using apetito.meinapetito.Portal.Contracts.Dashboard.Models;
using AutoMapper;

namespace apetito.meinapetito.Portal.Api.Dashboard;

public class DashboardAutomapperProfile : Profile
{

    public DashboardAutomapperProfile()
    {
        MapDashboardObjects();
    }
    
    private void MapDashboardObjects()
    {
        CreateMap<ArticleDetails, DashboardArticleDetailsDto>().ForMember(dest
            => dest.Price, opts
            => opts.MapFrom(src => src.Prices.Items.FirstOrDefault(a => a.PriceGroupCode == "TK")));

        CreateMap<Article, DashboardArticleDto>()
            .ForMember(dest
                => dest.Id, opts
                => opts.MapFrom(src => src.ArticleNumber))
            .ForMember(
                dest
                    => dest.Title, opts
                    => opts.MapFrom(src => src.Details.Designations.MainDesignation.Text)
            ).ForMember(
                dest
                    => dest.Description, opts
                    => opts.MapFrom(src => src.Details.Designations.MainDesignation.AdditionalText)
            )
            .ForMember(
                dest
                    => dest.Image, opts
                    => opts.MapFrom(src => src.ArticleNumber)
            )
            .ForMember(
                dest
                    => dest.Details, opts
                    => opts.MapFrom(src => src.Details)
            );

        CreateMap<ArticleGateway.Contracts.v1.Article.Additive, DashboardAdditiveDto>();
        CreateMap<ArticleGateway.Contracts.v1.Article.Allergen, DashboardAllergenDto>();
        CreateMap<string, DashboardImageDto>().ConvertUsing<ImageConverter>();
        CreateMap<ArticleGateway.Contracts.v1.Article.Diet, DashboardDietDto>();
        CreateMap<Component, DashboardComponentDto>();
        CreateMap<ContentWeightSalesUnit, DashboardContentWeightSalesUnitDto>();
        CreateMap<WeightInUnit, DashboardWeightInUnitDto>();
        CreateMap<SortimentArticleAssignment, DashboardSortimentAssignmentDto>();
        CreateMap<Sortiment, DashboardSortimentDto>();
        CreateMap<SortimentList, IList<DashboardSortimentDto>>();
        CreateMap<ValidityPeriod, DashboardValidityPeriodDto>();
        CreateMap<ArticleGateway.Contracts.v1.Article.Seal, DashboardSealDto>();

        CreateMap<SealList, IList<DashboardSealDto>>()
            .ConvertUsing<
                OnionConverter<SealList, DashboardSealDto, apetito.ArticleGateway.Contracts.v1.Article.Seal>>();
        CreateMap<DesignationsList, IList<DashboardDesignationDto>>()
            .ConvertUsing<OnionConverter<DesignationsList, DashboardDesignationDto, Designation>>();
        CreateMap<SortimentList, IList<DashboardSortimentAssignmentDto>>()
            .ConvertUsing<OnionConverter<SortimentList, DashboardSortimentAssignmentDto, SortimentArticleAssignment>>();
        CreateMap<WeightRangeList, IList<DashboardWeightRangeDto>>()
            .ConvertUsing<OnionConverter<WeightRangeList, DashboardWeightRangeDto, WeightRange>>();
        CreateMap<WeightComponentList, IList<DashboardWeightComponentDto>>()
            .ConvertUsing<OnionConverter<WeightComponentList, DashboardWeightComponentDto, WeightComponent>>();

        CreateMap<PreparationFeature, DashboardPreparationFeatureDto>();
        CreateMap<ArticleGateway.Contracts.v1.Article.Nutrient, DashboardNutrientDto>();
        CreateMap<Designation, DashboardDesignationDto>();
        CreateMap<WeightComponent, DashboardWeightComponentDto>();
        CreateMap<Component, DashboardComponentDto>();

        CreateMap<Price, DashboardPriceDto>();
        CreateMap<IngredientSummary, DashboardIngredientSummaryDto>();
        CreateMap<WeightComponents, DashboardWeightComponentsDto>();
        CreateMap<WeightRangeList, DashboardWeightRangeDto>();
        CreateMap<WeightInUnit, DashboardWeightInUnitDto>();
        CreateMap<WeightComponentListSalesUnit,
            DashboardWeightComponentListSalesUnitDto>();
        CreateMap<WeightComponentList, DashboardWeightComponentDto>();
        CreateMap<Unit, DashboardUnitDto>();
        
        CreateMap<GetProductsRequestModel, RetrieveArticlesQuery>();
    }
    
    public class ImageConverter : ITypeConverter<string, DashboardImageDto>
    {
        private readonly IDashboardPhotoPathBuilder _dashboardPhotoPathBuilder;

        public ImageConverter(IDashboardPhotoPathBuilder dashboardPhotoPathBuilder)
        {
            _dashboardPhotoPathBuilder = dashboardPhotoPathBuilder;
        }

        public DashboardImageDto Convert(string source, DashboardImageDto destination, ResolutionContext context)
        {
            return new DashboardImageDto
            {
                Small = _dashboardPhotoPathBuilder.BuildPhotoPath(source, ImageSize.Small),
                Middle = _dashboardPhotoPathBuilder.BuildPhotoPath(source, ImageSize.Middle),
                Big = _dashboardPhotoPathBuilder.BuildPhotoPath(source, ImageSize.Big),
            };
        }
    }

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
            return source.Items.Select(item => _mapper.Map<TDestination>(item)).ToList();
        }
    }
}