namespace BIMdance.Revit.Utils.DependencyInjection;

public interface IServiceScope : IDisposable
{
    /// <summary>
    /// The <see cref="System.IServiceProvider"/> used to resolve dependencies from the scope.
    /// </summary>
    IServiceProvider ServiceProvider { get; }
}