using apetito.CQS;
using apetito.Customers.Contracts.CustomersOfUser.ApiClients;
using apetito.Customers.Contracts.CustomersOfUser.ApiClients.RequestModels;
using apetito.meinapetito.Portal.Application.Root.Users.Claims;
using apetito.meinapetito.Portal.Application.Root.Users.Current.Customers.Queries;
using apetito.meinapetito.Portal.Contracts.Root.Users.AssignedUsers;
using apetito.meinapetito.Portal.Contracts.Root.Users.Customers;

namespace apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Queries.Handlers;

public class RetrieveAssignedUsersFromSapQueryHandler : IQueryHandler<RetrieveAssignedUsersFromSAP, IList<AssignedUserDto>>
{
    private readonly ICustomerUsersServiceRestClient _customerUsersServiceRestClient;
    private readonly IQueryHandler<RetrieveCustomersOfUserFromSAPQuery, CustomersOfUserDto> _customersOfUserQuery;
    public RetrieveAssignedUsersFromSapQueryHandler(ICustomerUsersServiceRestClient customerUsersServiceRestClient, IQueryHandler<RetrieveCustomersOfUserFromSAPQuery, CustomersOfUserDto> customersOfUserQuery)
    {
        _customerUsersServiceRestClient = customerUsersServiceRestClient;
        _customersOfUserQuery = customersOfUserQuery;
    }

    public async Task<IList<AssignedUserDto>> Execute(RetrieveAssignedUsersFromSAP query)
    {
        var customerNumbers = await _customersOfUserQuery.Execute(new RetrieveCustomersOfUserFromSAPQuery()
        {
            UserEmail = query.Email
        });
        
        var allAvailableCustomerNumbers = customerNumbers.Customers.Select(a => a.CustomerNumber).ToList();

        if (!allAvailableCustomerNumbers.Any())
        {
            return new List<AssignedUserDto>();
        }
        
        var fromSap = await _customerUsersServiceRestClient.RetrieveCustomerDataOfUserAsync(
            new RetrieveCustomerUsersQuery()
            {
                CustomerNumbers = allAvailableCustomerNumbers,
                Email = query.Email
            });

        var assignedUserDtos = new Dictionary<string, AssignedUserDto>();

        foreach (var fromSapItem in fromSap)
        {
            var currentRole = UserClaimFactory
                    .DeriveClaimsFromContactPortal(fromSapItem.ContactPortal)
                    .GetPortalUserRoleClaim().Value;
            
            if (assignedUserDtos.ContainsKey(fromSapItem.Email))
            {
                assignedUserDtos[fromSapItem.Email].CustomerNumbers?.Add(new CustomerNumberWithRoleDto()
                {
                    Role = currentRole,
                    CustomerNumber = fromSapItem.CustomerNumber
                });
                continue;
            }

            assignedUserDtos.Add(fromSapItem.Email, new AssignedUserDto
            {
                Email = fromSapItem.Email,
                Id = Guid.NewGuid().ToString(),
                FirstName = fromSapItem.FirstName,
                LastName = fromSapItem.LastName,
                CustomerNumbers = new List<CustomerNumberWithRoleDto>()
                {
                    new ()
                    {
                        Role = currentRole,
                        CustomerNumber = fromSapItem.CustomerNumber
                    }
                },
                Status = "Active",
                IsSap = true
            });
        }

        return assignedUserDtos.Values.Where(a => a.CustomerNumbers.Any(z => query.CustomerNumbers.Contains(z.CustomerNumber))).ToList();
    }

   
}