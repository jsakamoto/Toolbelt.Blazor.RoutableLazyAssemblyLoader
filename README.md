# Blazor Routable Lazy Assembly Loader [![NuGet Package](https://img.shields.io/nuget/v/Toolbelt.Blazor.RoutableLazyAssemblyLoader.svg)](https://www.nuget.org/packages/Toolbelt.Blazor.RoutableLazyAssemblyLoader/)

## Summary

This service and router component allows you to simplify lazy assembly loading for your Blazor WebAssembly app. 

Moreover, this library allows us to **avoid flickering** when first loading a page which is contained in a lazy loading assembly on pre-rendered Blazor WebAssembly apps.


## Supported Blazor versions

This library suppots ASP.NET Core Blazor WebAssembly version 6.0 or later.

## Basic Usage

### 1. Configure the project file of your Blazor WebAssembly app to enable lazy assembly loading

Please make sure the project file (.csproj) of your Blazor WebAssembly app is configured to enable lazy assembly loading. Please refer to the "Project file configuration" section of the following document for details.

_["Lazy load assemblies in ASP.NET Core Blazor WebAssembly | Microsoft Learn"](https://learn.microsoft.com/aspnet/core/blazor/webassembly-lazy-load-assemblies#project-file-configuration)_

Quoted from the above document below.

> Mark assemblies for lazy loading in the app's project file (.csproj) using the `BlazorWebAssemblyLazyLoad` item. Use the assembly name with the `.dll` extension. The Blazor framework prevents the assembly from loading at app launch.
>
> ```xml
> <ItemGroup>
>   <BlazorWebAssemblyLazyLoad Include="{ASSEMBLY NAME}.dll" />
> </ItemGroup>
> ```
>
> The `{ASSEMBLY NAME}` placeholder is the name of the assembly. The `.dll` file extension is required.
>
> Include one `BlazorWebAssemblyLazyLoad` item for each assembly. If an assembly has dependencies, include a BlazorWebAssemblyLazyLoad entry for each dependency.


### 2. Installation and Registration

**Step.1** Install the library via NuGet package, like this.

```shell
dotnet add package Toolbelt.Blazor.RoutableLazyAssemblyLoader
```

**Step.2** Register the "RoutableLazyAssemblyLoader" service into the DI container, including the mapping of "requested URL path -> assembly names to be loaded".

```csharp
// 📄Program.cs
using Toolbelt.Blazor.Extensions.DependencyInjection; // 👈 1. Add this line
...
// 👇 2. Add this code
builder.Services.AddRoutableLazyAssemblyLoader(path => path switch
{
    // ⚠️ Please implement your own logic to return the list of lazy assembly names 
    //    corresponding to each requested URL path.
    
    // This sample code shows that the routable Razor component required for the page
    // "https://.../counter" lives in the "CounterPage.dll" lazy loading assembly.
    "counter" => new[] { "CounterPage.dll" },

    // If nothing to load, just return null.
    _ => null
});
...
```

### 3. Use the `<LazyAssemblyLoadableRouter>` component instead of `<Router>` component

Replace the `<Router>` component with `<RoutableLazyAssemblyLoader>` component in your `App.razor` file.

```html
@* 📄App.razor *@

@using Toolbelt.Blazor.Components // 👈 1. Add this line
...
// 👇 2. Replace the <Router> component with <RoutableLazyAssemblyLoader> component.
<LazyAssemblyLoadableRouter AppAssembly="@typeof(App).Assembly">
    <Found Context="routeData">
        <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
        <FocusOnNavigate RouteData="@routeData" Selector="h1" />
    </Found>
    <NotFound>
        <PageTitle>Not found</PageTitle>
        <LayoutView Layout="@typeof(MainLayout)">
            <p role="alert">Sorry, there's nothing at this address.</p>
        </LayoutView>
    </NotFound>
</LazyAssemblyLoadableRouter>
```

### 4. Run it!

After the above steps, you can run your Blazor WebAssembly app as usual. In the sample code above, the "CounterPage.dll" lazy loading assembly won't be loaded initially and will be loaded when the user navigates to the "/counter" page.

## Prevent flickering on 1st time rendering of lazy loaded pages in pre-rendered Blazor WebAssembly apps.

If you use the pre-rendering feature of Blazor WebAssembly, you may notice that the lazy-loaded page flickers when it is first rendered. This is because the lazy-loaded page is rendered twice. The first time is when the page is rendered on the server side, and the second time is when the page is rendered on the client side. Usually, the time gap between 1st rendering and 2nd rendering will be almost 0, so nobody may see it flickers. However, when the page is lazy loaded, it might take a few hundred milliseconds to 2nd rendering because Blazor has to fetch the lazy assembly. That means a blank page will be shown during the loading of the lazy assembly. That is the reason for the flickering of the page in this scenario.

Fortunately, this library provides a solution to this problem. The solution is to preload the lazy assembly for the current URL before starting rendering. This library provides an extension method for the `WebAssemblyHost` class to do this. Please call the `PreloadRoutableLazyAssemblyAsync` extension method before calling the `RunAsync` method of the `WebAssemblyHost` instance, as below.

```csharp
// 📄Program.cs
...
var host = builder.Build();
// 👇 Preload lazy assembly for the current URL before starting rendering
//    by calling the "PreloadRoutableLazyAssemblyAsync" extension method.
await host.PreloadRoutableLazyAssemblyAsync();
await host.RunAsync();
```

After implementing the above code, flickering will be prevented.

![Movie - Before and After](https://raw.githubusercontent.com/jsakamoto/Toolbelt.Blazor.RoutableLazyAssemblyLoader/main/.assets/before-after-movie.gif)

## Acknowledgements

The idea to prevent flickering on 1st time rendering of lazy loaded pages in pre-rendered Blazor WebAssembly apps was **provided by [Connor Hallman](https://github.com/biegehydra)** in [his pull request #32](https://github.com/jsakamoto/BlazorWasmPreRendering.Build/pull/32) for the ["BlazorWasmPreRendering.Build" NuGet package project](https://github.com/jsakamoto/BlazorWasmPreRendering.Build). And when I asked him that may I instantiate his idea to a NuGet package by me, he readily agreed. Thank you, Connor, for your contributions!

## Release Note

[Release notes](https://github.com/jsakamoto/Toolbelt.Blazor.RoutableLazyAssemblyLoader/blob/main/RELEASE-NOTES.txt)

## License

[Mozilla Public License Version 2.0](https://github.com/jsakamoto/Toolbelt.Blazor.RoutableLazyAssemblyLoader/blob/main/LICENSE)
