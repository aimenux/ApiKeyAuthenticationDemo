using Example08.Domain;
using Example08.Infrastructure.Repositories;
using Example08.Presentation.Endpoints;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace Example08.Tests.UnitTests;

public class MoviesEndpointsTests
{
    [Fact]
    public async Task Should_Get_Movies_Returns_Success()
    {
        // arrange
        var repository = Substitute.For<IMovieRepository>();
        repository
            .GetMoviesAsync(Arg.Any<CancellationToken>())
            .Returns(new List<Movie>
            {
                new Movie
                {
                    Id = 1,
                    Title = "Matrix"
                }
            });
        var logger = NullLogger<MoviesEndpoints>.Instance;
        var endpoints = new MoviesEndpoints(repository, logger);

        // act
        var result = await endpoints.GetMoviesAsync(CancellationToken.None);

        // assert
        result.Should().BeOfType<Ok<IEnumerable<Movie>>>();
        result
            .As<Ok<IEnumerable<Movie>>>().Value
            .As<IEnumerable<Movie>>()
            .Should().NotBeNullOrEmpty();
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task Should_Get_Movie_By_Id_Returns_Success(int movieId)
    {
        // arrange
        var repository = Substitute.For<IMovieRepository>();
        repository
            .GetMovieByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(new Movie
            {
                Id = 1,
                Title = "Matrix"
            });
        var logger = NullLogger<MoviesEndpoints>.Instance;
        var endpoints = new MoviesEndpoints(repository, logger);

        // act
        var result = await endpoints.GetMovieByIdAsync(movieId, CancellationToken.None);

        // assert
        result.Should().BeOfType<Ok<Movie>>();
        result
            .As<Ok<Movie>>().Value
            .As<Movie>()
            .Should().NotBeNull();
    }    
}