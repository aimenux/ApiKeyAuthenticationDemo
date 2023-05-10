using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Example05.Presentation.Authentication;

public class ApiKeySecurityFilter : IAuthorizationFilter
{
    private readonly IConfiguration _configuration;

    public ApiKeySecurityFilter(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyConstants.ApiKeyHeaderName, out var requestApiKey))
        {
            context.Result = new UnauthorizedObjectResult("Api key is missing");
            return;
        }

        var actualApiKey = _configuration.GetValue<string>(ApiKeyConstants.ApiKeySectionName);
        if (requestApiKey != actualApiKey)
        {
            context.Result = new UnauthorizedObjectResult("Api key is invalid");
        }
    }
}