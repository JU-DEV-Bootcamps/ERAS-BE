namespace Eras.Error.Bussiness;
public class NotFoundException : BussinessException
{
    public NotFoundException(string FriendlyMessage) 
        : base(FriendlyMessage, 404)
    { }
}
