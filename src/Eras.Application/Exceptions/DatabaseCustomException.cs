namespace Eras.Application.Exceptions;
public class DatabaseCustomException: CustomException
{
    public DatabaseCustomException() { }

    public DatabaseCustomException(string Message) : base(Message) { }

    public DatabaseCustomException(string Message, Exception InnerException) :
        base(Message, InnerException)
    { }

    public DatabaseCustomException(string Message, int ErrorCode) : base(Message, ErrorCode) { }

    public DatabaseCustomException(string Message, Exception InnerException, int ErrorCode) :
        base(Message, InnerException, ErrorCode)
    { }
}
