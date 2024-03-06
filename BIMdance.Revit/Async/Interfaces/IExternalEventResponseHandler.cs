namespace BIMdance.Revit.Async.Interfaces;

public interface IExternalEventResponseHandler<in TResponse>
{
    void Cancel();
    void SetResult(TResponse response);
    void ThrowException(Exception exception);
}