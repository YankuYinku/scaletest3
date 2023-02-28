using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Encodings.Web;

namespace apetito.meinapetito.Portal.Api.Infrastructure
{
    public class AzureADJWTAuthenticationHandler : JwtBearerHandler
    {
        public AzureADJWTAuthenticationHandler(IOptionsMonitor<JwtBearerOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        { }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authorityConfig = await Options.ConfigurationManager.GetConfigurationAsync(Context.RequestAborted);
            var authorityIssuer = authorityConfig.Issuer;

            var jwtToken = ReadTokenFromHeader();

            var jwtHandler = new JwtSecurityTokenHandler();

            if (jwtHandler.CanReadToken(jwtToken))
            {
                var token = jwtHandler.ReadJwtToken(jwtToken);
                if(authorityConfig.ClaimsSupported.Contains("impersonatedUser") && !token.Claims.Select(x => x.Type).Contains("impersonatedUser"))
                {
                    //We dont need to authenticate this case, because it will always fail
                    return AuthenticateResult.NoResult();
                }
                if (string.Equals(token.Issuer, authorityIssuer, StringComparison.OrdinalIgnoreCase))
                {
                    // Means the token was issued by this authority, we make sure full validation runs as normal
                    return await base.HandleAuthenticateAsync();
                }
                else
                {
                    // Skip validation since the token is issued by a an issuer that this instance doesn't know about
                    Logger.LogDebug($"Skipping jwt token validation because token issuer was {token.Issuer} but the authority issuer is: {authorityIssuer}");
                    return AuthenticateResult.NoResult();
                }
            }

            return await base.HandleAuthenticateAsync();
        }

        private string ReadTokenFromHeader()
        {
            string token = null;

            string authorization = Request.Headers["Authorization"];

            // If no authorization header found, nothing to process further
            if (string.IsNullOrEmpty(authorization))
            {
                return null;
            }

            if (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                token = authorization.Substring("Bearer ".Length).Trim();
            }

            return token;
        }
    }
}
