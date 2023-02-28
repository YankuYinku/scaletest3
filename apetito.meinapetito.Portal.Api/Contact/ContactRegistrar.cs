using apetito.CQS;
using apetito.DependencyInjection.Services;
using apetito.meinapetito.Portal.Application.Contact.Commands;
using apetito.meinapetito.Portal.Application.Contact.Commands.Handlers;

namespace apetito.meinapetito.Portal.Api.Contact;

public class ContactRegistrar : IServicesRegistrar
{
    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<ICommandHandler<SendContact>, SendContactHandler>();
    }
}