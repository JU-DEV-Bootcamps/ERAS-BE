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

    public void OnActionExecuted(ActionExecutedContext Context)
    {
        if (Context.Exception == null)
        {
            return;
        }

        IErasException? exception = (Context.Exception is IErasException)
            ? Context.Exception as IErasException
            : new CriticalException<Exception>(_logger, Context.Exception);

        exception?.LogException();

        Context.Result = new ObjectResult(exception)
        {
            Value = exception?.FriendlyMessage,
            StatusCode = exception?.StatusCode,            
        };

        Context.ExceptionHandled = true;
    }

    public void OnActionExecuting(ActionExecutingContext Context)
    { }
}
