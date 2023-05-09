using Example01.Infrastructure.Repositories;

namespace Example01.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IMovieRepository, MovieRepository>();
        return services;
    }
}