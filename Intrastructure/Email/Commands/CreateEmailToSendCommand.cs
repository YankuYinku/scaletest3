using apetito.CQS;
using apetito.meinapetito.Portal.Contracts.Root.Infrastructure.Consts;

namespace apetito.meinapetito.Portal.Application.Root.Intrastructure.Email.Commands;

public class CreateEmailToSendCommand : ICommand
{
    public Guid ReferenceId { get; set; }
    public EmailOptionsContextEnum Context { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public List<string> ToReceivers { get; set; }
    public List<string>? CcReceivers { get; set; }
    public List<string>? BccReceivers { get; set; }
    public List<string>? AttachmentsUris { get; set; }
}