using apetito.CQS;
using apetito.DependencyInjection.Services;
using apetito.meinapetito.Portal.Application.Root.Authentication.UserAccessTokens.BearerToken;
using apetito.meinapetito.Portal.Application.Root.Authentication.UserAccessTokens.IbsscToken;
using apetito.meinapetito.Portal.Application.Root.Authentication.UserAccessTokens;
using apetito.meinapetito.Portal.Application.Root.Authentication.UserAccessTokens.BearerToken.Queries;
using apetito.meinapetito.Portal.Application.Root.Authentication.UserAccessTokens.BearerToken.Queries.Handlers;
using apetito.meinapetito.Portal.Application.Root.Authentication.UserAccessTokens.BearerToken.Services;
using apetito.meinapetito.Portal.Application.Root.Authentication.UserAccessTokens.IbsscToken.Queries;
using apetito.meinapetito.Portal.Application.Root.Authentication.UserAccessTokens.IbsscToken.Queries.Handlers;
using apetito.meinapetito.Portal.Contracts.Root.Authentication.UserAccessToken;

namespace apetito.meinapetito.Portal.Api.Root.Authentication.UserAccessTokens;

public class UserAccessTokenServicesRegistrar : IServicesRegistrar
{
    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<LegacyBearerTokenOptions>().Bind(configuration.GetSection("UserAccessToken:BearerToken"));
        services.AddOptions<IbsscTokenOptions>().Bind(configuration.GetSection("UserAccessToken:IbsscToken"));
        services.AddTransient<IUserAccessBearerTokenFactory, LegacyBearerTokenFactory>();
        services.AddTransient<IUserAccessIbsscTokenFactory, IbsscTokenFactory>();
        
        services
            .AddTransient<IQueryHandler<RetrieveUserAccessBearerTokenByCustomersOfUserQuery, UserAccessBearerTokenDto>,
                RetrieveUserAccessBearerTokenByCustomersOfUserQueryHandler>();
        
        services
            .AddTransient<IQueryHandler<RetrieveUserAccessBearerTokenByUserEmailQuery, UserAccessBearerTokenDto>,
                RetrieveUserAccessBearerTokenByUserEmailQueryHandler>();
        
        services
            .AddTransient<IQueryHandler<RetrieveUserAccessIbsscTokenByUserEmailQuery, UserAccessIbsscTokenDto>,
                RetrieveUserAccessIbsscTokenByUserEmailQueryHandler>();
    }
}