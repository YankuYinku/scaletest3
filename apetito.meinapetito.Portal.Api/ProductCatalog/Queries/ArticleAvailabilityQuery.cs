using apetito.CQS;
using apetito.meinapetito.Portal.Application.ProductCatalog.Queries;
using apetito.meinapetito.Portal.Contracts.ProductCatalog.Models.Availability;
using HotChocolate;
using HotChocolate.Types;

namespace apetito.meinapetito.Portal.Api.ProductCatalog.Queries;

[ExtendObjectType("Query")]
public class ArticleAvailabilityQuery
{
    public async Task<AvailabilityCheckResultDto> CheckArticlesAvailabilityAsync(IList<ArticleWithQuantityDto> articles,
        [Service] IQueryHandler<RetrieveArticlesAvailabilityQuery,AvailabilityCheckResultDto> availabilityQueryHandler)
        {
            return await availabilityQueryHandler.Execute(new RetrieveArticlesAvailabilityQuery()
            {
                ArticlesWithQuantities = articles
            });
        }
}