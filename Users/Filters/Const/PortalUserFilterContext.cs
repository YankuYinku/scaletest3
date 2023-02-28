namespace apetito.meinapetito.Portal.Application.Root.Users.Filters.Const;

public class PortalUserFilterContext
{
    private const string Foods = "Food";
    private const string Materials = "PromotionalMaterial";
    private const string Faqs = "Faq";
    private const string Downloads = "Download";
    
    private readonly IList<string> _possibleValues = new List<string>
    {
        Foods,
        Materials,
        Faqs,
        Downloads
    };

    private readonly string _value;

    public PortalUserFilterContext(string value)
    {
        if (!_possibleValues.Contains(value))
        {
            throw new Exception("Invalid value for Context");
        }
        _value = value;
    }

    public override string ToString()
    {
        return _value;
    }
}