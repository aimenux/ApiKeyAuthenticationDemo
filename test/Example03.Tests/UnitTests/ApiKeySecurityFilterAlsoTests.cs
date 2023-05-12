using Example03.Presentation.Authentication;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Example03.Tests.UnitTests;

public class ApiKeySecurityFilterAlsoTests
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

        var actionContext = new ActionContext(context, new RouteData(), new ActionDescriptor());
        var executingContext = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object>(), "controller");
        var executedContext = new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), "controller");
        var securityFilter = new ApiKeySecurityFilterAlso(configuration);

        // act
        await securityFilter.OnActionExecutionAsync(executingContext, () => Task.FromResult(executedContext));

        // assert
        executingContext.Result.Should().BeNull();
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

        var actionContext = new ActionContext(context, new RouteData(), new ActionDescriptor());
        var executingContext = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object>(), "controller");
        var executedContext = new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), "controller");
        var securityFilter = new ApiKeySecurityFilterAlso(configuration);

        // act
        await securityFilter.OnActionExecutionAsync(executingContext, () => Task.FromResult(executedContext));

        // assert
        executingContext.Result.Should().BeOfType<UnauthorizedObjectResult>();
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

        var actionContext = new ActionContext(context, new RouteData(), new ActionDescriptor());
        var executingContext = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object>(), "controller");
        var executedContext = new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), "controller");
        var securityFilter = new ApiKeySecurityFilterAlso(configuration);

        // act
        await securityFilter.OnActionExecutionAsync(executingContext, () => Task.FromResult(executedContext));

        // assert
        executingContext.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }
}