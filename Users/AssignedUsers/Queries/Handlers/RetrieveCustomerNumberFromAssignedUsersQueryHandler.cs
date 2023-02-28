using apetito.CQS;
using apetito.meinapetito.Portal.Contracts.Root.Users.Current;
using apetito.meinapetito.Portal.Data.Root;
using Microsoft.EntityFrameworkCore;

namespace apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Queries.Handlers;

public class RetrieveCustomerNumberFromAssignedUsersQueryHandler : IQueryHandler<RetrieveCustomerNumberFromAssignedUsers,IList<CustomerDto>>
{
    private readonly PortalDbContext _portalDbContext;

    public RetrieveCustomerNumberFromAssignedUsersQueryHandler(PortalDbContext portalDbContext)
    {
        _portalDbContext = portalDbContext;
    }

    public async Task<IList<CustomerDto>> Execute(RetrieveCustomerNumberFromAssignedUsers query)
    {
        var items = await _portalDbContext.PortalUsers.Where(a => a.Email == query.Email).FirstOrDefaultAsync();

        if (items is null)
        {
            return new List<CustomerDto>();
        }

        return new List<CustomerDto>();
    }
}