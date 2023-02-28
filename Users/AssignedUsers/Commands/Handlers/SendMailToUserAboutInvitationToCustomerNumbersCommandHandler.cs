using apetito.CQS;
using apetito.meinapetito.Portal.Application.Infrastructure.Languages;
using apetito.meinapetito.Portal.Application.Root.Intrastructure.Email.Commands;
using apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Queries;
using apetito.meinapetito.Portal.Contracts.Root.Infrastructure;
using apetito.meinapetito.Portal.Contracts.Root.Infrastructure.Consts;

namespace apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Commands.Handlers;

public class
    SendMailToUserAboutInvitationToCustomerNumbersCommandHandler : ICommandHandler<
        SendMailToUserAboutInvitationToCustomerNumbers>
{
    private readonly ICommandHandler<CreateEmailToSendCommand> _createCommandHandler;
    private readonly ICommandHandler<RegisterEmailToSendCommand> _registerCommandHandler;
    private readonly IQueryHandler<RetrieveFilledEmailToInviteUser, EmailDto> _emailContentQueryHandler;

    public SendMailToUserAboutInvitationToCustomerNumbersCommandHandler(
        ICommandHandler<CreateEmailToSendCommand> createCommandHandler,
        ICommandHandler<RegisterEmailToSendCommand> registerCommandHandler,
        IQueryHandler<RetrieveFilledEmailToInviteUser, EmailDto> emailContentQueryHandler)
    {
        _createCommandHandler = createCommandHandler;
        _registerCommandHandler = registerCommandHandler;
        _emailContentQueryHandler = emailContentQueryHandler;
    }

    public async Task Handle(SendMailToUserAboutInvitationToCustomerNumbers command)
    {
        var emailContent = await GetEmailTemplate(command);
        var referenceId = Guid.NewGuid();

        await _createCommandHandler.Handle(new CreateEmailToSendCommand()
        {
            ReferenceId = referenceId,
            Context = EmailOptionsContextEnum.InvitationMail,
            Subject = emailContent.Subject,
            Body = emailContent.Content,
            ToReceivers = new List<string> {command.Email},
            CcReceivers = new List<string>(),
            BccReceivers = new List<string>(),
            AttachmentsUris = new List<string>()
        });
        await _registerCommandHandler.Handle(new RegisterEmailToSendCommand
        {
            ReferenceId = referenceId,
            Context = EmailOptionsContextEnum.InvitationMail
        });
    }

    private async Task<EmailDto?> GetEmailTemplate(SendMailToUserAboutInvitationToCustomerNumbers command)
    {
        var retrieveTemplateInRequestedLanguage = new RetrieveFilledEmailToInviteUser()
        {
            Email = command.Email,
            FirstName = command.FirstName,
            InvitatorEmail = command.InvitatorEmail,
            LastName = command.LastName,
            WasAlreadyInAzureADB2C = command.WasAlreadyInAzureADB2C,
            LanguageCode = command.LanguageCode
        };

        var emailContent = await _emailContentQueryHandler.Execute(retrieveTemplateInRequestedLanguage);

        if (emailContent.IsSubjectOrContentEmpty)
        {
            var retrieveTemplateInFallbackLanguage = retrieveTemplateInRequestedLanguage with { LanguageCode = Languages.German };
            return await _emailContentQueryHandler.Execute(retrieveTemplateInFallbackLanguage);
        }

        return emailContent;
    }
}