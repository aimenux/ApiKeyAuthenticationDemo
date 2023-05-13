using System.Net;
using FluentAssertions;

namespace Example08.Tests.IntegrationTests;

public class WebApiTests
{
    [Theory]
    [InlineData("api/movies/1")]
    [InlineData("api/movies/list")]
    public async Task Should_Get_Movies_Returns_Success(string route)
    {
        // arrange
        var fixture = new WebApiTestsFixture();
        var client = fixture.CreateClient();

        // act
        var response = await client.GetAsync(route);
        var responseBody = await response.Content.ReadAsStringAsync();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseBody.Should().NotBeNullOrWhiteSpace();
    }
    
    [Theory]
    [InlineData("api/movies/1")]
    [InlineData("api/movies/list")]
    public async Task Should_Get_Movies_Returns_Unauthorized(string route)
    {
        // arrange
        var apiKeyHeaderValue = Guid.NewGuid().ToString();
        var fixture = new WebApiTestsFixture(apiKeyHeaderValue);
        var client = fixture.CreateClient();

        // act
        var response = await client.GetAsync(route);

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}