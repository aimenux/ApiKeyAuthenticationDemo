namespace Example06.Presentation.Authentication;

public class ApiKeySecurityFilter : IEndpointFilter
{
    private readonly IConfiguration _configuration;

    public ApiKeySecurityFilter(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyConstants.ApiKeyHeaderName, out var requestApiKey))
        {
            return HttpResults.Unauthorized("Api key is missing");
        }

        var actualApiKey = _configuration.GetValue<string>(ApiKeyConstants.ApiKeySectionName);
        if (requestApiKey != actualApiKey)
        {
            return HttpResults.Unauthorized("Api key is invalid");
        }

        return await next(context);
    }
}