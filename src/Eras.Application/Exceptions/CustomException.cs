namespace Eras.Application.Exceptions;
public class CustomException : Exception
{
    public int ErrorCode { get; set; }
    public CustomException() { }

    public CustomException(string Message) : base(Message) { }

    public CustomException(string Message, Exception InnerException) :
        base(Message, InnerException) { }

    public CustomException(string Message, int ErrorCode) : base(Message) {
        this.ErrorCode = ErrorCode;
    }

    public CustomException(string Message, Exception InnerException, int ErrorCode):
        base(Message, InnerException)
    {
        this.ErrorCode = ErrorCode;
    }
}
