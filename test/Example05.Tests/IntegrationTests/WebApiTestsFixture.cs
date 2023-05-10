using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using static Example05.Presentation.Authentication.ApiKeyConstants;

namespace Example05.Tests.IntegrationTests;

internal class WebApiTestsFixture : WebApplicationFactory<Program>
{
    private readonly string _apiKeyHeaderValue;
    
    private const string ApiKey = "ApiKey4Tests";
    
    public WebApiTestsFixture(string apiKeyHeaderValue = ApiKey)
    {
        _apiKeyHeaderValue = apiKeyHeaderValue;
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Authentication:ApiKey"] = ApiKey
            });
        });

        builder.ConfigureTestServices(services =>
        {
        });
    }

    protected override void ConfigureClient(HttpClient client)
    {
        base.ConfigureClient(client);
        client
            .DefaultRequestHeaders
            .Add(ApiKeyHeaderName, _apiKeyHeaderValue);
    }
}