using apetito.CQS;
using apetito.Notification.Contracts.Models.Message;

namespace apetito.meinapetito.Portal.Application.Root.Intrastructure.Email.Queries;

public class RetrieveEmailToSendQuery : IQuery<Notification.Contracts.Models.Message.SendGrid>
{
    public string ReferenceId { get; set; }
}