using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace BlazorWasmApp.Client;

public static class Startup
{
    public static void ConfigureRoutableLazyAssemblyLoader(IServiceCollection services)
    {
        services.AddRoutableLazyAssemblyLoader(path => path switch
        {
            "counter" => new[] { "CounterPage.dll" },
            _ => null
        });
    }
}
