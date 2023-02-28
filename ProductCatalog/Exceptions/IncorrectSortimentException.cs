namespace apetito.meinapetito.Portal.Application.ProductCatalog.Exceptions;

public abstract class IncorrectSortimentException : Exception
{
    public string IncorrectSortiment { get; }
    public string Reason { get; }

    protected IncorrectSortimentException(string sortiment, string reason)
        : base($"Sortiment \"{sortiment}\" was incorrect. Reason: {reason}")
    {
        IncorrectSortiment = sortiment;
        Reason = reason;
    }
}