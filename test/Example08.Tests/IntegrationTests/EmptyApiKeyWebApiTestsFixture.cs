using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Example08.Tests.IntegrationTests;

internal class EmptyApiKeyWebApiTestsFixture : WebApplicationFactory<Program>
{
    private readonly string _emptyApiKeyValue;

    public EmptyApiKeyWebApiTestsFixture(string configuredApiKey)
    {
        _emptyApiKeyValue = configuredApiKey;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Authentication:ApiKey"] = _emptyApiKeyValue
            });
        });
    }
}
