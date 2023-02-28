using apetito.CQS;
using apetito.meinapetito.Portal.Contracts.Root.Users.AssignedUsers;
using apetito.meinapetito.Portal.Data.Root;
using apetito.meinapetito.Portal.Data.Root.MainCustomerAdmins.CustomerNumbers;
using Microsoft.EntityFrameworkCore;

namespace apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Queries.Handlers;

public class RetrieveAssignedUsersFromDbQueryHandler : IQueryHandler<RetrieveAssignedUsersFromDb,IList<AssignedUserDto>>
{
    private readonly PortalDbContext _portalDbContext;

    public RetrieveAssignedUsersFromDbQueryHandler(PortalDbContext portalDbContext)
    {
        _portalDbContext = portalDbContext;
    }

    public async Task<IList<AssignedUserDto>> Execute(RetrieveAssignedUsersFromDb query)
    {
        var entries = await _portalDbContext.PortalUserCustomerNumbersEntries
            .Where(a => query.CustomerNumbers.Contains(a.CustomerNumber)).ToListAsync();

        IList<Guid> userIdCandidated = entries.Select(a => a.PortalUserReferenceId).ToList();

        var userNames = await _portalDbContext.PortalUsers.Where(a => userIdCandidated.Any(id => a.PortalUserId == id))
            .ToListAsync();
        
        var userNumbers = await _portalDbContext.PortalUserCustomerNumbers.Include(a => a.CustomerNumbers)
            .Where(a => userIdCandidated.Any(id =>a.PortalUserReferenceId == id)).ToListAsync();

        var dictionary =
            new Dictionary<Guid, PortalUserCustomerNumbersData>();

        foreach (var item in userNumbers)
        {
            dictionary.Add(item.PortalUserReferenceId,item);
        }

        var resultList = new List<AssignedUserDto>();
        foreach (var userName in userNames)
        {
            if (!dictionary.TryGetValue(userName.PortalUserId, out var userCustomerNumbers))
            {
                continue;
            }

            var assignedUser = new AssignedUserDto
            {
                Email = userName.Email,
                Id = userName.PortalUserId.ToString(),
                FirstName = userName.FirstName,
                LastName = userName.LastName,
                CustomerNumbers = userCustomerNumbers.CustomerNumbers.Select(a =>  new CustomerNumberWithRoleDto()
                {
                    Role = a.Role,
                    CustomerNumber = a.CustomerNumber
                }).ToList(),
                Status = userName.Status,
                IsSap = false
            };
            
            resultList.Add(assignedUser);
        }
        
        return resultList;
    }
}