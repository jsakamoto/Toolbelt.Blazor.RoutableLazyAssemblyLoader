using BlazorWasmApp.Client;
using BlazorWasmApp.Client.Services;
using BlazorWasmApp.Shared;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Toolbelt.Blazor.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IWeatherForecast, ClientWeatherForecast>();
Startup.ConfigureRoutableLazyAssemblyLoader(builder.Services);

var host = builder.Build();
await host.PreloadRoutableLazyAssemblyAsync();
await host.RunAsync();

