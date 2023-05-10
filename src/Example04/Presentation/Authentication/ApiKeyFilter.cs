namespace Example04.Presentation.Authentication;

public class ApiKeyFilter : IEndpointFilter
{
    private readonly IConfiguration _configuration;

    public ApiKeyFilter(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyConstants.ApiKeyHeaderName, out var requestApiKey))
        {
            return Results.Unauthorized();
        }

        var actualApiKey = _configuration.GetValue<string>(ApiKeyConstants.ApiKeySectionName);
        if (requestApiKey != actualApiKey)
        {
            return Results.Unauthorized();
        }

        return await next(context);
    }
}