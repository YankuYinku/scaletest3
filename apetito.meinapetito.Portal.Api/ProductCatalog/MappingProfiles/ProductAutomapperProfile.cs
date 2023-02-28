using apetito.ArticleGateway.Contracts.Querying;
using apetito.ArticleGateway.Contracts.v0.Components;
using apetito.ArticleGateway.Contracts.v1.Article;
using apetito.meinapetito.Cache.Articles.Contracts.ProductCatalog.Models;
using apetito.meinapetito.Portal.Api.ProductCatalog.MappingProfiles.Converters;
using apetito.meinapetito.Portal.Contracts.ProductCatalog.Models;
using apetito.meinapetito.Portal.Contracts.ProductCatalog.Models.Components;
using apetito.meinapetito.Portal.Contracts.ProductCatalog.Models.Filters;
using apetito.meinapetito.Portal.Data.Root.Users;
using AutoMapper;
using Additive = apetito.ArticleGateway.Contracts.v0.Components.Additive;
using Allergen = apetito.ArticleGateway.Contracts.v0.Components.Allergen;
using Category = apetito.ArticleGateway.Contracts.v0.Category.Category;
using Diet = apetito.ArticleGateway.Contracts.v0.Components.Diet;
using Nutrient = apetito.ArticleGateway.Contracts.v0.Components.Nutrient;
using Seal = apetito.ArticleGateway.Contracts.v0.Components.Seal;

namespace apetito.meinapetito.Portal.Api.ProductCatalog.MappingProfiles;

public class ProductAutomapperProfile : Profile
{
    public ProductAutomapperProfile()
    {
        MapComponents();
        MapCatalogProductObjects();
        MapCacheObjects();
    }

    private void MapComponents()
    {
        CreateMap<Additive, AdditiveComponentDto>();
        CreateMap<Allergen, AllergenComponentDto>();
        CreateMap<Diet, DietComponentDto>();
        CreateMap<FoodForm, FoodFormComponentDto>();
        CreateMap<Nutrient, NutrientComponentDto>();
        CreateMap<Seal, SealComponentDto>();
        CreateMap<Category, ProductCatalogCategoryDto>()
            .ForMember(dest
                => dest.Name, opts
                => opts.MapFrom(src => src.Text));
    }

