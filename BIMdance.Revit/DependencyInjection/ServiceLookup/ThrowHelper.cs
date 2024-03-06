namespace BIMdance.Revit.DependencyInjection.ServiceLookup;

internal class ThrowHelper
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void ThrowObjectDisposedException()
    {
        throw new ObjectDisposedException(nameof(IServiceProvider));
    }
}