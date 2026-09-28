namespace Eras.Application.Contracts.Infrastructure;

public interface ICurrentUserService
{
    string? Sub { get; }
    string? Email { get; }
    string? FirstName { get; }
    string? LastName { get; }
    IReadOnlyCollection<string> Roles { get; }
}
