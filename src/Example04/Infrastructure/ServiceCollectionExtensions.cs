using Example04.Infrastructure.Repositories;

namespace Example04.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IMovieRepository, MovieRepository>();
        return services;
    }
}