using apetito.CQS;
using apetito.meinapetito.Portal.Data.Root;
using apetito.meinapetito.Portal.Data.Root.MainCustomerAdmins.CustomerNumbers.Sortiments;
using Microsoft.EntityFrameworkCore;

namespace apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Commands.Handlers;

public class CreateUserCustomerNumbersSortimentsCommandHandler : ICommandHandler<CreateUserCustomerNumbersSortiments>
{
    private readonly PortalDbContext _portalDbContext;

    public CreateUserCustomerNumbersSortimentsCommandHandler(PortalDbContext portalDbContext)
    {
        _portalDbContext = portalDbContext;
    }

    public async Task Handle(CreateUserCustomerNumbersSortiments command)
    {
        var sortiments = await _portalDbContext.PortalUserCustomerNumberEntrySortiments.Include(a => a.Sortiments).FirstOrDefaultAsync(a 
            => a.PortalUserReferenceId == command.PortalUserReferenceId && a.PortalUserCustomerNumbersEntryReferenceId == command.PortalUserCustomerNumbersEntryId);
        
        if (sortiments is not null)
        {
            await ProcessSortimentCodesEntries(command,sortiments.PortalUserCustomerNumbersEntrySortimentId,sortiments.Sortiments.Select(a => a.SortimentCode).ToList());
            
            return;
        }
        Guid entriesId = Guid.NewGuid();
        await _portalDbContext.PortalUserCustomerNumberEntrySortiments.AddAsync(new PortalUserCustomerNumberEntrySortimentData()
        {
            PortalUserReferenceId = command.PortalUserReferenceId,
            PortalUserCustomerNumbersEntryReferenceId = command.PortalUserCustomerNumbersEntryId,
            PortalUserCustomerNumbersEntrySortimentId = entriesId
        });

        await ProcessSortimentCodesEntries(command,entriesId,new List<string>());
    }
    
    private async Task ProcessSortimentCodesEntries(CreateUserCustomerNumbersSortiments command,Guid entriesId,IList<string> alreadyExistingCustomerNumbers)
    {
        foreach (var sortimentCode in command.CustomerNumberDetails.Sortiments)
        {
            if (alreadyExistingCustomerNumbers.Contains(sortimentCode))
            {
                continue;
            }

            Guid newEntryId = Guid.NewGuid();
            
            await _portalDbContext.PortalUserCustomerNumberEntrySortimentsEntries.AddAsync(new PortalUserCustomerNumberEntrySortimentEntryData()
            {
                PortalUserReferenceId = command.PortalUserReferenceId,
                SortimentCode = sortimentCode,
                PortalUserCustomerNumbersEntrySortimentEntryId = newEntryId,
                PortalUserCustomerNumbersEntrySortimentId = entriesId,
                PortalUserCustomerNumbersEntryReferenceId = command.PortalUserCustomerNumbersEntryId
            });
        }
        
        await _portalDbContext.SaveChangesAsync();
    }
}