    private void MapCatalogProductObjects()
    {
        CreateMap<ArticleDetails, ProductCatalogArticleDetailsDto>()
            .ForMember(dest
                => dest.Price, opts
                => opts.MapFrom(src => src.Prices.Items.FirstOrDefault(a => a.PriceGroupCode == PriceCode)))
            .ForMember(dest => dest.Categories, opts => opts.MapFrom(src =>
                src.Sortiments.Items.Select(s => s.Details.Categories).SelectMany(a => a.Items)
                    .DistinctBy(a => a.Code)));

        CreateMap<Article, ProductCatalogArticleDto>()
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


        CreateMap<ArticleGateway.Contracts.v1.Article.Additive, ProductCatalogAdditiveDto>();
        CreateMap<ArticleGateway.Contracts.v1.Article.Allergen, ProductCatalogAllergenDto>();
        CreateMap<ArticleFilter, ProductCatalogCategoryDto>()
            .ForMember(a => a.Name, opts => opts.MapFrom(a => a.Text));
        CreateMap<string, ProductCatalogImageDto>().ConvertUsing<ProductCatalogImageConverter>();
        CreateMap<ArticleGateway.Contracts.v1.Article.Diet, ProductCatalogDietDto>();
        CreateMap<Component, ProductCatalogComponentDto>();
        CreateMap<Taxanomy, ProductCatalogTaxanomyDto>();
        CreateMap<ContentWeightSalesUnit, ProductCatalogContentWeightSalesUnitDto>();
        CreateMap<WeightInUnit, ProductCatalogWeightInUnitDto>();
        CreateMap<Eyecatcher, ProductCatalogSortimentEvecatcherDto>();
        CreateMap<Portion, ProductCatalogSuggestedPortionDto>();
        CreateMap<OutletArticleTypeEnum, ProductCatalogOutletArticleTypeDto>();
        CreateMap<SortimentPromotionalCode, ProductCatalogPromotionalCodeDto>();
        CreateMap<ConversionUnitList, ConversionUnitListDto>();
        CreateMap<AmountInUnit, ProductCatalogAmountInUnitDto>();
CreateMap<apetito.ArticleGateway.Contracts.v1.Article.Category, ProductCatalogCategoryDto>();
        
        CreateMap<LogisticInformation, ProductCatalogLogisticDto>()
            .ForMember(dest => dest.ConversionUnits, opts => opts.MapFrom(src => src.ConversionUnits));

        CreateMap<SortimentArticleAssignment, ProductCatalogSortimentAssignmentDto>()
            .ForMember(
                dest
                    => dest.Eyecacher, opts
                    => opts.MapFrom(src => src.Details.Eyecatcher))
            .ForMember(
                dest
                    => dest.ChargableQuantity, opts
                    => opts.MapFrom(src => src.Details.ChargableQuantity))
            .ForMember(
                dest
                    => dest.PromotionalCode, opts
                    => opts.MapFrom(src => src.Details.PromotionalCode))
            .ForMember(
                dest
                    => dest.SuggestedPortion, opts
                    => opts.MapFrom(src => src.Details.SuggestedPortion))
            .ForMember(
                dest
                    => dest.OutletArticleType, opts
                    => opts.MapFrom(src => src.Details.OutletArticleType));
        CreateMap<Sortiment, ProductCatalogSortimentDto>();
        CreateMap<apetito.ArticleGateway.Contracts.v1.Article.Category, ProductCatalogCategoryDto>();
        CreateMap<SortimentList, IList<ProductCatalogSortimentDto>>();
        CreateMap<SortimentList, IList<ProductCatalogSortimentDto>>();
        CreateMap<ValidityPeriod, ProductCatalogValidityPeriodDto>();
        CreateMap<ArticleGateway.Contracts.v1.Article.Seal, ProductCatalogSealDto>();


        CreateMap<SealList, IList<ProductCatalogSealDto>>()
            .ConvertUsing<Converters.OnionConverter<SealList, ProductCatalogSealDto, apetito.ArticleGateway.Contracts.v1.Article.Seal>>();

        CreateMap<AdditiveList, IList<ProductCatalogAdditiveDto>>()
            .ConvertUsing<Converters.OnionConverter<AdditiveList, ProductCatalogAdditiveDto,
                    apetito.ArticleGateway.Contracts.v1.Article.Additive>>();

        CreateMap<AllergenList, IList<ProductCatalogAllergenDto>>()
            .ConvertUsing<Converters.OnionConverter<AllergenList, ProductCatalogAllergenDto,
                    apetito.ArticleGateway.Contracts.v1.Article.Allergen>>();

        CreateMap<DietList, IList<ProductCatalogDietDto>>()
            .ConvertUsing<Converters.OnionConverter<DietList, ProductCatalogDietDto, apetito.ArticleGateway.Contracts.v1.Article.Diet>>();

        CreateMap<CompontentList, IList<ProductCatalogComponentDto>>()
            .ConvertUsing<Converters.OnionConverter<CompontentList, ProductCatalogComponentDto, Component>>();

        CreateMap<CategoryList, IList<ProductCatalogCategoryDto>>()
            .ConvertUsing<Converters.OnionConverter<CategoryList, ProductCatalogCategoryDto,
                    apetito.ArticleGateway.Contracts.v1.Article.Category>>();

        CreateMap<NutrientList, IList<ProductCatalogNutrientDto>>()
            .ConvertUsing<Converters.OnionConverter<NutrientList, ProductCatalogNutrientDto,
                    apetito.ArticleGateway.Contracts.v1.Article.Nutrient>>();

        CreateMap<PreparationFeatureList, IList<ProductCatalogPreparationFeatureDto>>()
            .ConvertUsing<Converters.OnionConverter<PreparationFeatureList, ProductCatalogPreparationFeatureDto,
                    PreparationFeature>>();

        CreateMap<PreparationInstructionList, IList<ProductCatalogPreparationInstructionDto>>()
            .ConvertUsing<Converters.OnionConverter<PreparationInstructionList, ProductCatalogPreparationInstructionDto,
                    PreparationInstruction>>();

        CreateMap<PriceList, IList<ProductCatalogPriceDto>>()
            .ConvertUsing<Converters.OnionConverter<PriceList, ProductCatalogPriceDto, Price>>();


        CreateMap<DesignationsList, IList<ProductCatalogDesignationDto>>()
            .ConvertUsing<Converters.OnionConverter<DesignationsList, ProductCatalogDesignationDto, Designation>>();
        CreateMap<SortimentList, IList<ProductCatalogSortimentAssignmentDto>>()
            .ConvertUsing<Converters.OnionConverter<SortimentList, ProductCatalogSortimentAssignmentDto,
                SortimentArticleAssignment>>();
        CreateMap<WeightRangeList, IList<ProductCatalogWeightRangeDto>>()
            .ConvertUsing<Converters.OnionConverter<WeightRangeList, ProductCatalogWeightRangeDto, WeightRange>>();
        CreateMap<WeightComponentList, IList<ProductCatalogWeightComponentDto>>()
            .ConvertUsing<Converters.OnionConverter<WeightComponentList, ProductCatalogWeightComponentDto, WeightComponent>>();
        CreateMap<PriceClassList, IList<ProductCatalogPriceClassDto>>()
            .ConvertUsing<Converters.OnionConverter<PriceClassList, ProductCatalogPriceClassDto, PriceClass>>();
        CreateMap<PreparationFeature, ProductCatalogPreparationFeatureDto>();
        CreateMap<PreparationInstruction, ProductCatalogPreparationInstructionDto>()
            .ForMember(
                dest
                    => dest.LabelOrder, opts
                    => opts.MapFrom(src => src.Details.LabelOrder)
            );
        CreateMap<ArticleGateway.Contracts.v1.Article.Nutrient, ProductCatalogNutrientDto>();
        CreateMap<Designation, ProductCatalogDesignationDto>();
        CreateMap<WeightComponent, ProductCatalogWeightComponentDto>();
        CreateMap<Component, ProductCatalogComponentDto>();
        CreateMap<PriceClass, ProductCatalogPriceClassDto>();

        CreateMap<Price, ProductCatalogPriceDto>();
        CreateMap<IngredientSummary, ProductCatalogIngredientSummaryDto>();
        CreateMap<Component, ProductCatalogComponentDto>();
        CreateMap<WeightComponents, ProductCatalogWeightComponentsDto>();
        CreateMap<WeightRangeList, ProductCatalogWeightRangeDto>();
        CreateMap<WeightInUnit, ProductCatalogWeightInUnitDto>();
        CreateMap<WeightComponentListSalesUnit,
            ProductCatalogWeightComponentListSalesUnitDto>();
        CreateMap<WeightComponentList, ProductCatalogWeightComponentDto>();
        //CreateMap<DietList, NutriScorePhotoDto>().ConvertUsing<NutriScoreConverter>();
        CreateMap<Unit, ProductCatalogUnitDto>();
    }

