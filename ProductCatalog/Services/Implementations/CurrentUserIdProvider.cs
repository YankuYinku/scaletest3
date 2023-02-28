using System.IdentityModel.Tokens.Jwt;
using apetito.BearerToken;
using apetito.meinapetito.Portal.Application.ProductCatalog.Services.Interfaces;

namespace apetito.meinapetito.Portal.Application.ProductCatalog.Services.Implementations;

public class CurrentUserIdProvider : ICurrentUserIdProvider
{
    private readonly IBearerTokenRequestProvider _currentUserTokenProvider;

    public CurrentUserIdProvider(IBearerTokenRequestProvider currentUserTokenProvider)
    {
        _currentUserTokenProvider = currentUserTokenProvider;
    }

    public Guid CurrentUserId()
    {
        var token = _currentUserTokenProvider.Authorization.Parameter;
        var jwt = new JwtSecurityToken(token);
        return Guid.Parse(jwt.Subject);
    }
}