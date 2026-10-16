namespace Eras.Application.Contracts.Infrastructure;

public interface ICurrentUserService
{
    string? Sub { get; }
    string? Email { get; }
    string? FirstName { get; }
    string? LastName { get; }
    string? Name { get; }
    IReadOnlyCollection<string> Roles { get; }
}
