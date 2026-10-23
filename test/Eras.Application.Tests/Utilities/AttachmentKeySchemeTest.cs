using Eras.Application.Utils;

namespace Eras.Application.Tests.Utilities;

public class AttachmentKeySchemeTest
{
    [Fact]
    public void BuildFolder_Should_CombineEntityTypeAndEntityId_WithSlashSeparator()
    {
        string folder = AttachmentKeyScheme.BuildFolder("interventions", 1);

        Assert.Equal("interventions/1", folder);
    }

    [Theory]
    [InlineData("Assessment", 42, "Assessment/42")]
    [InlineData("Student", 7, "Student/7")]
    public void BuildFolder_Should_BeEntityAgnostic(string entityType, int entityId, string expected)
    {
        string folder = AttachmentKeyScheme.BuildFolder(entityType, entityId);

        Assert.Equal(expected, folder);
    }
}
