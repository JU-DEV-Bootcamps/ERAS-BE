namespace Eras.Error;
public interface IErasException
{
    int StatusCode { get; }
    string FriendlyMessage { get; }    
    void LogException();
}
