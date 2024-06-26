﻿namespace BIMdance.Revit.Utils.DependencyInjection;

public static class ServiceCollectionContainerBuilderExtensions
{
    /// <summary>
    /// Creates a <see cref="ServiceProvider"/> containing services from the provided <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> containing service descriptors.</param>
    /// <returns>The <see cref="ServiceProvider"/>.</returns>

    public static ServiceProvider BuildServiceProvider(this IServiceCollection services)
    {
        return BuildServiceProvider(services, ServiceProviderOptions.Default);
    }

    /// <summary>
    /// Creates a <see cref="ServiceProvider"/> containing services from the provided <see cref="IServiceCollection"/>
    /// optionaly enabling scope validation.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> containing service descriptors.</param>
    /// <param name="validateScopes">
    /// <c>true</c> to perform check verifying that scoped services never gets resolved from root provider; otherwise <c>false</c>.
    /// </param>
    /// <returns>The <see cref="ServiceProvider"/>.</returns>
    public static ServiceProvider BuildServiceProvider(this IServiceCollection services, bool validateScopes)
    {
        return services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = validateScopes });
    }

    /// <summary>
    /// Creates a <see cref="ServiceProvider"/> containing services from the provided <see cref="IServiceCollection"/>
    /// optionaly enabling scope validation.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> containing service descriptors.</param>
    /// <param name="options">
    /// Configures various service provider behaviors.
    /// </param>
    /// <returns>The <see cref="ServiceProvider"/>.</returns>
    public static ServiceProvider BuildServiceProvider(this IServiceCollection services, ServiceProviderOptions options)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        return new ServiceProvider(services, options);
    }
}