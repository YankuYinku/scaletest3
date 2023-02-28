using apetito.CQS;
using apetito.meinapetito.Portal.Contracts.Root.Infrastructure.Consts;

namespace apetito.meinapetito.Portal.Application.Root.Intrastructure.Email.Commands;

public class RegisterEmailToSendCommand : ICommand
{
    public Guid ReferenceId { get; set; }
    public EmailOptionsContextEnum Context { get; set; } = EmailOptionsContextEnum.InvitationMail;
}