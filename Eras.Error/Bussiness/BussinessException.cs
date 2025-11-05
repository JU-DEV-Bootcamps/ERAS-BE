using Microsoft.Extensions.Logging;

namespace Eras.Error.Bussiness;
public class BussinessException<T> : ErasException<T>
{
    public BussinessException(ILogger<T> Logger, string FriendlyMessage, int StatusCode = 500)
        : base(Logger, FriendlyMessage, Severity.WARNING, StatusCode)
    { }

    public override void LogException()
    {
        LogMessage(FriendlyMessage);
    }
}
