using Eras.Error.Properties;

namespace Eras.Error.Critical;

public class CriticalException : ErasException
{
    public CriticalException(Exception Exception, string FriendlyMessage = "")
          : base(string.IsNullOrEmpty(FriendlyMessage) ? Resources.GeneralMessage :  FriendlyMessage, Exception, Severity.ERROR, 500)
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
