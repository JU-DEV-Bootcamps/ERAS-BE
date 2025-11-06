using Eras.Error.Properties;
using Microsoft.Extensions.Logging;

namespace Eras.Error.Critical;

public class CriticalException<T> : ErasException<T>
{
    public CriticalException(ILogger<T> Logger, Exception Exception)
          : base(Logger, Resources.GeneralMessage, Exception, Severity.ERROR, 500)
    { }

    public override void LogException()
    {
        var currentException = InnerException;
        do
        {
            LogMessage($"Message: {currentException.Message}. trace {currentException.StackTrace}");
            currentException = currentException.InnerException;
        } while (currentException != null);
    }
}
