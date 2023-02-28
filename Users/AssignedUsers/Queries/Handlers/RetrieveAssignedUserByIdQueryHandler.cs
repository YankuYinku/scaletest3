using apetito.CQS;
using apetito.meinapetito.Portal.Contracts.Root.Users.AssignedUsers;
using apetito.meinapetito.Portal.Data.Root;
using Microsoft.EntityFrameworkCore;

namespace apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Queries.Handlers;

public class RetrieveAssignedUserByIdQueryHandler : IQueryHandler<RetrieveAssignedUserById, AssignedUserDto?>
{
    private readonly PortalDbContext _portalDbContext;

    public RetrieveAssignedUserByIdQueryHandler(PortalDbContext portalDbContext)
    {
        _portalDbContext = portalDbContext;
    }

    public async Task<AssignedUserDto?> Execute(RetrieveAssignedUserById query)
    {
        var entries = await _portalDbContext.PortalUserCustomerNumbersEntries
            .Where(a => a.PortalUserReferenceId == query.Id).ToListAsync();

        IList<Guid> userIdCandidated = entries.Select(a => a.PortalUserReferenceId).ToList();

        var userName =
            await _portalDbContext.PortalUsers.FirstOrDefaultAsync(
                a => a.PortalUserId == query.Id);

        var userNumbers = await _portalDbContext.PortalUserCustomerNumbersEntries
            .Where(a => userIdCandidated.Any(id => a.PortalUserReferenceId == id)).ToListAsync();
        
        var assignedUser = new AssignedUserDto
        {
            Email = userName.Email,
            Id = userName.PortalUserId.ToString(),
            FirstName = userName.FirstName,
            LastName = userName.LastName,
            Status = userName.Status,
            IsSap = false,
            CustomerNumbers = userNumbers.Select(a => new CustomerNumberWithRoleDto
            {
                Role = a.Role,
                CustomerNumber = a.CustomerNumber,
                LanguageCode = a.LanguageCode
            }).ToList()
        };

        return assignedUser;
    }
}