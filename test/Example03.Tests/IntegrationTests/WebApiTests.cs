using System.Net;
using AwesomeAssertions;
using Microsoft.Extensions.Options;
using Example03.Presentation.Authentication;

namespace Example03.Tests.IntegrationTests;

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
        response.Headers.WwwAuthenticate.Should().ContainSingle(challenge => challenge.Scheme == ApiKeyConstants.ApiKeyChallenge);
    }

    [Theory]
    [InlineData("api/movies/1")]
    [InlineData("api/movies/list")]
    public async Task Should_Get_Movies_Returns_Unauthorized_When_Api_Key_Is_Missing(string route)
    {
        // arrange
        var fixture = new WebApiTestsFixture();
        var client = fixture.CreateClient();
        client.DefaultRequestHeaders.Remove(ApiKeyConstants.ApiKeyHeaderName);

        // act
        var response = await client.GetAsync(route);

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        response.Headers.WwwAuthenticate.Should().ContainSingle(challenge => challenge.Scheme == ApiKeyConstants.ApiKeyChallenge);
    }

    [Theory]
    [InlineData("api/movies/1")]
    [InlineData("api/movies/list")]
    public async Task Should_Get_Movies_Returns_Unauthorized_When_Api_Key_Is_Empty(string route)
    {
        // arrange
        var fixture = new WebApiTestsFixture();
        var client = fixture.CreateClient();
        client.DefaultRequestHeaders.Remove(ApiKeyConstants.ApiKeyHeaderName);
        client.DefaultRequestHeaders.Add(ApiKeyConstants.ApiKeyHeaderName, string.Empty);

        // act
        var response = await client.GetAsync(route);

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        response.Headers.WwwAuthenticate.Should().ContainSingle(challenge => challenge.Scheme == ApiKeyConstants.ApiKeyChallenge);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_Fail_To_Start_When_Api_Key_Is_Missing_Or_Empty(string configuredApiKey)
    {
        // arrange
        var fixture = new EmptyApiKeyWebApiTestsFixture(configuredApiKey);

        // act
        var act = () => fixture.CreateClient();

        // assert
        // WebApplicationFactory can surface ObjectDisposedException instead of the validation error when the host
        // fails during startup, so either one means startup was refused.
        var exception = act.Should().Throw<Exception>().Which;
        (exception is OptionsValidationException or ObjectDisposedException).Should().BeTrue();
    }
}
