using Eras.Domain.Entities.UserManagement;

using Xunit;

namespace Eras.Domain.Tests.Entities.UserManagement;

public class ErasRoleTests
{
    [Fact]
    public void Resolve_UsesDefaultKeycloakNames_WhenNoneProvided()
    {
        var result = ErasRole.Resolve(["ERAS Professional"]);

        Assert.Equal(ErasRole.Professional.Label, result);
    }

    [Fact]
    public void Resolve_PrioritizesAdministrator_OverOtherAssignedRoles()
    {
        var result = ErasRole.Resolve(["ERAS Student Services Officer", "ERAS Administrator"]);

        Assert.Equal(ErasRole.Administrator.Label, result);
    }

    [Fact]
    public void Resolve_ReturnsGuest_WhenNoRolesMatch()
    {
        var result = ErasRole.Resolve(["some-unrelated-role"]);

        Assert.Equal(ErasRole.Guest.Label, result);
    }

    [Fact]
    public void Resolve_UsesConfiguredRoleNames_ForAnEnvironmentWithDifferentKeycloakNaming()
    {
        var roleNames = new KeycloakRoleNames(
            Administrator: "admin",
            Officer: "Student Services Officer",
            Professional: "Professional");

        Assert.Equal(ErasRole.Administrator.Label, ErasRole.Resolve(["admin"], roleNames));
        Assert.Equal(ErasRole.Officer.Label, ErasRole.Resolve(["Student Services Officer"], roleNames));
        Assert.Equal(ErasRole.Professional.Label, ErasRole.Resolve(["Professional"], roleNames));
    }

    [Fact]
    public void Resolve_DoesNotMatchLocalDevRoleNames_WhenConfiguredWithDifferentNames()
    {
        var roleNames = new KeycloakRoleNames(
            Administrator: "admin",
            Officer: "Student Services Officer",
            Professional: "Professional");

        var result = ErasRole.Resolve(["ERAS Administrator"], roleNames);

        Assert.Equal(ErasRole.Guest.Label, result);
    }
}
