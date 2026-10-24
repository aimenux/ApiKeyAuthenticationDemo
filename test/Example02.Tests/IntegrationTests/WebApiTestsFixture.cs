using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using static Example02.Presentation.Authentication.ApiKeyConstants;

namespace Example02.Tests.IntegrationTests;

internal class WebApiTestsFixture : WebApplicationFactory<Program>
{
    private readonly string _apiKeyHeaderValue;
    
    private const string DefaultApiKeyValue = "ApiKey4Tests";
    
    public WebApiTestsFixture(string apiKeyHeaderValue = DefaultApiKeyValue)
    {
        _apiKeyHeaderValue = apiKeyHeaderValue;
        ClientOptions.BaseAddress = new Uri("https://localhost:5000");
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Authentication:ApiKey"] = DefaultApiKeyValue,
                ["Https:Port"] = "5000"
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