using System.Reflection;

using Eras.Api.Controllers;

using Xunit;

namespace Eras.Api.Tests.Authorization;

public class ProfessionalProfilePolicyTests
{
    private const string Administrator = "ERAS Administrator";
    private const string Officer = "ERAS Student Services Officer";
    private const string Professional = "ERAS Professional";

    private static readonly string[] ProfessionalProfileActions =
    [
        "GetMyProfessionalProfileAsync",
        "UpdateMyProfessionalProfileAsync",
    ];

    [Fact]
    [Trait("Change", "603-persist-professional-profile")]
    [Trait("AC", "603-persist-professional-profile/AC-5")]
    public async Task ProfessionalProfile_OfficerIsDenied_OnReadAndUpdate()
    {
        // Arrange (GIVEN) a user with the role "ERAS Student Services Officer"
        // Act (WHEN) they request or submit a professional profile
        // Assert (THEN) each request is denied (403)
        foreach (string name in ProfessionalProfileActions)
        {
            MethodInfo action = ControllerPolicyEvaluator.Action(typeof(UsersController), name);
            Assert.False(await ControllerPolicyEvaluator.IsAuthorizedAsync(action, Officer), $"{name} must be denied to an Officer");
        }
    }

    [Fact]
    [Trait("Change", "603-persist-professional-profile")]
    [Trait("AC", "603-persist-professional-profile/AC-6")]
    public async Task ProfessionalProfile_GuestIsDenied()
    {
        // Arrange (GIVEN) an authenticated user with no ERAS role
        // Act (WHEN) they request a professional profile
        // Assert (THEN) the request is denied (403)
        MethodInfo action = ControllerPolicyEvaluator.Action(typeof(UsersController), "GetMyProfessionalProfileAsync");
        Assert.False(await ControllerPolicyEvaluator.IsAuthorizedAsync(action));
    }

    [Fact]
    [Trait("Change", "603-persist-professional-profile")]
    [Trait("AC", "603-persist-professional-profile/AC-20")]
    public async Task ProfessionalProfile_AdministratorIsDenied()
    {
        // Arrange (GIVEN) a user with the role "ERAS Administrator"
        // Act (WHEN) they request or submit a professional profile
        // Assert (THEN) each request is denied (403)
        foreach (string name in ProfessionalProfileActions)
        {
            MethodInfo action = ControllerPolicyEvaluator.Action(typeof(UsersController), name);
            Assert.False(await ControllerPolicyEvaluator.IsAuthorizedAsync(action, Administrator), $"{name} must be denied to an Administrator");
        }
    }

    [Fact]
    public async Task ProfessionalProfile_ProfessionalIsAllowed_OnReadAndUpdate()
    {
        foreach (string name in ProfessionalProfileActions)
        {
            MethodInfo action = ControllerPolicyEvaluator.Action(typeof(UsersController), name);
            Assert.True(await ControllerPolicyEvaluator.IsAuthorizedAsync(action, Professional), $"{name} must be allowed to a Professional");
        }
    }
}
