namespace apetito.meinapetito.Portal.Application.ProductCatalog.Exceptions;

public class MaterialSortimentWhileQueryingProducts : IncorrectSortimentException
{
    private const string Message = "While querying products, sortiments for materials is not allowed";
    public MaterialSortimentWhileQueryingProducts(string incorrectSortiment) 
        : base(incorrectSortiment,Message)
    {
    }
}