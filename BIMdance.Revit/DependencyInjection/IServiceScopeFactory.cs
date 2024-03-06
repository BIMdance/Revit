namespace BIMdance.Revit.DependencyInjection;

public interface IServiceScopeFactory
{
    /// <summary>
    /// Create an <see cref="IServiceScope"/> which
    /// contains an <see cref="System.IServiceProvider"/> used to resolve dependencies from a
    /// newly created scope.
    /// </summary>
    /// <returns>
    /// An <see cref="IServiceScope"/> controlling the
    /// lifetime of the scope. Once this is disposed, any scoped services that have been resolved
    /// from the <see cref="ServiceProvider"/>
    /// will also be disposed.
    /// </returns>
    IServiceScope CreateScope();
}