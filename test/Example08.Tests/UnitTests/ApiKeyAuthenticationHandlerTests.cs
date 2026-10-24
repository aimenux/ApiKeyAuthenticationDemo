using System.Text.Encodings.Web;
using AwesomeAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Example08.Presentation.Authentication;

namespace Example08.Tests.UnitTests;

public class ApiKeyAuthenticationHandlerTests
{
    private const string ApiKey = "test-api-key";

    [Fact]
    public async Task When_ApiKeyHeader_Is_Valid_Then_Should_Succeed()
    {
        // arrange
        var handler = await CreateHandlerAsync(ApiKey);

        // act
        var result = await handler.AuthenticateAsync();

        // assert
        result.Succeeded.Should().BeTrue();
        result.Principal!.Identity!.Name.Should().Be(ApiKeyConstants.ApiKeyOwner);
    }

    [Fact]
    public async Task When_ApiKeyHeader_Is_Missing_Then_Should_Fail()
    {
        // arrange
        var handler = await CreateHandlerAsync(null);

        // act
        var result = await handler.AuthenticateAsync();

        // assert
        result.Succeeded.Should().BeFalse();
        result.Failure!.Message.Should().Be("Api key is missing");
    }

    [Theory]
    [InlineData("")]
    [InlineData("wrong-api-key")]
    [InlineData("test-api-ke")]
    public async Task When_ApiKeyHeader_Is_Invalid_Then_Should_Fail(string apiKey)
    {
        // arrange
        var handler = await CreateHandlerAsync(apiKey);

        // act
        var result = await handler.AuthenticateAsync();

        // assert
        result.Succeeded.Should().BeFalse();
        result.Failure!.Message.Should().Be("Api key is invalid");
    }

    private static async Task<ApiKeyAuthenticationHandler> CreateHandlerAsync(string headerValue)
    {
        var options = Options.Create(new ApiKeyOptions { ApiKey = ApiKey });

        var httpContext = new DefaultHttpContext();
        if (headerValue is not null)
        {
            httpContext.Request.Headers[ApiKeyConstants.ApiKeyHeaderName] = headerValue;
        }

        var optionsMonitor = Substitute.For<IOptionsMonitor<ApiKeyAuthenticationOptions>>();
        optionsMonitor.Get(Arg.Any<string>()).Returns(new ApiKeyAuthenticationOptions());

        var handler = new ApiKeyAuthenticationHandler(optionsMonitor, NullLoggerFactory.Instance, UrlEncoder.Default, options);
        var scheme = new AuthenticationScheme(ApiKeyConstants.ApiKeyScheme, null, typeof(ApiKeyAuthenticationHandler));
        await handler.InitializeAsync(scheme, httpContext);
        return handler;
    }
}
