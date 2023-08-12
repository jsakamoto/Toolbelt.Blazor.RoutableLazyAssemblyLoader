using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.WebAssembly.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Toolbelt.Blazor.Services;

/// <summary>
/// Provides a service for loading assemblies that include routable components at runtime in a browser context.
/// </summary>
public class RoutableLazyAssemblyLoader
{
    private readonly LazyAssemblyLoader AssemblyLoader;

    private readonly NavigationManager NavigationManager;

    private readonly ILogger<RoutableLazyAssemblyLoader> Logger;

    private readonly Func<string, IEnumerable<string>?> GetAssemblyNameFromUrlPath;

    private readonly List<Assembly> _LoadedAssemblies = new List<Assembly>();

    /// <summary>
    /// Gets the loaded assemblies by this <see cref="RoutableLazyAssemblyLoader"/>.
    /// </summary>
    public IEnumerable<Assembly> LoadedAssemblies => this._LoadedAssemblies;

    /// <summary>
    /// Initializes a new instance of <see cref="RoutableLazyAssemblyLoader"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceProvider"/>.</param>
    /// <param name="getAssemblyNameFromUrlPath">A function that returns the names of assembly that need to load for the URL path in the argument.</param>
    public RoutableLazyAssemblyLoader(IServiceProvider services, Func<string, IEnumerable<string>?> getAssemblyNameFromUrlPath)
    {
        this.AssemblyLoader = services.GetRequiredService<LazyAssemblyLoader>();
        this.NavigationManager = services.GetRequiredService<NavigationManager>();
        this.Logger = services.GetRequiredService<ILogger<RoutableLazyAssemblyLoader>>();
        this.GetAssemblyNameFromUrlPath = getAssemblyNameFromUrlPath;
    }

    /// <summary>
    /// Handles the <see cref="Router.OnNavigateAsync"/> event.
    /// </summary>
    /// <param name="args">The <see cref="NavigationContext"/> that contains the URL path.</param>
    public Task OnNavigateAsync(NavigationContext args) => this.LoadLazyAssembliesAsync(args.Path).AsTask();

    /// <summary>
    /// Loads the assemblies that include routable components for the specified URL path.
    /// </summary>
    /// <param name="path">The URL path trimmed root slash (ex. "home/index").</param>
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
    private async ValueTask LoadLazyAssembliesAsync(string path)
    {
        try
        {
            var assemblyNames = this.GetAssemblyNameFromUrlPath(path) ?? Enumerable.Empty<string>();
            if (!assemblyNames.Any()) return;
            var assemblies = await this.AssemblyLoader.LoadAssembliesAsync(assemblyNames);
            this._LoadedAssemblies.AddRange(assemblies);
        }
        catch (Exception ex) { this.Logger.LogError(ex, ex.Message); }
    }

    /// <summary>
    /// Preloads the assemblies that include routable components for the current URL path.
    /// </summary>
    public async ValueTask PreloadRoutableLazyAssemblyAsync()
    {
        var uri = new Uri(this.NavigationManager.Uri);
        var path = uri.PathAndQuery.TrimStart('/');
        await this.LoadLazyAssembliesAsync(path);
    }
}
