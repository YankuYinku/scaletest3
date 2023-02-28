using apetito.meinapetito.Portal.Contracts.ProductCatalog;

namespace apetito.meinapetito.Portal.Api.ProductCatalog.Queries;

public abstract class ExpectedItemsBasedQuery
{
    protected static ArticleTypeRequestEnum GetTypeByParameter(string? sortimentType)
        => sortimentType switch
        {
            "Food" => ArticleTypeRequestEnum.Product,
            "PromotionalMaterial" => ArticleTypeRequestEnum.Material,
            _ => throw new ArgumentOutOfRangeException()
        };
}