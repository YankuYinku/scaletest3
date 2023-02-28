using apetito.CQS;
using apetito.meinapetito.Portal.Application.Infrastructure.Prismic.Services.Implementations;
using apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Options;
using apetito.meinapetito.Portal.Contracts.Root.Infrastructure;
using prismic;
using prismic.fragments;

namespace apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Queries.Handlers;

public class RetrieveFilledEmailToInviteUserQueryHandler : IQueryHandler<RetrieveFilledEmailToInviteUser, EmailDto>
{
    private readonly EmailSendingDocumentTypeOptions _emailSendingDocumentTypeOptions;
    private readonly IPrismicApiClientCallExecutor _prismicApiClientCallExecutor;

    public RetrieveFilledEmailToInviteUserQueryHandler(EmailSendingDocumentTypeOptions emailSendingDocumentTypeOptions,
        IPrismicApiClientCallExecutor prismicApiClientCallExecutor)
    {
        _emailSendingDocumentTypeOptions = emailSendingDocumentTypeOptions;
        _prismicApiClientCallExecutor = prismicApiClientCallExecutor;
    }

    public Task<EmailDto> Execute(RetrieveFilledEmailToInviteUser query)
        => PrepareMailContent(query);

    private async Task<EmailDto> PrepareMailContent(RetrieveFilledEmailToInviteUser query)
    {
        var item = await _prismicApiClientCallExecutor.GetSingletonDocumentAsync(_emailSendingDocumentTypeOptions.Type,
            query.LanguageCode
        );

        if (item is null)
        {
            return new EmailDto();
        }
        
        var subject = ExtractSubjectFromDocument(item);

        string content = ExtractContent(item);

        var filledContent = ReplacePlaceholderWithValues(query, content);
        
        return new EmailDto
        {
            Content = filledContent,
            Subject = subject
        };
    }

    private static string ExtractContent(WithFragments item)
    {
        var content = string.Empty;

        var contentFragment = item.Fragments[ContentDocumentKey];

        if (contentFragment is StructuredText structuredText)
        {
            content = structuredText.AsHtml(DummyLambdaDocumentLink);
        }

        return content;
    }

    private static string ExtractSubjectFromDocument(WithFragments item)
    {
        var subject = string.Empty;
        
        var subjectFragment = item.Fragments[SubjectDocumentKey];

        if (subjectFragment is Text text)
        {
            subject = text.Value;
        }

        return subject;
    }

    private static string ReplacePlaceholderWithValues(RetrieveFilledEmailToInviteUser query, string content)
    {
        return ReplacingMap.Aggregate(content, (current, replacingMapItem) 
            => current.Replace(replacingMapItem.Key, replacingMapItem.Value(query)));
    }

    private static readonly IDictionary<string, Func<RetrieveFilledEmailToInviteUser, string>> ReplacingMap =
        new Dictionary<string, Func<RetrieveFilledEmailToInviteUser, string>>
        {
            {FirstnamePlaceholder, a => a.FirstName},
            {LastnamePlaceholder, a => a.LastName},
            {InviteFromEmailPlaceholder, a => a.InvitatorEmail},
            {InviteToEmailPlaceholder, a => a.Email}
        };



    private const string FirstnamePlaceholder = "{{firstname}}";
    private const string LastnamePlaceholder = "{{lastname}}";
    private const string InviteFromEmailPlaceholder = "{{InviteFromEmail}}";
    private const string InviteToEmailPlaceholder = "{{InviteToEmail}}";
    
    private const string SubjectDocumentKey = "e-mail-template_benutzer-einladung.subject";
    private const string ContentDocumentKey = "e-mail-template_benutzer-einladung.body";

    private static string DummyLinkResolverFunction(DocumentLink documentLink) => documentLink.Id;
    
    private static LambdaDocumentLinkResolver DummyLambdaDocumentLink => new (DummyLinkResolverFunction);
}