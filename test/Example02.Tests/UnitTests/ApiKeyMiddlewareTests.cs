using Example02.Presentation.Authentication;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Example02.Tests.UnitTests;

public class ApiKeyMiddlewareTests
{
    [Theory]
    [InlineData("xyz")]
    [InlineData("abc")]
    public async Task When_ApiKeyHeader_Is_Valid_Then_Should_Returns_Ok(string apiKey)
    {
        // arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Authentication:ApiKey"] = apiKey
            })
            .Build();
        var context = new DefaultHttpContext
        {
            Request = { Headers = { new KeyValuePair<string, StringValues>(ApiKeyConstants.ApiKeyHeaderName, apiKey) }},
            Response =
            {
                Body = new MemoryStream()
            }
        };

        RequestDelegate next = _ => Task.CompletedTask;
        var middleware = new ApiKeyMiddleware(next, configuration);

        // act
        await middleware.InvokeAsync(context);

        // assert
        context.Response.StatusCode.Should().Be(200);
    }
    
    [Fact]
    public async Task When_ApiKeyHeader_Is_Missing_Then_Should_Returns_Unauthorized()
    {
        // arrange
        var configuration = new ConfigurationBuilder().Build();
        var context = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        RequestDelegate next = _ => Task.CompletedTask;
        var middleware = new ApiKeyMiddleware(next, configuration);

        // act
        await middleware.InvokeAsync(context);

        // assert
        context.Response.StatusCode.Should().Be(401);
    }
    
    [Fact]
    public async Task When_ApiKeyHeader_Is_Invalid_Then_Should_Returns_Unauthorized()
    {
        // arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Authentication:ApiKey"] = "xyz"
            })
            .Build();
        var context = new DefaultHttpContext
        {
            Request = { Headers = { new KeyValuePair<string, StringValues>(ApiKeyConstants.ApiKeyHeaderName, "123") }},
            Response =
            {
                Body = new MemoryStream()
            }
        };

        RequestDelegate next = _ => Task.CompletedTask;
        var middleware = new ApiKeyMiddleware(next, configuration);

        // act
        await middleware.InvokeAsync(context);

        // assert
        context.Response.StatusCode.Should().Be(401);
    }
}