    private void MapCacheObjects()
    {
        CreateMap<ProductCatalogArticleDetailsCacheDto, ProductCatalogArticleDetailsDto>()
            .ForMember(dest
                => dest.NutriScorePhoto, opts
                => opts.MapFrom(src => src.NutriScorePhotoPath));

        CreateMap<string, NutriScorePhotoDto>().ForMember(a => a.Path, opts => opts.MapFrom(a => a));

        CreateMap<ProductCatalogArticleCacheDto, ProductCatalogArticleDto>()
            .ForMember(
                dest
                    => dest.Image, opts
                    => opts.MapFrom(src => src.Id)
                    
            )
            .ForMember(dest => dest.ImagePaths, opt 
                => opt.MapFrom(f => f.ImageSet))
            ;

        CreateMap<ProductCatalogImageSetCacheDto, ProductCatalogImagePathsDto>().ConvertUsing<CatalogProductImageSetConverter>();
        CreateMap<ProductCatalogAdditiveCacheDto, ProductCatalogAdditiveDto>();
        CreateMap<ProductCatalogCategoryCacheDto, ProductCatalogCategoryDto>();
        CreateMap<ProductCatalogAllergenCacheDto, ProductCatalogAllergenDto>();
        CreateMap<string, ProductCatalogImageDto>().ConvertUsing<ProductCatalogImageConverter>();
        CreateMap<ProductCatalogDietCacheDto, ProductCatalogDietDto>();
        CreateMap<ProductCatalogComponentCacheDto, ProductCatalogComponentDto>();
        CreateMap<ProductCatalogTaxanomyCacheDto, ProductCatalogTaxanomyDto>();
        CreateMap<ProductCatalogContentWeightSalesUnitCacheDto, ProductCatalogContentWeightSalesUnitDto>();
        CreateMap<ProductCatalogWeightInUnitCacheDto, ProductCatalogWeightInUnitDto>();
        CreateMap<ProductCatalogSortimentEvecatcherCacheDto, ProductCatalogSortimentEvecatcherDto>();
        CreateMap<ProductCatalogSuggestedPortionCacheDto, ProductCatalogSuggestedPortionDto>();
        CreateMap<ProductCatalogOutletArticleTypeCacheDto, ProductCatalogOutletArticleTypeDto>();
        CreateMap<ProductCatalogPromotionalCodeCacheDto, ProductCatalogPromotionalCodeDto>();
        CreateMap<ConversionUnitListCacheDto, ConversionUnitListDto>();
        CreateMap<ProductCatalogUnitCacheDto, ProductCatalogUnitDto>();
        CreateMap<ProductCatalogAmountInUnitCacheDto, ProductCatalogAmountInUnitDto>();

        CreateMap<ProductCatalogLogisticCacheDto, ProductCatalogLogisticDto>();

        CreateMap<ProductCatalogSortimentAssignmentCacheDto, ProductCatalogSortimentAssignmentDto>();
        
        CreateMap<ProductCatalogSortimentCacheDto, ProductCatalogSortimentDto>();
        CreateMap<ProductCatalogValidityPeriodCacheDto, ProductCatalogValidityPeriodDto>();
        CreateMap<ProductCatalogSealCacheDto, ProductCatalogSealDto>();

        CreateMap<ProductCatalogPreparationFeatureCacheDto, ProductCatalogPreparationFeatureDto>();
        CreateMap<ProductCatalogPreparationInstructionCacheDto, ProductCatalogPreparationInstructionDto>();
        
        CreateMap<ProductCatalogNutrientCacheDto, ProductCatalogNutrientDto>();
        CreateMap<ProductCatalogDesignationCacheDto, ProductCatalogDesignationDto>();
        CreateMap<ProductCatalogWeightComponentCacheDto, ProductCatalogWeightComponentDto>();
        CreateMap<ProductCatalogComponentCacheDto, ProductCatalogComponentDto>();
        CreateMap<ProductCatalogPriceClassCacheDto, ProductCatalogPriceClassDto>();
        
        CreateMap<ProductCatalogPriceCacheDto, ProductCatalogPriceDto>();
        CreateMap<ProductCatalogIngredientSummaryCacheDto, ProductCatalogIngredientSummaryDto>()
            .ForMember(a => a.IngredientContainsText, opts => opts.MapFrom(
                
                p 
                => string.IsNullOrWhiteSpace(p.IngredientText)? "" : 
                    GetSecondValueFromSplit(p.IngredientText,ContainsKeyword)))
            .ForMember(a => a.IngredientWithoutContainsText, opts => opts.MapFrom(
                
                p 
                    => string.IsNullOrWhiteSpace(p.IngredientText)? "" : 
                        GetFirstValueFromSplit(p.IngredientText,ContainsKeyword)))
                ;
        CreateMap<ProductCatalogComponentCacheDto, ProductCatalogComponentDto>();
        CreateMap<ProductCatalogWeightComponentsCacheDto, ProductCatalogWeightComponentsDto>();
        CreateMap<ProductCatalogWeightRangeCacheDto, ProductCatalogWeightRangeDto>();
        CreateMap<ProductCatalogWeightInUnitCacheDto, ProductCatalogWeightInUnitDto>();
        CreateMap<ProductCatalogWeightComponentListSalesUnitCacheDto,
            ProductCatalogWeightComponentListSalesUnitDto>();
        CreateMap<ProductCatalogWeightComponentCacheDto, ProductCatalogWeightComponentDto>();
        CreateMap<IList<ProductCatalogDietCacheDto>, NutriScorePhotoDto>().ConvertUsing<NutriScoreFromCacheConverter>();
        CreateMap<Unit, ProductCatalogUnitDto>();
        
        
        CreateMap<FilterSetCacheDto, FilterSetDto>();
        CreateMap<FilterBlockCacheDto, FilterBlockDto>();
        CreateMap<FilterEntryCacheDto, FilterEntryDto>();
    }

    private static string GetSecondValueFromSplit(string value, string separator) =>
        value.Split(separator).Length == 2 ? $"{separator}{value.Split(separator)[1]}" : "";
    private static string GetFirstValueFromSplit(string value, string separator) =>
        value.Split(separator).Length >= 1 ? $"{value.Split(separator)[0]}" : "";
    private const string PriceCode = "TK";
    private const string ContainsKeyword = "Enthält:";
}