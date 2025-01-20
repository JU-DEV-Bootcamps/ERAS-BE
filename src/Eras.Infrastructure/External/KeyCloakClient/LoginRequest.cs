namespace Eras.Infrastructure.External.KeycloakClient
{
    public class LoginRequest
    {
        required public string Username { get; set; }
        required public string Password { get; set; }
    }
}
