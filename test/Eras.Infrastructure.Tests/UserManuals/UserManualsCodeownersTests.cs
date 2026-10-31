namespace Eras.Infrastructure.Tests.UserManuals;

public class UserManualsCodeownersTests
{
    [Fact]
    [Trait("Change", "640-add-user-manuals-page")]
    [Trait("AC", "640-add-user-manuals-page/AC-20")]
    public void ManualsFolder_RequiresDevelopersGroupAsOwner()
    {
        // Arrange (GIVEN) the manuals folder at the root of the main ERAS repository and its review rules
        var root = ParentRepository.Root();
        if (root is null)
            return; // only ERAS-BE is checked out: the parent repository pipeline runs this check

        var codeowners = Path.Combine(root, ".github", "CODEOWNERS");
        Assert.True(File.Exists(codeowners), ".github/CODEOWNERS is missing from the main ERAS repository.");

        // Act (WHEN) the manuals folder is checked for a required reviewer
        var rule = File.ReadAllLines(codeowners)
            .Select(Line => Line.Trim().Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries))
            .FirstOrDefault(Parts => Parts.Length > 1 && Parts[0] is "/manuals/" or "/manuals" or "/manuals/*");

        // Assert (THEN / AND) a developers group is required as owner of every file in it
        Assert.NotNull(rule);
        Assert.Contains(rule!.Skip(1), Owner => Owner.StartsWith('@'));
    }
}
