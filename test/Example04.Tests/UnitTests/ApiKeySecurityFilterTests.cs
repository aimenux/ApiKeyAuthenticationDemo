using Example04.Presentation.Authentication;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Example04.Tests.UnitTests;

public class ApiKeySecurityFilterTests
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

        var filterContext = new FilterContext(context);
        var securityFilter = new ApiKeySecurityFilter(configuration);

        // act
        var result = await securityFilter.InvokeAsync(filterContext, _ => new ValueTask<object>(Results.Ok()));

        // assert
        result.Should().BeOfType<Ok>();
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

        var filterContext = new FilterContext(context);
        var securityFilter = new ApiKeySecurityFilter(configuration);

        // act
        var result = await securityFilter.InvokeAsync(filterContext, _ => new ValueTask<object>());

        // assert
        result.Should().BeOfType<HttpResults.UnauthorizedHttpResultWithResponseBody>();
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

        var filterContext = new FilterContext(context);
        var securityFilter = new ApiKeySecurityFilter(configuration);

        // act
        var result = await securityFilter.InvokeAsync(filterContext, _ => new ValueTask<object>());

        // assert
        result.Should().BeOfType<HttpResults.UnauthorizedHttpResultWithResponseBody>();
    }

    private class FilterContext : EndpointFilterInvocationContext
    {
        public FilterContext(HttpContext httpContext)
        {
            HttpContext = httpContext;
            Arguments = new List<object>();
        }

        public override T GetArgument<T>(int index)
        {
            return default;
        }

        public override HttpContext HttpContext { get; }
        public override IList<object> Arguments { get; }
    }
}