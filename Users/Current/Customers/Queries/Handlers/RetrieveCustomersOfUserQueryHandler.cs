using apetito.CQS;
using apetito.Customers.Contracts.CustomersOfUser.Models;
using apetito.meinapetito.Portal.Contracts.Root.Users.Customers;
using CustomersOfUserDto = apetito.meinapetito.Portal.Contracts.Root.Users.Customers.CustomersOfUserDto;
using SortimentDto = apetito.meinapetito.Portal.Contracts.Root.Users.Sortiments.SortimentDto;

namespace apetito.meinapetito.Portal.Application.Root.Users.Current.Customers.Queries.Handlers;

public class RetrieveCustomersOfUserQueryHandler : IQueryHandler<RetrieveCustomersOfUserQuery, CustomersOfUserDto>
{
    private readonly IQueryHandler<RetrieveCustomersOfUserFromSAPQuery, CustomersOfUserDto>
        _customersOfUserFromSapQuery;

    private readonly IQueryHandler<RetrieveCustomersOfUserFromPortalDbQuery, CustomersOfUserDto>
        _customersOfUserFromPortalDQuery;

    public RetrieveCustomersOfUserQueryHandler(
        IQueryHandler<RetrieveCustomersOfUserFromSAPQuery, CustomersOfUserDto> customersOfUserFromSapQuery,
        IQueryHandler<RetrieveCustomersOfUserFromPortalDbQuery, CustomersOfUserDto> customersOfUserFromPortalDbQuery
    )
    {
        _customersOfUserFromSapQuery = customersOfUserFromSapQuery;
        _customersOfUserFromPortalDQuery = customersOfUserFromPortalDbQuery;
    }

    public async Task<CustomersOfUserDto> Execute(RetrieveCustomersOfUserQuery query)
    {
        var customersOfUser = 
             await _customersOfUserFromSapQuery.Execute(new RetrieveCustomersOfUserFromSAPQuery()
             { UserEmail = query.UserEmail });

             var customerObjectFromPortalDb = await _customersOfUserFromPortalDQuery.Execute(
            new RetrieveCustomersOfUserFromPortalDbQuery()
            {
                Email = query.UserEmail
            });

        var mergedCustomers = MergeCustomers(customersOfUser.Customers, customerObjectFromPortalDb.Customers);
        var mergedRoles = MergeRoles(customersOfUser.Roles, customerObjectFromPortalDb.Roles);

        if (mergedRoles.Contains(ForceMenuPlannerRole))
        {
            foreach (var mergedCustomer in mergedCustomers.Where(mergedCustomer => !mergedCustomer.EffectiveOrderSystems.Contains(MenuPlannerOrderSystem)))
            {
                mergedCustomer.EffectiveOrderSystems.Add(MenuPlannerOrderSystem);
            }
        }
        
        return new CustomersOfUserDto(query.UserEmail)
        {
            Customers = mergedCustomers,
            Roles = mergedRoles,
        };
    }

    private List<string> MergeRoles(IEnumerable<string> sapList, IEnumerable<string> dbList)
    {
        return sapList.Concat(dbList).Distinct().ToList();
    }

    private static List<CustomerDto> MergeCustomers(IEnumerable<CustomerDto> sapList, IEnumerable<CustomerDto> dbList)
    {
        var dictionary = sapList.DistinctBy(a => a.CustomerNumber).ToDictionary(a => a.CustomerNumber);

        foreach (CustomerDto customerDto in dbList)
        {
            if (dictionary.ContainsKey(customerDto.CustomerNumber))
            {
                continue;
            }

            dictionary.Add(customerDto.CustomerNumber, customerDto);
        }

        return dictionary.Values.ToList();
    }

    private const string ForceMenuPlannerRole = "forcemenuplanner";
    private const string MenuPlannerOrderSystem = "MPL";
}