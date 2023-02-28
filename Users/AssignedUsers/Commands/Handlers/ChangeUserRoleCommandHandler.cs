using apetito.CQS;
using apetito.meinapetito.Portal.Data.Root;
using Microsoft.EntityFrameworkCore;

namespace apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Commands.Handlers;

public class ChangeUserRoleCommandHandler : ICommandHandler<ChangeUserRole>
{
    private readonly PortalDbContext _context;

    public ChangeUserRoleCommandHandler(PortalDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ChangeUserRole command)
    {
        var entry = await _context.PortalUserCustomerNumbersEntries.FirstOrDefaultAsync(a =>
            a.CustomerNumber == command.CustomerNumber && a.PortalUserReferenceId == command.UserId);

        if (entry is null)
        {
            return;
        }

        entry.Role = command.Role;

        await _context.SaveChangesAsync();
    }
}