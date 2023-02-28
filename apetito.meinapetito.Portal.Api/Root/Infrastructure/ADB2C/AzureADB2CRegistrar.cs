using apetito.CQS;
using apetito.DependencyInjection.Services;
using apetito.meinapetito.Portal.Application.Root.Intrastructure.ADB2C.Options;
using apetito.meinapetito.Portal.Application.Root.Intrastructure.ADB2C.Queries;
using apetito.meinapetito.Portal.Application.Root.Intrastructure.ADB2C.Queries.Handlers;
using apetito.meinapetito.Portal.Contracts.Root.Infrastructure.ADB2C;

namespace apetito.meinapetito.Portal.Api.Root.Infrastructure.ADB2C
{
    public class AzureADB2CRegistrar : IServicesRegistrar
    {
        public void AddServices(IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddTransient<IQueryHandler<RetrieveAzureADB2CUserQuery, UserDto?>,
                    RetrieveAzureADB2CUserQueryHandler>();
            RegisterOptions(services, configuration);
        }

        private void RegisterOptions(IServiceCollection services, IConfiguration configuration)
        {
            var tenantId = configuration["AzureADB2C:TenantID"];
            var domain = configuration["AzureADB2C:Domain"];
            var clientId = configuration["AzureADB2C:AppRegistration:PortalManagement:ClientID"];
            var clientSecret = configuration["AzureADB2C:AppRegistration:PortalManagement:ClientSecret"];

            var options = new AzureADB2CUserOptions
            {
                TenantId = tenantId, ClientId = clientId, ClientSecret = clientSecret, Domain = domain
            };
            services.AddSingleton(options);
        }
    }
}