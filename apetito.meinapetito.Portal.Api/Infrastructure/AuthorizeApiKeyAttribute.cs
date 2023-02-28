using apetito.meinapetito.Portal.Application.Root.Users.AssignedUsers.Options;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace apetito.meinapetito.Portal.Api.Infrastructure
{
    [AttributeUsage(validOn: AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        private const string APIKEYNAME = "X-API-KEY";
        private readonly Type _optionType;

        public string SecretPropertyName { get; set; } = "Secret";

        public AuthorizeApiKeyAttribute(Type optionType)
        {
            _optionType = optionType;

        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!GetApiKey(context.HttpContext, out var extractedApiKey))
            {
                context.Result = new BadRequestResult();
                return;
            }

            var secretOption = context.HttpContext.RequestServices.GetRequiredService(_optionType);
            var secretProperty = secretOption.GetType().GetProperty(SecretPropertyName);

            if (secretProperty == null)
                throw new ArgumentException($"Option type {_optionType} must contain a property named '{SecretPropertyName}'");

            var secret = secretProperty.GetValue(secretOption) as string ?? string.Empty;

            if (!secret.Equals(extractedApiKey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            await next();
        }

        public static bool GetApiKey(HttpContext context, out StringValues extractedApiKey)
        {
            var isTokenPresent = context.Request.Headers.TryGetValue(APIKEYNAME, out extractedApiKey);
            return isTokenPresent;
        }
    }
}