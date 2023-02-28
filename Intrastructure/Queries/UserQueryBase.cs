namespace apetito.meinapetito.Portal.Application.Root.Intrastructure.Queries;

public abstract class UserQueryBase
{
    public string UserEmail { get; set; } = string.Empty;
    public ICollection<int> CustomerNumbers { get; set; } = new List<int>();
}