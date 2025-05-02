namespace Eras.Application.Contracts.Infrastructure
{
    public interface IKeycloakAuthService<T>
    {
        Task<T> LoginAsync(string Username, string Password);
    }
}
