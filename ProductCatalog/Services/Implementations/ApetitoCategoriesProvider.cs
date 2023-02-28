using apetito.ArticleGateway.Contracts.ApiClient.V0.Categories;
using apetito.iProDa3.Contracts;
using apetito.iProDa3.Contracts.Models.Sortiments;
using apetito.meinapetito.Portal.Contracts.ProductCatalog.Models;
using AutoMapper;
using apetito.meinapetito.Portal.Application.ProductCatalog.Services.Interfaces;
using apetito.meinapetito.Portal.Contracts.ProductCatalog;

namespace apetito.meinapetito.Portal.Application.ProductCatalog.Services.Implementations
{
    public class ApetitoCategoriesProvider : IApetitoCategoriesProvider
    {
        private readonly IArticleGatewayCategoriesRestApi _api;
        private readonly IIProDa3SortimentRestClient _iiProDa3Sortiment;
        private readonly IMapper _mapper;
        private readonly ISortimentsProvider _sortimentsProvider;
        private readonly ILogger<ApetitoCategoriesProvider> _logger;

        public ApetitoCategoriesProvider(IArticleGatewayCategoriesRestApi api, IMapper mapper,
            ISortimentsProvider sortimentsProvider, IIProDa3SortimentRestClient iiProDa3Sortiment,
            ILogger<ApetitoCategoriesProvider> logger)
        {
            _api = api;
            _mapper = mapper;
            _sortimentsProvider = sortimentsProvider;
            _iiProDa3Sortiment = iiProDa3Sortiment;
            _logger = logger;
        }

        public async Task<IQueryable<ProductCatalogCategoryDto>> GetCategoriesAsync(
            ArticleTypeRequestEnum expectedArticleType)
            => (expectedArticleType switch
            {
                ArticleTypeRequestEnum.Product => await GetCategoriesForFoodsAsync(),
                ArticleTypeRequestEnum.Material => await GetCategoriesForMaterialsAsync(),
                _ => throw new ArgumentOutOfRangeException(nameof(expectedArticleType), expectedArticleType, null)
            });

        private async Task<IQueryable<ProductCatalogCategoryDto>> GetCategoriesForFoodsAsync()
        {
            try
            {
                // TODO: use IArticleGatewayCategoriesRestApi to get sortiments
                var result = await _api.RetrieveCategoriesAsync();

                var queryable = result.Select(z => _mapper.Map<ProductCatalogCategoryDto>(z)).AsQueryable();

                return queryable;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, ex?.Message);

                return (new List<ProductCatalogCategoryDto>()).AsQueryable();
            }
        }

        private async Task<IQueryable<ProductCatalogCategoryDto>> GetCategoriesForMaterialsAsync()
        {
            try
            {
                var sortiments = await _sortimentsProvider.RetrieveAsync();


                var sortimentsQueryResult = await _iiProDa3Sortiment.GetSortimentsQuery(new SortimentQuery()
                {
                    SerializeCategories = true,
                    SortimentCodes = sortiments.Select(a => a.Code).Where(a => a.StartsWith("WM")).ToList()
                });


                var dictionary =
                    new Dictionary<string, ProductCatalogCategoryDto>();

                foreach (var sortimentQueryResult in sortimentsQueryResult)
                {
                    foreach (var category in sortimentQueryResult.Kategorien)
                    {
                        if (dictionary.ContainsKey(category.Kategorie))
                        {
                            continue;
                        }

                        var description = category.Details.FirstOrDefault(a => a.Sprachcode == GermanCode)?.Text ??
                                          category.Kategorie;

                        dictionary.Add(category.Kategorie, new ProductCatalogCategoryDto()
                        {
                            Code = category.Kategorie,
                            Description = description,
                            Name = description
                        });
                    }
                }

                return dictionary.Values.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, ex?.Message);

                return (new List<ProductCatalogCategoryDto>()).AsQueryable();
            }
        }

        private const string GermanCode = "D";
    }
}