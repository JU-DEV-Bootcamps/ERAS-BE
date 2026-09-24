namespace Eras.Infrastructure.External.KeycloakClient;
public class KeycloakClaims
{
    public required string Sub { get; init; }
    public required string Email { get; init; }
    public required string GivenName { get; init; }
    public required string FamilyName { get; init; }
    public required Dictionary<string, Resource> ResourceAccess { get; init; }
}

public class Resource
{
    public required string[] roles {get; init;} 
}