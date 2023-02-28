using apetito.CQS;
using apetito.meinapetito.Portal.Contracts.Root.Users.AssignedUsers;
using apetito.meinapetito.Portal.Contracts.Root.Users.Current;
using apetito.meinapetito.Portal.Contracts.Root.Users.Sortiments;
using apetito.meinapetito.Portal.Data.Root;
using Microsoft.EntityFrameworkCore;

namespace apetito.meinapetito.Portal.Application.Root.Users.Current;

public class RetrieveUserByIdQueryHandler : IQueryHandler<RetrieveUserById,IList<CustomerDto>>
{
    private readonly PortalDbContext _portalDbContext;

    public RetrieveUserByIdQueryHandler(PortalDbContext portalDbContext)
    {
        _portalDbContext = portalDbContext;
    }

    public async Task<IList<CustomerDto>> Execute(RetrieveUserById query)
    {
        var user = await _portalDbContext.PortalUsers.FirstOrDefaultAsync(a => a.PortalUserId == query.Id);

        IList<CustomerDto> result = new List<CustomerDto>();

        if (user is null)
        {
            return result;
        }
        
        var customerNumbers =
            await _portalDbContext.PortalUserCustomerNumbersEntries.Where(a =>
                a.PortalUserReferenceId == user.PortalUserId).ToListAsync();

        foreach (var customerNumber in customerNumbers)
        {
            var sortiments = await _portalDbContext.PortalUserCustomerNumberEntrySortimentsEntries.Where(a =>
                a.PortalUserReferenceId == user.PortalUserId && a.PortalUserCustomerNumbersEntryReferenceId ==
                customerNumber.PortalUserCustomerNumbersEntryId ).ToListAsync();
            result.Add(new CustomerDto
            {
                Role = customerNumber.Role,
                ContactPortal = string.Empty,
                CustomerNumber = customerNumber.CustomerNumber,
                LanguageCode = customerNumber.LanguageCode,
                OrderSystem = customerNumber.OrderSystem,
                Sortiments = sortiments.Select(a =>new SortimentDto()
                {
                    Code = a.SortimentCode
                }).ToList()
            });
        }
        
        return result;
    }
}