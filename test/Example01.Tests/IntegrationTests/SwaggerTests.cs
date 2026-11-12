using System.Net;
using System.Text.Json;
using AwesomeAssertions;
using Example01.Presentation.Authentication;

namespace Example01.Tests.IntegrationTests;

public class SwaggerTests
{
    private const string SwaggerDocumentRoute = "swagger/v1/swagger.json";

    [Fact]
    public async Task Should_Get_Swagger_Document_Without_Api_Key()
    {
        // arrange
        var fixture = new WebApiTestsFixture();
        var client = fixture.CreateClient();
        client.DefaultRequestHeaders.Remove(ApiKeyConstants.ApiKeyHeaderName);

        // act
        var response = await client.GetAsync(SwaggerDocumentRoute);

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async Task Should_Describe_Movies_Routes_In_Swagger_Document()
    {
        // arrange
        var fixture = new WebApiTestsFixture();
        var client = fixture.CreateClient();

        // act
        using var document = await GetSwaggerDocumentAsync(client);

        // assert
        var paths = document.RootElement.GetProperty("paths");
        paths.TryGetProperty("/api/movies/list", out _).Should().BeTrue();
        paths.TryGetProperty("/api/movies/{movieId}", out _).Should().BeTrue();
    }

    [Fact]
    public async Task Should_Declare_Api_Key_Security_Scheme_In_Swagger_Document()
    {
        // arrange
        var fixture = new WebApiTestsFixture();
        var client = fixture.CreateClient();

        // act
        using var document = await GetSwaggerDocumentAsync(client);

        // assert
        var scheme = document.RootElement
            .GetProperty("components")
            .GetProperty("securitySchemes")
            .GetProperty(ApiKeyConstants.ApiKeyId);
        scheme.GetProperty("type").GetString().Should().Be("apiKey");
        scheme.GetProperty("name").GetString().Should().Be(ApiKeyConstants.ApiKeyHeaderName);
        scheme.GetProperty("in").GetString().Should().Be("header");
    }

    [Fact]
    public async Task Should_Require_Api_Key_In_Swagger_Document()
    {
        // arrange
        var fixture = new WebApiTestsFixture();
        var client = fixture.CreateClient();

        // act
        using var document = await GetSwaggerDocumentAsync(client);

        // assert
        var security = document.RootElement.GetProperty("security");
        security.EnumerateArray()
            .Any(requirement => requirement.TryGetProperty(ApiKeyConstants.ApiKeyId, out _))
            .Should().BeTrue();
    }

    private static async Task<JsonDocument> GetSwaggerDocumentAsync(HttpClient client)
    {
        var response = await client.GetAsync(SwaggerDocumentRoute);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        return JsonDocument.Parse(await response.Content.ReadAsStringAsync());
    }
}
