using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Example03.Presentation.Authentication;

public class ApiKeySecurityFilterAlso : IAsyncActionFilter
{
    private readonly IConfiguration _configuration;

    public ApiKeySecurityFilterAlso(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }
    
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
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
            return;
        }

        await next();
    }
}