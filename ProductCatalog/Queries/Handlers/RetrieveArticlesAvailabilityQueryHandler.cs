using apetito.ArticleGateway.Contracts.ArticleAvailability;
using apetito.CQS;
using apetito.meinapetito.Portal.Contracts.ProductCatalog.Models.Availability;

namespace apetito.meinapetito.Portal.Application.ProductCatalog.Queries.Handlers;

public class RetrieveArticlesAvailabilityQueryHandler : IQueryHandler<RetrieveArticlesAvailabilityQuery,AvailabilityCheckResultDto>
{
    private readonly IArticleAvailabilityRestClient _articleAvailabilityRestClient;

    public RetrieveArticlesAvailabilityQueryHandler(IArticleAvailabilityRestClient articleAvailabilityRestClient)
    {
        _articleAvailabilityRestClient = articleAvailabilityRestClient;
    }

    public async Task<AvailabilityCheckResultDto> Execute(RetrieveArticlesAvailabilityQuery query)
    {
        var availabilityCheckRequestId = await _articleAvailabilityRestClient.CreateAvailabilityRequestKey(new ArticleAvailabilityCheckRequest()
        {
            CheckMethods = new List<CheckMethods>
            {
                0
            },
            ArticleToCheck = query.ArticlesWithQuantities.Select(a => new ArticleToCheckModel
            {
                ArticleNumber = a.ArticleNumber,
                RequiredQuantity = a.Quantity,
                CheckDate = DateTime.Now
            }).ToList()
        });

        var availabilityCheckResult = await GetCheckingResult(availabilityCheckRequestId);

        return new AvailabilityCheckResultDto
        {
            Available = GetActiveArticles(availabilityCheckResult),
            NotAvailable = GetNotActiveArticles(availabilityCheckResult)
        };
    }

    private static IList<ArticleWithAllowedQuantityDto> GetActiveArticles(ArticleAvailabilityCheckResponse response) 
        => GetArticles(response, a => a.IsAvailable);
    private static IList<ArticleWithAllowedQuantityDto> GetNotActiveArticles(ArticleAvailabilityCheckResponse response) 
        => GetArticles(response, a => !a.IsAvailable);
    
    private static IList<ArticleWithAllowedQuantityDto> GetArticles(ArticleAvailabilityCheckResponse response,
        Func<ArticleAvailabilityCheckResult, bool> function)
        => response.ArticleAvailabilityCheckResults.Where(function).Select(a => new ArticleWithAllowedQuantityDto()
        {
            AllowedQuantity = a.AvailableQuantity,
            ArticleNumber = a.ArticleNumber
        }).ToList();

        private async Task<ArticleAvailabilityCheckResponse> GetCheckingResult(Guid id) => await _articleAvailabilityRestClient.GetAvailabilityByRequestKey(id);
}