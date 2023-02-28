using apetito.ArticleGateway.Contracts.v1.Article;
using apetito.ArticleGateway.Contracts.v1.Querying;
using apetito.iProDa3.Contracts.ApiClient.RequestModels;
using apetito.iProDa3.Contracts.ApiClient.V1;
using apetito.meinapetito.Webhooks.ArticleChanges.Application.Services.Interface;
using Newtonsoft.Json;

namespace apetito.meinapetito.Webhooks.ArticleChanges.Application.Services.Implementation
{
    public class ApetitoArticlesAllergeneRelevantChangeDetector : IApetitoArticlesAllergeneRelevantChangeDetector
    {
        private const int PageSize = 100;
        private readonly IiProDa3ArticlesV1RestClient _apetitoProductCatalogIProdaApi;

        public ApetitoArticlesAllergeneRelevantChangeDetector(IiProDa3ArticlesV1RestClient apetitoProductCatalogIProdaApi)
        {
            _apetitoProductCatalogIProdaApi = apetitoProductCatalogIProdaApi;
        }

        public async Task<IList<string>> GetChangedProductsIdentifiersAsync(DateTime lastCheckDate)
        {
            ArticleQueryResult? articleQueryResult = null;
            var changedArticles = new List<string>();
            var currentPage = 1;

            do 
            {
                articleQueryResult = await LoadArticlePageAsync(currentPage);
                var changedArticlesOnPage = DetectAllergeneRelevantChangesSince(articleQueryResult, lastCheckDate);
                changedArticles.AddRange(changedArticlesOnPage);
                currentPage++;
            } while (currentPage <= articleQueryResult.OverallPages);

            return changedArticles;
        }

        private static IEnumerable<string> DetectAllergeneRelevantChangesSince(ArticleQueryResult currentArticleQueryResult, DateTime lastCheckDate)
        {
            foreach (Article? article in currentArticleQueryResult.Items)
            {
                if (article is null)
                    continue;
             
                var articleDetails = article.Details?.Allergens?.Details;

                if (articleDetails is null)
                    continue;

                var hasAllergeneRelevantChange = articleDetails["RelevantChange"].Equals(true);
                var changedDateAsString = articleDetails.TryGetValue("LastRelevantChangeDate", out var item)
                    ? item?.ToString()
                    : string.Empty;
                var hasChangeDate = DateTime.TryParse(changedDateAsString, out var changedDate);
                var isChangedDatePastLastCheckDate = changedDate >= lastCheckDate;

                if (hasAllergeneRelevantChange && hasChangeDate && isChangedDatePastLastCheckDate)
                    yield return article.ArticleNumber;
            }
        }

        private async Task<ArticleQueryResult> LoadArticlePageAsync(int page)
        {
            var query = new RetrieveArticlesQuery
            {
                Page = page,
                PageSize = PageSize
            };

            return await _apetitoProductCatalogIProdaApi.RetrieveArticlesAsync<ArticleQueryResult>(query);
        }
    }
}