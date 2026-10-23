namespace Eras.Domain.Entities.UserManagement;

/// <summary>
/// The raw Keycloak client-role names for each ERAS role, as configured for the
/// environment the app is running in. Every Keycloak instance ERAS talks to (local,
/// staging, production) can name its own roles differently — this is the one place
/// that translation happens, so the rest of the app only ever deals with the stable
/// internal <see cref="ErasRole"/> labels. Defaults match the local/dev Keycloak
/// instance's role names, so environments that don't configure this get today's
/// behavior unchanged.
/// </summary>
public sealed record KeycloakRoleNames(
    string Administrator = "ERAS Administrator",
    string Officer = "ERAS Student Services Officer",
    string Professional = "ERAS Professional");
