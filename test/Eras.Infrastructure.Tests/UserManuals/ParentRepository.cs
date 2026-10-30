namespace Eras.Infrastructure.Tests.UserManuals;

/// <summary>
/// Finds the parent ERAS repository (the one that holds <c>manuals/</c> and <c>.github/CODEOWNERS</c>) by walking up
/// from the test directory. It is null when only ERAS-BE is checked out, and the checks that need it do nothing then.
/// </summary>
internal static class ParentRepository
{
    public static string? Root()
    {
        for (DirectoryInfo? directory = new(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, ".gitmodules")))
                return directory.FullName;
        }

        return null;
    }

    public static string? ManualsFolder()
    {
        var configured = Environment.GetEnvironmentVariable("ERAS_MANUALS_PATH");
        if (!string.IsNullOrWhiteSpace(configured))
            return configured;

        var root = Root();
        return root is null ? null : Path.Combine(root, "manuals");
    }
}
