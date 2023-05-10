using Example05.Presentation.Authentication;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace Example05.Tests.UnitTests;

public class ApiKeySecurityAttributeTests
{
    [Theory]
    [InlineData("xyz")]
    [InlineData("abc")]
    public void When_ApiKeyHeader_Is_Valid_Then_Should_Returns_Ok(string apiKey)
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
            RequestServices = new ServiceCollection().AddSingleton<IConfiguration>(configuration).BuildServiceProvider(),
            Request = { Headers = { new KeyValuePair<string, StringValues>(ApiKeyConstants.ApiKeyHeaderName, apiKey) }},
            Response =
            {
                Body = new MemoryStream()
            }
        };

        var actionContext = new ActionContext(context, new RouteData(), new ActionDescriptor());
        var filterContext = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
        var securityAttribute = new ApiKeySecurityAttribute();

        // act
        securityAttribute.OnAuthorization(filterContext);

        // assert
        filterContext.Result.Should().BeNull();
    }
    
    [Fact]
    public void When_ApiKeyHeader_Is_Missing_Then_Should_Returns_Unauthorized()
    {
        // arrange
        var configuration = new ConfigurationBuilder().Build();
        var context = new DefaultHttpContext
        {
            RequestServices = new ServiceCollection().AddSingleton<IConfiguration>(configuration).BuildServiceProvider(),
            Response =
            {
                Body = new MemoryStream()
            }
        };

        var actionContext = new ActionContext(context, new RouteData(), new ActionDescriptor());
        var filterContext = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
        var securityAttribute = new ApiKeySecurityAttribute();

        // act
        securityAttribute.OnAuthorization(filterContext);

        // assert
        filterContext.Result.Should().NotBeNull();
        filterContext.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }
    
    [Fact]
    public void When_ApiKeyHeader_Is_Invalid_Then_Should_Returns_Unauthorized()
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
            RequestServices = new ServiceCollection().AddSingleton<IConfiguration>(configuration).BuildServiceProvider(),
            Request = { Headers = { new KeyValuePair<string, StringValues>(ApiKeyConstants.ApiKeyHeaderName, "123") }},
            Response =
            {
                Body = new MemoryStream()
            }
        };

        var actionContext = new ActionContext(context, new RouteData(), new ActionDescriptor());
        var filterContext = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
        var securityAttribute = new ApiKeySecurityAttribute();

        // act
        securityAttribute.OnAuthorization(filterContext);

        // assert
        filterContext.Result.Should().NotBeNull();
        filterContext.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }
}