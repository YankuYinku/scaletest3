namespace apetito.meinapetito.Portal.Application.ProductCatalog.Exceptions;

public class ProductSortimentWhileQueryingMaterials : IncorrectSortimentException
{
    private const string Message = "While querying materials, sortiments for products is not allowed";
    public ProductSortimentWhileQueryingMaterials(string incorrectSortiment) 
        : base(incorrectSortiment,Message)
    {
    }
}