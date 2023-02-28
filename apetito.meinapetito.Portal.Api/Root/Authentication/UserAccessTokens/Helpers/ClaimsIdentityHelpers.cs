using System.IdentityModel.Tokens.Jwt;

namespace apetito.meinapetito.Portal.Api.Root.Authentication.UserAccessTokens.Helpers;

public static class ClaimsIdentityHelpers
{
    public static string GetUserOrImpersonatedUserEmailAdressInGraphQl(this HttpContext httpContext)
    {
        var token = new JwtSecurityTokenHandler().ReadJwtToken(httpContext.Request.Headers["Authorization"]);
        var emailAddress = new ClaimsPrincipal(new ClaimsIdentity(token.Claims)).GetUserOrImpersonatedUserEmailAdress();
        return emailAddress;
    }
    
    public static string GetFirstNameInGraphQl(this HttpContext httpContext)
    {
        var token = new JwtSecurityTokenHandler().ReadJwtToken(httpContext.Request.Headers["Authorization"]);
        var firstName = new ClaimsPrincipal(new ClaimsIdentity(token.Claims)).GetFirstName();
        
        return firstName;
    }
    
    public static string GetLastNameInGraphQl(this HttpContext httpContext)
    {
        var token = new JwtSecurityTokenHandler().ReadJwtToken(httpContext.Request.Headers["Authorization"]);
        var lastName = new ClaimsPrincipal(new ClaimsIdentity(token.Claims)).GetLastName();
        
        return lastName;
    }
    
    public static string GetFirstName(this ClaimsPrincipal? user)
    {
        var firstName = (user?.Identity as ClaimsIdentity)?.Claims.Where(x => x.Type == "given_name").Select(x => x.Value).FirstOrDefault() ?? string.Empty;

        return firstName;
    }

    public static string GetLastName(this ClaimsPrincipal? user)
    {
        var lastName = (user?.Identity as ClaimsIdentity)?.Claims.Where(x => x.Type == "family_name").Select(x => x.Value).FirstOrDefault() ?? string.Empty;

        return lastName;
    }
    
    public static string GetUserOrImpersonatedUserEmailAdress(this ClaimsPrincipal? user)
    {
        if (IsImpersonatedUserLogin(user))
            return GetImpersonatedUserEmailAdress(user);
        
        return GetUserEmailAdress(user);
    }

    public static string GetUserEmailAdress(this ClaimsPrincipal? user)
    {
        return  (user?.Identity as ClaimsIdentity)?.Claims.Where(x => x.Type == "emails").Select(x => x.Value).FirstOrDefault() ?? string.Empty;
    }

    public static bool IsImpersonatedUserLogin(this ClaimsPrincipal? user)
    {
        return (user?.Identity as ClaimsIdentity)?.Claims?.Any(x => x.Type == "impersonatedUser") ?? false;
    }

    public static string GetImpersonatedUserEmailAdress(this ClaimsPrincipal? user)
    {
        return (user?.Identity as ClaimsIdentity)?.Claims?.SingleOrDefault(x => x.Type == "impersonatedUser")?.Value ?? string.Empty;
    }
}