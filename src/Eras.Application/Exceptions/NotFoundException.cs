namespace Eras.Application.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string Message)
            : base(Message)
        {
        }
    }
}
