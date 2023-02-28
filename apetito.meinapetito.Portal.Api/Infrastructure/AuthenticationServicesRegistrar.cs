using apetito.DependencyInjection.Services;
using apetito.BearerToken;
using Microsoft.Extensions.Options;

namespace apetito.meinapetito.Portal.Api.Infrastructure;

public class AuthenticationServicesRegistrar : IServicesRegistrar
{
    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        var azureAdB2CAuthority = configuration["AzureAD:Authority"];
        var azureAdB2CAuthorityImpersonation = configuration["AzureAD:AuthorityImpersonation"];
        var meinapetitoPortalApiClientId = configuration["AzureAD:ClientId"];

        services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<JwtBearerOptions>, JwtBearerPostConfigureOptions>());
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddScheme<JwtBearerOptions, AzureADJWTAuthenticationHandler>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                ConfigureOptions(options, azureAdB2CAuthority, meinapetitoPortalApiClientId);
            })
            .AddScheme<JwtBearerOptions, AzureADJWTAuthenticationHandler>("ImpersonatedBearer", options =>
            {
                ConfigureOptions(options, azureAdB2CAuthorityImpersonation, meinapetitoPortalApiClientId);
            });

        services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme, "ImpersonatedBearer")
                .Build();
        });

        services.AddBearerTokenRequestProvider();
    }

    private void ConfigureOptions(JwtBearerOptions options, string azureAdB2CAuthority, string meinapetitoPortalApiClientId)
    {
        options.Authority = $"{azureAdB2CAuthority}/v2.0";

        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidAudiences = new List<string>
            {
                meinapetitoPortalApiClientId,
            },
        };
    }
}