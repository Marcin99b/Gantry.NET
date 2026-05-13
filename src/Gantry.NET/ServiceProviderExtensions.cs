namespace Gantry.NET;

public static class ServiceProviderExtensions
{
    public static IServiceProvider AddGantry(this IServiceProvider serviceProvider, string connectionString)
        => AddGantry(serviceProvider, new GantryOptions() 
        { 
            ConnectionString = connectionString 
        });

    public static IServiceProvider AddGantry(this IServiceProvider serviceProvider, GantryOptions options)
    {
        return serviceProvider;
    }
}
