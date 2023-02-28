using apetito.CQS;
using apetito.meinapetito.Portal.Data.Root;
using apetito.meinapetito.Portal.Data.Root.MainCustomerAdmins.CustomerNumbers;
using Microsoft.EntityFrameworkCore;

namespace apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Commands.Handlers;

public class CreateUserCustomerNumbersCommandHandler : ICommandHandler<CreateUserCustomerNumbers>
{
    private readonly PortalDbContext _portalDbContext;
    private readonly ICommandHandler<CreateUserCustomerNumbersSortiments> _createSortimentsCommandHandler;
    public CreateUserCustomerNumbersCommandHandler(PortalDbContext portalDbContext, ICommandHandler<CreateUserCustomerNumbersSortiments> createSortimentsCommandHandler)
    {
        _portalDbContext = portalDbContext;
        _createSortimentsCommandHandler = createSortimentsCommandHandler;
    }

    public async Task Handle(CreateUserCustomerNumbers command)
    {
        var customerNumbersEntry = await _portalDbContext.PortalUserCustomerNumbers.Include(a => a.CustomerNumbers).FirstOrDefaultAsync(a 
            => a.PortalUserReferenceId == command.UserId);
        
        if (customerNumbersEntry is not null)
        {
            await ProcessCustomerNumberEntries(command,customerNumbersEntry.PortalUserCustomerNumbersId,customerNumbersEntry.CustomerNumbers.Select(a => a.CustomerNumber).ToList());
            
            return;
        }
        Guid entriesId = Guid.NewGuid();
        await _portalDbContext.PortalUserCustomerNumbers.AddAsync(new PortalUserCustomerNumbersData
        {
            PortalUserReferenceId = command.UserId,
            PortalUserCustomerNumbersId = entriesId
        });

        await ProcessCustomerNumberEntries(command,entriesId,new List<int>());
    }

    private async Task ProcessCustomerNumberEntries(CreateUserCustomerNumbers command,Guid entriesId,IList<int> alreadyExistingCustomerNumbers)
    {
        foreach ((int customerNumber, var role) in command.CustomerNumberWithRoles)
        {
            if (alreadyExistingCustomerNumbers.Contains(customerNumber))
            {
                continue;
            }

            Guid portalUserCustomerNumbersEntryId = Guid.NewGuid();
            
            await _portalDbContext.PortalUserCustomerNumbersEntries.AddAsync(new PortalUserCustomerNumbersEntryData
            {
                Role = role.Role,
                CustomerNumber = customerNumber,
                LanguageCode = role.LanguageCode,
                OrderSystem = role.OrderSystem,
                PortalUserCustomerNumbersId = entriesId,
                PortalUserCustomerNumbersEntryId = portalUserCustomerNumbersEntryId,
                PortalUserReferenceId = command.UserId,
            });

            await _createSortimentsCommandHandler.Handle(
                new CreateUserCustomerNumbersSortiments(portalUserCustomerNumbersEntryId, command.UserId, role));
        }
        
        await _portalDbContext.SaveChangesAsync();
    }
}