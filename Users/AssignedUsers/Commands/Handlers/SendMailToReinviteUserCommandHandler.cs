using apetito.CQS;
using apetito.meinapetito.Portal.Application.Root.Intrastructure.Email.Commands;

namespace apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Commands.Handlers;

public class SendMailToReinviteUserCommandHandler : ICommandHandler<SendMailToReinviteUser>
{
    private readonly ICommandHandler<SendMailToUserAboutInvitationToCustomerNumbers> _commandHandler;

    public SendMailToReinviteUserCommandHandler(
        ICommandHandler<SendMailToUserAboutInvitationToCustomerNumbers> commandHandler)
    {
        _commandHandler = commandHandler;
    }

    public async Task Handle(SendMailToReinviteUser command)
    {
        await _commandHandler.Handle(new SendMailToUserAboutInvitationToCustomerNumbers(command.FirstName,
            command.LastName, command.Email, command.InvitatorEmail, command.WasAlreadyInAzureADB2C,command.LanguageCode));
    }
}