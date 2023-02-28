using apetito.CQS;
using apetito.meinapetito.Portal.Api.Root.Authentication.UserAccessTokens.Helpers;
using apetito.meinapetito.Portal.Application.Contact.Commands;
using apetito.meinapetito.Portal.Contracts.Contact.Models;
using HotChocolate;
using HotChocolate.Types;

namespace apetito.meinapetito.Portal.Api.Contact;

[ExtendObjectType("Mutation")]
public class ContactMutation
{
    public async Task<ContactRequest> ContactAsync(
        [Service] ILogger<ContactMutation> logger,
        [Service] ICommandHandler<SendContact> commandHandler,
        [Service] IHttpContextAccessor httpContextAccessor,
        ContactRequest request)
    {
        var senderEmail = httpContextAccessor.HttpContext?.GetUserOrImpersonatedUserEmailAdressInGraphQl();
        var senderFirstName = httpContextAccessor.HttpContext?.GetFirstNameInGraphQl();
        var senderLastName = httpContextAccessor.HttpContext?.GetLastNameInGraphQl();
        
        SendContact sendContact = new SendContact(request.TopicCode, request.TopicDescription, request.Subject, request.Message, string.IsNullOrWhiteSpace(request.LanguageCode) ? DefaultLanguage : request.LanguageCode, senderFirstName, senderLastName, senderEmail);

        await commandHandler.Handle(sendContact);
        
        return request;
    }

    private const string DefaultLanguage = "de-DE";
}