using Eras.Api.Controllers;

using Xunit;

namespace Eras.Api.Tests.Authorization;

public class StudentInterventionPoliciesTests
{
    private const string Administrator = "ERAS Administrator";
    private const string Officer = "ERAS Student Services Officer";
    private const string Professional = "ERAS Professional";

    private static readonly string[] WriteActions = ["CreateAsync", "UpdateAsync", "DeleteAsync"];

    [Fact]
    [Trait("Change", "643-add-student-interventions")]
    [Trait("AC", "643-add-student-interventions/AC-18")]
    public async Task List_GuestIsDeniedAsync()
    {
        // Arrange (GIVEN) a user with no ERAS role (Guest)
        var list = ControllerPolicyEvaluator.Action(typeof(StudentInterventionsController), "GetByStudentAsync");

        // Act (WHEN) they request the entries of a student
        bool allowed = await ControllerPolicyEvaluator.IsAuthorizedAsync(list);

        // Assert (THEN) the action is denied (403 from the authorization pipeline)
        Assert.False(allowed);
    }

    [Fact]
    [Trait("Change", "643-add-student-interventions")]
    [Trait("AC", "643-add-student-interventions/AC-19")]
    public async Task CreateUpdateDelete_OfficerIsDeniedAsync()
    {
        // Arrange (GIVEN) an ERAS Student Services Officer
        // Act (WHEN) they try to add an entry and to edit and delete an existing one
        // Assert (THEN) each action is denied
        foreach (string name in WriteActions)
        {
            var action = ControllerPolicyEvaluator.Action(typeof(StudentInterventionsController), name);
            Assert.False(await ControllerPolicyEvaluator.IsAuthorizedAsync(action, Officer), $"{name} must be denied to an Officer");
        }
    }

    [Theory]
    [InlineData(Administrator)]
    [InlineData(Professional)]
    public async Task CreateUpdateDelete_AdministratorAndProfessionalAreAllowedAsync(string Role)
    {
        foreach (string name in WriteActions)
        {
            var action = ControllerPolicyEvaluator.Action(typeof(StudentInterventionsController), name);
            Assert.True(await ControllerPolicyEvaluator.IsAuthorizedAsync(action, Role), $"{name} must be allowed to {Role}");
        }
    }

    [Theory]
    [InlineData(Administrator)]
    [InlineData(Officer)]
    [InlineData(Professional)]
    public async Task List_EveryErasRoleIsAllowedAsync(string Role)
    {
        var list = ControllerPolicyEvaluator.Action(typeof(StudentInterventionsController), "GetByStudentAsync");

        Assert.True(await ControllerPolicyEvaluator.IsAuthorizedAsync(list, Role));
    }

    [Fact]
    public async Task WriteActions_GuestIsDeniedAsync()
    {
        foreach (string name in WriteActions)
        {
            var action = ControllerPolicyEvaluator.Action(typeof(StudentInterventionsController), name);
            Assert.False(await ControllerPolicyEvaluator.IsAuthorizedAsync(action), $"{name} must be denied to a Guest");
        }
    }
}
