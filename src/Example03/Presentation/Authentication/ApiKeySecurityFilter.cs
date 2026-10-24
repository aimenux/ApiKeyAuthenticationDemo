using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Example03.Presentation.Authentication;

public class ApiKeySecurityFilter : IAuthorizationFilter
{
    private readonly IOptions<ApiKeyOptions> _options;

    public ApiKeySecurityFilter(IOptions<ApiKeyOptions> options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyConstants.ApiKeyHeaderName, out var requestApiKey))
        {
            context.HttpContext.Response.Headers.WWWAuthenticate = ApiKeyConstants.ApiKeyChallenge;
            context.Result = new UnauthorizedObjectResult("Api key is missing");
            return;
        }

        var actualApiKey = _options.Value.ApiKey;
        
        if (!IsValidApiKey(requestApiKey.ToString(), actualApiKey))
        {
            context.HttpContext.Response.Headers.WWWAuthenticate = ApiKeyConstants.ApiKeyChallenge;
            context.Result = new UnauthorizedObjectResult("Api key is invalid");
        }
    }

    private static bool IsValidApiKey(string requestApiKey, string actualApiKey)
    {
        var suppliedHash = SHA256.HashData(Encoding.UTF8.GetBytes(requestApiKey));
        var expectedHash = SHA256.HashData(Encoding.UTF8.GetBytes(actualApiKey));
        return CryptographicOperations.FixedTimeEquals(suppliedHash, expectedHash);
    }
}