namespace Eras.Domain.Entities.UserManagement;
public sealed class ErasRole
{
    public static readonly ErasRole Administrator = new ("ERAS Administrator");
    public static readonly ErasRole Officer = new ("ERAS Student Services Officer");
    public static readonly ErasRole Professional = new ("ERAS Professional");
    public static readonly ErasRole Guest = new ("ERAS Guest");

    public string Label { get; }

    private ErasRole(string label)
    {
        Label = label;
    }

    public static IEnumerable<string> ListLabels() => [Administrator.Label, Officer.Label, Professional.Label, Guest.Label];

    /// <summary>
    /// Resolves the effective ERAS role from a set of raw Keycloak client role names,
    /// prioritizing Administrator over any other assigned role. <paramref name="RoleNames"/>
    /// maps each ERAS role to the raw name it has in the Keycloak instance for the current
    /// environment; when omitted, it defaults to the local/dev instance's names.
    /// </summary>
    public static string Resolve(IEnumerable<string> Roles, KeycloakRoleNames? RoleNames = null)
    {
        var names = RoleNames ?? new KeycloakRoleNames();
        var roleList = Roles as ICollection<string> ?? Roles.ToList();

        if (roleList.Contains(names.Administrator))
            return Administrator.Label;

        if (roleList.Contains(names.Officer))
            return Officer.Label;

        if (roleList.Contains(names.Professional))
            return Professional.Label;

        return Guest.Label;
    }
}