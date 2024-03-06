namespace BIMdance.Revit.DependencyInjection;

/// <summary>
/// Options for configuring various behaviors of the default <see cref="IServiceProvider"/> implementation.
/// </summary>
public class ServiceProviderOptions
{
    // Avoid allocating objects in the default case
    internal static readonly ServiceProviderOptions Default = new();

    /// <summary>
    /// <c>true</c> to perform check verifying that scoped services never gets resolved from root provider; otherwise <c>false</c>.
    /// </summary>
    public bool ValidateScopes { get; set; }

    internal ServiceProviderMode Mode { get; set; } = ServiceProviderMode.Dynamic;
}