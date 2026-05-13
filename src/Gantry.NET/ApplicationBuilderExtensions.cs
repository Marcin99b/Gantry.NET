using Microsoft.AspNetCore.Builder;

namespace Gantry.NET;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseGantry(this IApplicationBuilder applicationBuilder)
    {
        return applicationBuilder;
    }
}
