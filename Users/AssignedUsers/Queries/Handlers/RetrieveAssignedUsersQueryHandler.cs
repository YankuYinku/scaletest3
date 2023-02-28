using apetito.CQS;
using apetito.meinapetito.Portal.Contracts.Root.Users.AssignedUsers;

namespace apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Queries.Handlers;

public class RetrieveAssignedUsersQueryHandler : IQueryHandler<RetrieveAssignedUsers, IList<AssignedUserDto>>
{
    private readonly IQueryHandler<RetrieveAssignedUsersFromSAP, IList<AssignedUserDto>> _handler;
    private readonly IQueryHandler<RetrieveAssignedUsersFromDb, IList<AssignedUserDto>> _handlerFromDb;

    public RetrieveAssignedUsersQueryHandler(
        IQueryHandler<RetrieveAssignedUsersFromSAP, IList<AssignedUserDto>> handler,
        IQueryHandler<RetrieveAssignedUsersFromDb, IList<AssignedUserDto>> handlerFromDb)
    {
        _handler = handler;
        _handlerFromDb = handlerFromDb;
    }

    public async Task<IList<AssignedUserDto>> Execute(RetrieveAssignedUsers query)
    {
        var sapUsersTask = GetSapUsersTask(query);
        var itemsTask = GetPortalUsersTask(query);

        var sapUsers = await sapUsersTask;
        var items = await itemsTask;

        var assignedUsers = MergeResults(sapUsers, items);
        
        return assignedUsers.OrderBy(a => a.IsSap)
            .ThenBy(b => b.LastName)
            .ThenBy(a => a.FirstName).ToList();
    }

    private static List<AssignedUserDto> MergeResults(IList<AssignedUserDto> sapUsers,
        IList<AssignedUserDto> items)
    {
        var assignedUsers = sapUsers.ToDictionary(a => a.Email);

        foreach (var item in items)
        {
            if (assignedUsers.TryGetValue(item.Email, out var user))
            {
                foreach (var currentCustomerNumber in item.CustomerNumbers)
                {
                    if (!user.CustomerNumbers.Contains(currentCustomerNumber))
                    {
                        user.CustomerNumbers.Add(currentCustomerNumber);
                    }
                }

                continue;
            }

            assignedUsers.Add(item.Email, item);
        }

        return assignedUsers.Values.ToList();
    }

    private Task<IList<AssignedUserDto>> GetPortalUsersTask(RetrieveAssignedUsers query)
        => _handlerFromDb.Execute(new RetrieveAssignedUsersFromDb()
        {
            Email = query.Email,
            CustomerNumbers = query.CustomerNumbers
        });

    private Task<IList<AssignedUserDto>> GetSapUsersTask(RetrieveAssignedUsers query)
        => _handler.Execute(new RetrieveAssignedUsersFromSAP()
        {
            Email = query.Email,
            CustomerNumbers = query.CustomerNumbers
        });
}