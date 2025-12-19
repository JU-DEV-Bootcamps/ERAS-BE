using Eras.Error;
using Eras.Error.Critical;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Eras.Api.Filters;

public class ErrorFilter : IActionFilter
{
    private readonly ILogger<Exception> _logger;

    public ErrorFilter(ILogger<Exception> Logger)
    {
        _logger = Logger;
    }

    private void LogMessage(Severity Severity, string Message)
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

    public void OnActionExecuted(ActionExecutedContext Context)
    {
        if (Context.Exception == null)
        {
            return;
        }

        IErasException? exception = (Context.Exception is IErasException)
            ? Context.Exception as IErasException
            : new CriticalException(Context.Exception);

        if (exception != null)
        {
            exception.OnLoggingException += LogMessage;
            exception.LogException();
            Context.Result = new ObjectResult(exception)
            {
                Value = exception.FriendlyMessage,
                StatusCode = exception?.StatusCode,
            };
        }
       
        Context.ExceptionHandled = true;
    }

    public void OnActionExecuting(ActionExecutingContext Context)
    { }
}
