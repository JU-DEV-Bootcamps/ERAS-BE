namespace Eras.Application.Contracts.Infrastructure;

public interface ICurrentUserService
{
    string? KeycloakSub { get; }
    string? Email { get; }
    string? FirstName { get; }
    string? LastName { get; } 
}