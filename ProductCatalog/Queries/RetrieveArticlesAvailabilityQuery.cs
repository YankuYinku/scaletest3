using apetito.CQS;
using apetito.meinapetito.Portal.Contracts.ProductCatalog.Models.Availability;

namespace apetito.meinapetito.Portal.Application.ProductCatalog.Queries;

public class RetrieveArticlesAvailabilityQuery : IQuery<AvailabilityCheckResultDto>
{
    public IList<ArticleWithQuantityDto> ArticlesWithQuantities { get; set; }
}