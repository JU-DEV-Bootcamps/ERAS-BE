using System.Diagnostics.CodeAnalysis;

namespace Eras.Infrastructure.External.KeycloakClient
{
    [ExcludeFromCodeCoverage]
    public class LoginRequest
    {
        required public string Username { get; set; }
        required public string Password { get; set; }
    }
}
