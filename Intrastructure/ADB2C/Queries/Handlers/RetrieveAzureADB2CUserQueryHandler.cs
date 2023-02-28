using System.Net;
using apetito.CQS;
using apetito.meinapetito.Portal.Application.Root.Intrastructure.ADB2C.Options;
using apetito.meinapetito.Portal.Contracts.Root.Infrastructure.ADB2C;
using Azure.Identity;
using Microsoft.Graph;

namespace apetito.meinapetito.Portal.Application.Root.Intrastructure.ADB2C.Queries.Handlers
{
    public class RetrieveAzureADB2CUserQueryHandler : IQueryHandler<RetrieveAzureADB2CUserQuery, UserDto>
    {
        private readonly string[] _scopes = new[] {"https://graph.microsoft.com/.default"};
        private readonly AzureADB2CUserOptions _options;

        public RetrieveAzureADB2CUserQueryHandler(AzureADB2CUserOptions options)
        {
            _options = options;
        }

        public async Task<UserDto?> Execute(RetrieveAzureADB2CUserQuery query)
        {
            var graphClient = PrepareAzureGraphClient();

            var request = await graphClient.Users.Request().Top(2)
                .Select(a => new
                {
                    a.Identities, a.Surname, a.Id, a.GivenName, a.Mail
                })
                .GetAsync();

            var allUsers = request.CurrentPage.ToList();

            var nextPageRequest = request.NextPageRequest;
            
            while (nextPageRequest is not null)
            {
                var result = await nextPageRequest.GetAsync();
                
                allUsers.AddRange(result.CurrentPage.ToList());

                nextPageRequest = result.NextPageRequest;
            }

            var user = allUsers.FirstOrDefault(a => a.Identities is not null && a.Identities.Any(b => b.IssuerAssignedId == query.Email));

            if (user is {})
            {
                return MapUserToUserDto(user,query);
            }

            user = allUsers.FirstOrDefault(a => a.Mail == query.Email);
            
            return user is null ? null : MapUserToUserDto(user,query);
        }

        private GraphServiceClient PrepareAzureGraphClient()
        {
            var tenantId = _options.TenantId;
            var clientId = _options.ClientId;
            var clientSecret = _options.ClientSecret;

            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
            };

            var clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret, options);

            var token = new TokenCredentialAuthProvider(clientSecretCredential, _scopes);

            var graphClient = new GraphServiceClient(token);
            return graphClient;
        }

        private static UserDto MapUserToUserDto(User user,RetrieveAzureADB2CUserQuery query)
        {
            return new UserDto
            {
                AzureADB2CId = user.Id,
                FirstName = user.GivenName,
                LastName = user.Surname,
                Email = query.Email
            };
        }
    }
}