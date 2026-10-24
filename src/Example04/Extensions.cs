namespace Example04;

public static class WebApplicationExtensions
{
    public static WebApplicationBuilder WithHttpsCheck(this WebApplicationBuilder builder)
    {
        var httpsPort = GetHttpsPort(builder.Configuration);

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenLocalhost(httpsPort, listen => listen.UseHttps());
        });

        return builder;
    }

    private static int GetHttpsPort(ConfigurationManager configuration)
    {
        var configuredPort = configuration.GetValue<int?>("Https:Port");
        if (configuredPort is { } port)
        {
            return port;
        }

        var httpsUri = (configuration["urls"] ?? string.Empty)
            .Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Select(url => new Uri(url))
            .FirstOrDefault(uri => uri.Scheme == Uri.UriSchemeHttps);

        return httpsUri?.Port ?? throw new InvalidOperationException("No HTTPS port configured.");
    }
}
