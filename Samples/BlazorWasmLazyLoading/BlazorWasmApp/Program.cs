using BlazorWasmApp;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Toolbelt.Blazor.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
if (!builder.RootComponents.Any())
{
    builder.RootComponents.Add<App>("#app");
    builder.RootComponents.Add<HeadOutlet>("head::after");
}

ConfigureServices(builder.Services, builder.HostEnvironment.BaseAddress);

var host = builder.Build();

await host.PreloadRoutableLazyAssemblyAsync();
Console.WriteLine("M-X: Preloaded");

await host.RunAsync();

static void ConfigureServices(IServiceCollection services, string baseAddress)
{
    services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(baseAddress) });

    //services.AddRoutableLazyAssemblyLoader(path => path switch
    //{
    //    "counter" => ["CounterPage.wasm"],
    //    _ => null
    //});
    services.AddRoutableLazyAssemblyLoader(path =>
    {
        Console.WriteLine($"M-0: Checking path: \"{path}\"");
        if (path == "counter")
        {
            Console.WriteLine("M-1: Loading CounterPage.wasm");
            return ["CounterPage.wasm"];
        }
        Console.WriteLine("M-2: No matching path found");
        return null;
    });
}