using Example05.Presentation.Authentication;
using AwesomeAssertions;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Example05.Tests.UnitTests;

public class ApiKeySecurityFilterTests
{
    [Theory]
    [InlineData("xyz")]
    [InlineData("abc")]
    public void When_ApiKeyHeader_Is_Valid_Then_Should_Returns_Ok(string apiKey)
    {
        // arrange
        var options = Options.Create(new ApiKeyOptions { ApiKey = apiKey });
        var context = new DefaultHttpContext
        {
            Request = { Headers = { new KeyValuePair<string, StringValues>(ApiKeyConstants.ApiKeyHeaderName, apiKey) }},
            Response =
            {
                Body = new MemoryStream()
            }
        };

        var actionContext = new ActionContext(context, new RouteData(), new ActionDescriptor());
        var filterContext = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
        var securityFilter = new ApiKeySecurityFilter(options);

        // act
        securityFilter.OnAuthorization(filterContext);

        // assert
        filterContext.Result.Should().BeNull();
    }
    
    [Fact]
    public void When_ApiKeyHeader_Is_Missing_Then_Should_Returns_Unauthorized()
    {
        // arrange
        var options = Options.Create(new ApiKeyOptions());
        var context = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        var actionContext = new ActionContext(context, new RouteData(), new ActionDescriptor());
        var filterContext = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
        var securityFilter = new ApiKeySecurityFilter(options);

        // act
        securityFilter.OnAuthorization(filterContext);

        // assert
        filterContext.Result.Should().NotBeNull();
        filterContext.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }
    
    [Fact]
    public void When_ApiKeyHeader_Is_Invalid_Then_Should_Returns_Unauthorized()
    {
        // arrange
        var options = Options.Create(new ApiKeyOptions { ApiKey = "xyz" });
        var context = new DefaultHttpContext
        {
            Request = { Headers = { new KeyValuePair<string, StringValues>(ApiKeyConstants.ApiKeyHeaderName, "123") }},
            Response =
            {
                Body = new MemoryStream()
            }
        };

        var actionContext = new ActionContext(context, new RouteData(), new ActionDescriptor());
        var filterContext = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
        var securityFilter = new ApiKeySecurityFilter(options);

        // act
        securityFilter.OnAuthorization(filterContext);

        // assert
        filterContext.Result.Should().NotBeNull();
        filterContext.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }
}