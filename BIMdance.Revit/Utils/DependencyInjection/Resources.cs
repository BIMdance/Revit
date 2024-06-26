﻿// ReSharper disable UnusedMember.Local

namespace BIMdance.Revit.Utils.DependencyInjection;

internal static class Resources
{
    private static readonly ResourceManager ResourceManager = new("Microsoft.Extensions.DependencyInjection.Resources", typeof(Resources).GetTypeInfo().Assembly);

    /// <summary>
    /// Unable to activate type '{0}'. The following constructors are ambiguous:
    /// </summary>
    internal static string AmbiguousConstructorException => GetString("AmbiguousConstructorException");

    /// <summary>
    /// Unable to activate type '{0}'. The following constructors are ambiguous:
    /// </summary>
    internal static string FormatAmbiguousConstructorException(object p0) => string.Format(CultureInfo.CurrentCulture, GetString("AmbiguousConstructorException"), p0);

    /// <summary>
    /// Unable to resolve service for type '{0}' while attempting to activate '{1}'.
    /// </summary>
    internal static string CannotResolveService => GetString("CannotResolveService");

    /// <summary>
    /// Unable to resolve service for type '{0}' while attempting to activate '{1}'.
    /// </summary>
    internal static string FormatCannotResolveService(object p0, object p1) => string.Format(CultureInfo.CurrentCulture, GetString("CannotResolveService"), p0, p1);

    /// <summary>
    /// A circular dependency was detected for the service of type '{0}'.
    /// </summary>
    internal static string CircularDependencyException => GetString("CircularDependencyException");

    /// <summary>
    /// A circular dependency was detected for the service of type '{0}'.
    /// </summary>
    internal static string FormatCircularDependencyException(object p0) => string.Format(CultureInfo.CurrentCulture, GetString("CircularDependencyException"), p0);

    /// <summary>
    /// No constructor for type '{0}' can be instantiated using services from the service container and default values.
    /// </summary>
    internal static string UnableToActivateTypeException => GetString("UnableToActivateTypeException");

    /// <summary>
    /// No constructor for type '{0}' can be instantiated using services from the service container and default values.
    /// </summary>
    internal static string FormatUnableToActivateTypeException(object p0) => string.Format(CultureInfo.CurrentCulture, GetString("UnableToActivateTypeException"), p0);

    /// <summary>
    /// Open generic service type '{0}' requires registering an open generic implementation type.
    /// </summary>
    internal static string OpenGenericServiceRequiresOpenGenericImplementation => GetString("OpenGenericServiceRequiresOpenGenericImplementation");

    /// <summary>
    /// Open generic service type '{0}' requires registering an open generic implementation type.
    /// </summary>
    internal static string FormatOpenGenericServiceRequiresOpenGenericImplementation(object p0) => string.Format(CultureInfo.CurrentCulture, GetString("OpenGenericServiceRequiresOpenGenericImplementation"), p0);

    /// <summary>
    /// Cannot instantiate implementation type '{0}' for service type '{1}'.
    /// </summary>
    internal static string TypeCannotBeActivated => GetString("TypeCannotBeActivated");

    /// <summary>
    /// Cannot instantiate implementation type '{0}' for service type '{1}'.
    /// </summary>
    internal static string FormatTypeCannotBeActivated(object p0, object p1) => string.Format(CultureInfo.CurrentCulture, GetString("TypeCannotBeActivated"), p0, p1);

    /// <summary>
    /// A suitable constructor for type '{0}' could not be located. Ensure the type is concrete and services are registered for all parameters of a public constructor.
    /// </summary>
    internal static string NoConstructorMatch => GetString("NoConstructorMatch");

    /// <summary>
    /// A suitable constructor for type '{0}' could not be located. Ensure the type is concrete and services are registered for all parameters of a public constructor.
    /// </summary>
    internal static string FormatNoConstructorMatch(object p0) => string.Format(CultureInfo.CurrentCulture, GetString("NoConstructorMatch"), p0);

    /// <summary>
    /// Cannot consume {2} service '{0}' from {3} '{1}'.
    /// </summary>
    internal static string ScopedInSingletonException => GetString("ScopedInSingletonException");

    /// <summary>
    /// Cannot consume {2} service '{0}' from {3} '{1}'.
    /// </summary>
    internal static string FormatScopedInSingletonException(object p0, object p1, object p2, object p3) => string.Format(CultureInfo.CurrentCulture, GetString("ScopedInSingletonException"), p0, p1, p2, p3);

    /// <summary>
    /// Cannot resolve '{0}' from root provider because it requires {2} service '{1}'.
    /// </summary>
    internal static string ScopedResolvedFromRootException => GetString("ScopedResolvedFromRootException");

    /// <summary>
    /// Cannot resolve '{0}' from root provider because it requires {2} service '{1}'.
    /// </summary>
    internal static string FormatScopedResolvedFromRootException(object p0, object p1, object p2) => string.Format(CultureInfo.CurrentCulture, GetString("ScopedResolvedFromRootException"), p0, p1, p2);

    /// <summary>
    /// Cannot resolve {1} service '{0}' from root provider.
    /// </summary>
    internal static string DirectScopedResolvedFromRootException => GetString("DirectScopedResolvedFromRootException");

    /// <summary>
    /// Cannot resolve {1} service '{0}' from root provider.
    /// </summary>
    internal static string FormatDirectScopedResolvedFromRootException(object p0, object p1) => string.Format(CultureInfo.CurrentCulture, GetString("DirectScopedResolvedFromRootException"), p0, p1);

    // ReSharper disable once UnusedParameter.Local
    private static string GetString(string name, params string[] formatterNames)
    {
        return name;
        
        // var value = ResourceManager.GetString(name);
        //
        // Debug.Assert(value != null);
        //
        // if (formatterNames != null)
        // {
        //     for (var i = 0; i < formatterNames.Length; i++)
        //     {
        //         value = value.Replace("{" + formatterNames[i] + "}", "{" + i + "}");
        //     }
        // }
        //
        // return value;
    }
}