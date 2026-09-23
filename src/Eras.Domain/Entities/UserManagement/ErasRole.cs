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
}