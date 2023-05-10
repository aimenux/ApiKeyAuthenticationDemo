using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Example05.Presentation.Authentication;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiKeySecurityAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyConstants.ApiKeyHeaderName, out var requestApiKey))
        {
            context.Result = new UnauthorizedObjectResult("Api key is missing");
            return;
        }

        var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var actualApiKey = configuration.GetValue<string>(ApiKeyConstants.ApiKeySectionName);
        if (requestApiKey != actualApiKey)
        {
            context.Result = new UnauthorizedObjectResult("Api key is invalid");
        }
    }
}