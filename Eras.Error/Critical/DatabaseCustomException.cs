using Eras.Error.Properties;

namespace Eras.Error.Critical;

public class DatabaseCustomException: CriticalException
{
    public DatabaseCustomException(Exception InnerException) :
        base(InnerException, Resources.DatabaseExceptionMessage)
    { }
}
