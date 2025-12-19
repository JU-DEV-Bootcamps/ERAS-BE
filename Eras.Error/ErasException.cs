namespace Eras.Error;

public abstract class ErasException : Exception, IErasException
{    
    public event LogDelegate OnLoggingException;
    public int StatusCode { get; protected set; }
    public string FriendlyMessage { get; protected set; }
    public new Exception InnerException { get; set; }
    protected Severity Severity { get; }
    
    protected ErasException(
        string FriendlyMessage, 
        Severity Severity,
        int StatusCode)
    {
        this.FriendlyMessage = FriendlyMessage;
        this.Severity = Severity;
        this.StatusCode = StatusCode;
    }

    protected ErasException(
        string FriendlyMessage,
        Exception InnerException,
        Severity Severity,
        int StatusCode) 
        : this(FriendlyMessage, Severity, StatusCode)
    {
        this.InnerException = InnerException;
    }

    protected void LogMessage(string Message)
    {
        OnLoggingException?.Invoke(Severity, Message);
    }

    public abstract void LogException();
}
