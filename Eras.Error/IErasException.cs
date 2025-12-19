namespace Eras.Error;

public delegate void LogDelegate(Severity Severity, string FriendlyMessage);

public interface IErasException
{
    event LogDelegate OnLoggingException;
    int StatusCode { get; }
    string FriendlyMessage { get; }    
    void LogException();
}
