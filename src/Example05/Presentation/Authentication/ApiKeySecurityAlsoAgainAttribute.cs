using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Example05.Presentation.Authentication;

public class ApiKeySecurityAlsoAgainAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyConstants.ApiKeyHeaderName, out var requestApiKey))
        {
            context.HttpContext.Response.Headers.WWWAuthenticate = ApiKeyConstants.ApiKeyChallenge;
            context.Result = new UnauthorizedObjectResult("Api key is missing");
            return;
        }

        var options = context.HttpContext.RequestServices.GetRequiredService<IOptions<ApiKeyOptions>>();
        var actualApiKey = options.Value.ApiKey;
        
        if (!IsValidApiKey(requestApiKey.ToString(), actualApiKey))
        {
            context.HttpContext.Response.Headers.WWWAuthenticate = ApiKeyConstants.ApiKeyChallenge;
            context.Result = new UnauthorizedObjectResult("Api key is invalid");
            return;
        }

        await next();
    }

    private static bool IsValidApiKey(string requestApiKey, string actualApiKey)
    {
        var suppliedHash = SHA256.HashData(Encoding.UTF8.GetBytes(requestApiKey));
        var expectedHash = SHA256.HashData(Encoding.UTF8.GetBytes(actualApiKey));
        return CryptographicOperations.FixedTimeEquals(suppliedHash, expectedHash);
    }
}