using apetito.CQS;
using apetito.meinapetito.Portal.Data.Root;
using apetito.meinapetito.Portal.Data.Root.MainCustomerAdmins.CustomerNumbers;
using Microsoft.EntityFrameworkCore;

namespace apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Commands.Handlers;

public class RemoveCustomerNumberFromUserCommandHandler : ICommandHandler<RemoveCustomerNumberFromUserCommand>
{
    private readonly PortalDbContext _portalDbContext;

    public RemoveCustomerNumberFromUserCommandHandler(PortalDbContext portalDbContext)
    {
        _portalDbContext = portalDbContext;
    }

    public async Task Handle(RemoveCustomerNumberFromUserCommand command)
    {
        foreach (var customerNumber in command.CustomerNumbers)
        {
            var number = await _portalDbContext.PortalUserCustomerNumbersEntries.Where(a =>
                a.CustomerNumber == customerNumber && a.PortalUserReferenceId == command.Id).FirstOrDefaultAsync();

            if (number is null)
            {
                return;
            }

            await RemoveEntryAndConnections(number);
        }

        await _portalDbContext.SaveChangesAsync();
    }

    private async Task RemoveEntryAndConnections(PortalUserCustomerNumbersEntryData number)
    {
        Guid entryToDelete = number.PortalUserCustomerNumbersEntryId;

        _portalDbContext.Remove(number);

        var sortimentsToDelete = await _portalDbContext.PortalUserCustomerNumberEntrySortiments.Where(a =>
            a.PortalUserCustomerNumbersEntryReferenceId == entryToDelete).ToListAsync();

        var sortimentsEntriesToDelete =
            await _portalDbContext.PortalUserCustomerNumberEntrySortimentsEntries.Where(a =>
                a.PortalUserCustomerNumbersEntryReferenceId == entryToDelete).ToListAsync();

        foreach (var sortimentEntryToDelete in sortimentsEntriesToDelete)
        {
            _portalDbContext.Remove(sortimentEntryToDelete);
        }

        foreach (var sortimentToDelete in sortimentsToDelete)
        {
            _portalDbContext.Remove(sortimentToDelete);
        }
    }
}