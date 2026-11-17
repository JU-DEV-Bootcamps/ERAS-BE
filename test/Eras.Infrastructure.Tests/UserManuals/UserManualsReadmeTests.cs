using System.Text.RegularExpressions;

namespace Eras.Infrastructure.Tests.UserManuals;

/// <summary>
/// Checks the developer guide in the manuals folder for the topics the spec lists, not for exact wording. Like the
/// CODEOWNERS check, it does nothing when only ERAS-BE is checked out.
/// </summary>
public class UserManualsReadmeTests
{

    private static string? ReadReadme()
    {
        var folder = ParentRepository.ManualsFolder();
        if (folder is null || !Directory.Exists(folder))
            return null; // only ERAS-BE is checked out: the parent repository pipeline runs this check

        var readme = Path.Combine(folder, "README.md");
        Assert.True(File.Exists(readme), "manuals/README.md is missing from the main ERAS repository.");
        return File.ReadAllText(readme);
    }

    [Fact]
    [Trait("Change", "687-distinguish-user-manuals-errors")]
    [Trait("AC", "687-distinguish-user-manuals-errors/AC-13")]
    public void Readme_GuidesTheLocalWorkflowStepByStep()
    {
        // Arrange (GIVEN) the manuals folder at the root of the main ERAS repository
        var readme = ReadReadme();
        if (readme is null)
            return;

        // Act (WHEN) its README is read
        var numberedSteps = Regex.Matches(readme, @"^\s*\d+\.\s", RegexOptions.Multiline).Count;

        // Assert (THEN / AND) numbered steps, the manifest fields, the types and the limit
        Assert.True(numberedSteps >= 6, "The README must give numbered steps (the Docker stack and the backend outside Docker).");
        foreach (var field in new[] { "`id`", "`title`", "`language`", "`version`", "`fileName`", "`format`" })
            Assert.Contains(field, readme);
        Assert.Contains("pdf", readme, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("odt", readme, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("10 MB", readme);

        // the read-only mount, and docker compose up only after the mount changes
        Assert.Contains("read-only", readme, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("without a rebuild", readme, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("docker compose up", readme);

        // the backend outside Docker with UserManuals__Path, and no personal path committed
        Assert.Contains("UserManuals__Path", readme);
        Assert.Contains("commit", readme, StringComparison.OrdinalIgnoreCase);

        // Visual Studio: a variable set in a terminal does not reach the debugger, so user-secrets is the main step
        Assert.Contains("Visual Studio", readme);
        Assert.Contains("does not reach", readme, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("dotnet user-secrets set \"UserManuals:Path\"", readme);

        // the validation with the user manuals tests
        Assert.Contains("dotnet test", readme);
        Assert.Contains("UserManuals", readme);
    }

    [Fact]
    [Trait("Change", "687-distinguish-user-manuals-errors")]
    [Trait("AC", "687-distinguish-user-manuals-errors/AC-25")]
    public void Readme_ExplainsEachErrorCode()
    {
        // Arrange (GIVEN) the manuals folder at the root of the main ERAS repository
        var readme = ReadReadme();
        if (readme is null)
            return;

        // Act (WHEN) its README is read, one line per error code
        string[] lines = readme.Split('\n');
        string[] codes = ["manuals_manifest_missing", "manuals_manifest_invalid", "manuals_file_missing", "manuals_not_found"];

        // Assert (THEN / AND) every code has a line with its cause and fix, and the missing manifest names the mount and the path
        foreach (var code in codes)
            Assert.Contains(lines, Line => Line.Contains(code) && Line.Length > code.Length + 20);

        var manifestMissing = lines.First(Line => Line.Contains("manuals_manifest_missing"));
        Assert.Contains("docker compose up", manifestMissing);
        Assert.Contains("UserManuals__Path", manifestMissing);
    }
}
