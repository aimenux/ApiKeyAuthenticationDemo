using Example03.Infrastructure.Repositories;

namespace Example03.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IMovieRepository, MovieRepository>();
        return services;
    }
}