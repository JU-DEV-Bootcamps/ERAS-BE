namespace Eras.Application.Exceptions;
public class EntityNotFoundException : CustomException
{
    public EntityNotFoundException() { }

    public EntityNotFoundException(string Message) : base(Message) { }

    public EntityNotFoundException(string Message, Exception InnerException) :
        base(Message, InnerException)
    { }

    public EntityNotFoundException(string Message, int ErrorCode) : base(Message,ErrorCode){}

    public EntityNotFoundException(string Message, Exception InnerException, int ErrorCode) :
        base(Message, InnerException, ErrorCode){}
}
