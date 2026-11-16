namespace Eras.Error;

/// <summary>
/// An exception that tells the client why it failed with a stable, machine-readable code, besides the
/// friendly message. The API puts the code in the body of the error response.
/// </summary>
public interface IErrorCodeException : IErasException
{
    string ErrorCode { get; }
}
