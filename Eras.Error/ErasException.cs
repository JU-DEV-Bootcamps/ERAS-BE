using Microsoft.Extensions.Logging;

namespace Eras.Error;

public abstract class ErasException<T> : Exception, IErasException
{
    private readonly ILogger<T> _logger;

    public int StatusCode { get; protected set; }
    public string FriendlyMessage { get; protected set; }
    public new Exception InnerException { get; set; }
    protected Severity Severity { get; }
    
    protected ErasException(
        ILogger<T> Logger,
        string FriendlyMessage, 
        Severity Severity,
        int StatusCode)
    {
        this.FriendlyMessage = FriendlyMessage;
        this.Severity = Severity;
        this.StatusCode = StatusCode;
        _logger = Logger;
    }

    protected ErasException(
        ILogger<T> Logger,
        string FriendlyMessage,
        Exception InnerException,
        Severity Severity,
        int StatusCode) 
        : this(Logger, FriendlyMessage, Severity, StatusCode)
    {
        this.InnerException = InnerException;
    }

    protected void LogMessage(string Message)
    {
        switch (Severity)
        {
            case Severity.DEBUG:
                _logger.LogDebug(Message); 
                break;
            case Severity.INFORMATION:
                _logger.LogInformation(Message);
                break;
            case Severity.WARNING: 
                _logger.LogWarning(Message);
                break;
            case Severity.ERROR:
                _logger.LogError(Message);
                break;
            case Severity.FATAL: 
                _logger.LogCritical(Message);
                break;
        }
    }

    public abstract void LogException();
}
