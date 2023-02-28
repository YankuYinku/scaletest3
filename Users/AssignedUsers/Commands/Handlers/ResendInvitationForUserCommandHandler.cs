using apetito.CQS;
using apetito.meinapetito.Portal.Application.Infrastructure.Languages;
using apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Queries;
using apetito.meinapetito.Portal.Contracts.Root.Users.AssignedUsers;

namespace apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Commands.Handlers;

public class ResendInvitationForUserCommandHandler : ICommandHandler<ResendInvitationForUser>
{
    private readonly IQueryHandler<RetrieveAssignedUserById, AssignedUserDto?> _queryHandler;
    private readonly ICommandHandler<SendMailToReinviteUser> _commandHandler;

    public ResendInvitationForUserCommandHandler(IQueryHandler<RetrieveAssignedUserById, AssignedUserDto?> queryHandler, ICommandHandler<SendMailToReinviteUser> commandHandler)
    {
        _queryHandler = queryHandler;
        _commandHandler = commandHandler;
    }

    public async Task Handle(ResendInvitationForUser command)
    {
        var user = await _queryHandler.Execute(new RetrieveAssignedUserById()
        {
            Id = command.UserId
        });

        var languages = user.CustomerNumbers.Select(a => a.LanguageCode);

        if (user is null)
        {
            throw new Exception("User not found");
        }

        if (user.Status == "Active")
        {
            throw new Exception("User is active already");
        }
        
        SendMailToReinviteUser resendInvitationForUser =
            new SendMailToReinviteUser(user.FirstName,user.LastName,user.Email,command.CurrentUserEmail, CollectLanguageCodeForInvitation(languages));

        await _commandHandler.Handle(resendInvitationForUser);
    }

    private static string CollectLanguageCodeForInvitation(IEnumerable<string> languagesCollection)
    {
        var languages = languagesCollection.ToList();
        
        if (languages.Contains(Languages.German))
        {
            return Languages.German;
        }

        if (languages.Contains(Languages.Austrian))
        {
            return Languages.Austrian;
        }

        if (languages.Contains(Languages.Dutch))
        {
            return Languages.Dutch;
        }

        return Languages.German;
    }
}