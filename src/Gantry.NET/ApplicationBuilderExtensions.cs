using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Gantry.NET;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseGantry(this IApplicationBuilder applicationBuilder, Action<IGantryClient> action)
    {
        var client = applicationBuilder.ApplicationServices.GetRequiredService<IGantryClient>();
        action(client);
        return applicationBuilder;
    }
}
