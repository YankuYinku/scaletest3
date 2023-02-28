using apetito.CQS;
using apetito.meinapetito.Portal.Contracts.Root.Users.Customers;

namespace apetito.meinapetito.Portal.Application.Root.Users.Current.Customers.Queries;
public class RetrieveCustomersOfUserFromSAPQuery : IQuery<CustomersOfUserDto>
{
    public string UserEmail { get; set; }
}