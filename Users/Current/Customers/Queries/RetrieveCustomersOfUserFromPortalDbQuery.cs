using apetito.CQS;
using apetito.meinapetito.Portal.Contracts.Root.Users.Customers;

namespace apetito.meinapetito.Portal.Application.Root.Users.Current.Customers.Queries;

public class RetrieveCustomersOfUserFromPortalDbQuery : IQuery<CustomersOfUserDto>
{
    public string Email { get; set; } = string.Empty;
}