namespace Eras.Error.Bussiness;
public class BussinessException : ErasException
{
    public BussinessException(string FriendlyMessage, int StatusCode = 500)
        : base(FriendlyMessage, Severity.WARNING, StatusCode)
    { }

    public override void LogException()
    {
        LogMessage(FriendlyMessage);
    }
}
