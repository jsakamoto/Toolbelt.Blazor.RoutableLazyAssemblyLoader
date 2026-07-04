using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.CompilerServices;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Routing;
using Toolbelt.Blazor.Services;
namespace Toolbelt.Blazor.Components;

/// <summary>
/// A router component that supports lazy loading of routerble assemblies with the <see cref="Toolbelt.Blazor.Services.RoutableLazyAssemblyLoader"/> service.<br/>
/// This component supplies route data corresponding to the current navigation state.
/// </summary>
public class LazyAssemblyLoadableRouter : ComponentBase
{
    [Inject]
    protected RoutableLazyAssemblyLoader AssemblyLoader { get; init; } = default!;

    /// <summary>
    /// Gets or sets the assembly that should be searched for components matching the URI.
    /// </summary>
    [Parameter, EditorRequired]
    public Assembly AppAssembly { get; set; } = null!;

    /// <summary>
    /// Gets or sets a collection of additional assemblies that should be searched for components
    /// that can match URIs.
    /// </summary>
    [Parameter] public IEnumerable<Assembly>? AdditionalAssemblies { get; set; }

    /// <summary>
    /// Gets or sets the content to display when no match is found for the requested route.
    /// </summary>
    [Parameter, EditorRequired]
#if NET10_0_OR_GREATER
    [Obsolete("NotFound is deprecated. Use NotFoundPage instead.")]
#endif
    public RenderFragment NotFound { get; set; } = null!;

    /// <summary>
    /// Gets or sets the content to display when a match is found for the requested route.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment<RouteData> Found { get; set; } = null!;

    /// <summary>
    /// Get or sets the content to display when asynchronous navigation is in progress.
    /// </summary>
    [Parameter]
    public RenderFragment? Navigating { get; set; }

    /// <summary>
    /// Gets or sets a handler that should be called before navigating to a new page.
    /// </summary>
    [Parameter]
    public EventCallback<NavigationContext> OnNavigateAsync { get; set; }

    /// <summary>
    /// Gets or sets a flag to indicate whether route matching should prefer exact matches
    /// over wildcards.
    /// <para>This property is obsolete and configuring it does nothing.</para>
    /// </summary>
    [Parameter]
#if NET8_0_OR_GREATER
    [Obsolete("This property is obsolete and configuring it has no effect.")]
#endif
    public bool PreferExactMatches { get; set; }

#if NET10_0_OR_GREATER
    /// <summary>
    /// Gets or sets the page content to display when no match is found for the requested route.
    /// </summary>
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    [Parameter]
    public Type? NotFoundPage { get; set; }
#endif

    private IEnumerable<Assembly> MergedAdditionalAssemblies => this.AssemblyLoader.LoadedAssemblies.Concat(this.AdditionalAssemblies ?? Enumerable.Empty<Assembly>());

#pragma warning disable CS0618 // Type or member is obsolete
    protected override void BuildRenderTree(RenderTreeBuilder __builder)
    {
#pragma warning disable IL2111 // Method with parameters or return value with `DynamicallyAccessedMembersAttribute` is accessed via reflection. Trimmer can't guarantee availability of the requirements of the method.
        __builder.OpenComponent<Router>(0);
#pragma warning restore IL2111 // Method with parameters or return value with `DynamicallyAccessedMembersAttribute` is accessed via reflection. Trimmer can't guarantee availability of the requirements of the method.

#if NET10_0_OR_GREATER
        __builder.AddComponentParameter(1, nameof(Router.AppAssembly), RuntimeHelpers.TypeCheck(this.AppAssembly));
        __builder.AddComponentParameter(2, nameof(Router.AdditionalAssemblies), RuntimeHelpers.TypeCheck(this.MergedAdditionalAssemblies));
        __builder.AddComponentParameter(3, nameof(Router.NotFound), this.NotFound);
        __builder.AddComponentParameter(4, nameof(Router.Found), this.Found);
        __builder.AddComponentParameter(5, nameof(Router.Navigating), this.Navigating);
        __builder.AddComponentParameter(6, nameof(Router.OnNavigateAsync), RuntimeHelpers.TypeCheck(EventCallback.Factory.Create((object)this, (Func<NavigationContext, Task>)this.OnNavigateInternalAsync)));
        __builder.AddComponentParameter(7, nameof(Router.PreferExactMatches), RuntimeHelpers.TypeCheck(this.PreferExactMatches));
        __builder.AddComponentParameter(8, nameof(Router.NotFoundPage), RuntimeHelpers.TypeCheck(this.NotFoundPage));
#elif NET8_0_OR_GREATER
        __builder.AddComponentParameter(1, nameof(Router.AppAssembly), RuntimeHelpers.TypeCheck(this.AppAssembly));
        __builder.AddComponentParameter(2, nameof(Router.AdditionalAssemblies), RuntimeHelpers.TypeCheck(this.MergedAdditionalAssemblies));
        __builder.AddComponentParameter(3, nameof(Router.NotFound), this.NotFound);
        __builder.AddComponentParameter(4, nameof(Router.Found), this.Found);
        __builder.AddComponentParameter(5, nameof(Router.Navigating), this.Navigating);
        __builder.AddComponentParameter(6, nameof(Router.OnNavigateAsync), RuntimeHelpers.TypeCheck(EventCallback.Factory.Create((object)this, (Func<NavigationContext, Task>)this.OnNavigateInternalAsync)));
        __builder.AddComponentParameter(7, nameof(Router.PreferExactMatches), RuntimeHelpers.TypeCheck(this.PreferExactMatches));
#else
        __builder.AddAttribute(1, nameof(Router.AppAssembly), RuntimeHelpers.TypeCheck(this.AppAssembly));
        __builder.AddAttribute(2, nameof(Router.AdditionalAssemblies), RuntimeHelpers.TypeCheck(this.MergedAdditionalAssemblies));
        __builder.AddAttribute(3, nameof(Router.NotFound), (object?)this.NotFound);
        __builder.AddAttribute(4, nameof(Router.Found), (object?)this.Found);
        __builder.AddAttribute(5, nameof(Router.Navigating), (object?)this.Navigating);
        __builder.AddAttribute(6, nameof(Router.OnNavigateAsync), (object?)RuntimeHelpers.TypeCheck(EventCallback.Factory.Create((object)this, (Func<NavigationContext, Task>)this.OnNavigateInternalAsync)));
        __builder.AddAttribute(7, nameof(Router.PreferExactMatches), (object?)RuntimeHelpers.TypeCheck(this.PreferExactMatches));
#endif
        __builder.CloseComponent();
    }
#pragma warning restore CS0618 // Type or member is obsolete

    private async Task OnNavigateInternalAsync(NavigationContext context)
    {
        await this.AssemblyLoader.OnNavigateAsync(context);
        await this.OnNavigateAsync.InvokeAsync(context);
    }
}
