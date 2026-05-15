using Microsoft.Extensions.DependencyInjection;

namespace Gantry.NET;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddGantry(this IServiceCollection services, string connectionString)
        => AddGantry(services, new GantryOptions() 
        { 
            ConnectionString = connectionString 
        });

    public static IServiceCollection AddGantry(this IServiceCollection services, GantryOptions options)
    {
        _ = services.AddSingleton<IGantryClient>(new GantryClient(options));
        return services;
    }
}
