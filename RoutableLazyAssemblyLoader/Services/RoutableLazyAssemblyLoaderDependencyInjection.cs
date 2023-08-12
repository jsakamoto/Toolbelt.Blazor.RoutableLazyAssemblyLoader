using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Toolbelt.Blazor.Services;

namespace Toolbelt.Blazor.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to add a <see cref="RoutableLazyAssemblyLoader"/> service.
/// </summary>
public static class RoutableLazyAssemblyLoaderDependencyInjection
{
    /// <summary>
    /// Adds a <see cref="RoutableLazyAssemblyLoader"/> service to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="RoutableLazyAssemblyLoader"/> service to.</param>
    /// <param name="getAssemblyNameFromUrlPath">A function that returns the names of assembly that need to load for the URL path in the argument.</param>
    public static IServiceCollection AddRoutableLazyAssemblyLoader(this IServiceCollection services, Func<string, IEnumerable<string>?> getAssemblyNameFromUrlPath)
    {
        services.TryAddScoped<LazyAssemblyLoader>();
        return services.AddScoped(sp => new RoutableLazyAssemblyLoader(sp, getAssemblyNameFromUrlPath));
    }

    /// <summary>
    /// Preloads the assemblies that include routable components for the current URL path.
    /// </summary>
    /// <param name="host">The <see cref="WebAssemblyHost"/> that contains the <see cref="RoutableLazyAssemblyLoader"/> service.</param>
    public static async ValueTask PreloadRoutableLazyAssemblyAsync(this WebAssemblyHost host)
    {
        var assemblyLoader = host.Services.GetRequiredService<RoutableLazyAssemblyLoader>();
        await assemblyLoader.PreloadRoutableLazyAssemblyAsync();
    }
}
