using Example04.Domain;
using Example04.Infrastructure.Repositories;
using AwesomeAssertions;

namespace Example04.Tests.UnitTests;

public class MovieRepositoryTests
{
    [Fact]
    public async Task Should_Not_Allow_Returned_Movies_To_Be_Cast_To_Mutable_List()
    {
        // arrange
        var repository = new MovieRepository();

        // act
        var movies = await repository.GetMoviesAsync(CancellationToken.None);

        // assert
        var act = () => (List<Movie>)movies;
        act.Should().Throw<InvalidCastException>();
    }

    [Fact]
    public async Task Should_Not_Allow_Mutating_Returned_Movies()
    {
        // arrange
        var repository = new MovieRepository();
        var movies = await repository.GetMoviesAsync(CancellationToken.None);
        var mutableView = (IList<Movie>)movies;

        // act
        var act = () => mutableView.Add(new Movie { Id = 3, Title = "Injected" });

        // assert
        act.Should().Throw<NotSupportedException>();
        movies.Should().HaveCount(2);
    }
}
