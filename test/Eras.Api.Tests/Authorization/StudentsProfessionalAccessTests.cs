using System.Reflection;

using Eras.Api.Controllers;
using Eras.Infrastructure.Authorization;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

using Xunit;

namespace Eras.Api.Tests.Authorization;

public class StudentsProfessionalAccessTests
{
    private const string Administrator = "ERAS Administrator";
    private const string Officer = "ERAS Student Services Officer";
    private const string Professional = "ERAS Professional";

    private static readonly string[] ProfessionalReadActions = ["GetAllAsync", "GetStudentProfileAsync"];

    private static IEnumerable<MethodInfo> StudentActions() =>
        typeof(StudentsController).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(Method => Method.GetCustomAttributes<HttpMethodAttribute>().Any());

    [Fact]
    [Trait("Change", "643-add-student-interventions")]
    [Trait("AC", "643-add-student-interventions/AC-23")]
    public async Task Professional_IsDeniedImportCreateDeleteAndProfileUpdateAsync()
    {
        // Arrange (GIVEN) an ERAS Professional
        string[] denied = ["ImportStudentsAsync", "CreateManualStudentAsync", "DeleteStudentAsync", "UpdateStudentProfileAsync"];

        // Act (WHEN) they send a request to import, create, delete a student or edit a student's personal profile
        // Assert (THEN) each request is denied (403)
        foreach (string name in denied)
        {
            var action = ControllerPolicyEvaluator.Action(typeof(StudentsController), name);
            Assert.False(await ControllerPolicyEvaluator.IsAuthorizedAsync(action, Professional), $"{name} must be denied to a Professional");
        }
    }

    [Theory]
    [InlineData(Administrator)]
    [InlineData(Officer)]
    [InlineData(Professional)]
    public async Task ListAndProfileRead_EveryErasRoleIsAllowedAsync(string Role)
    {
        foreach (string name in ProfessionalReadActions)
        {
            var action = ControllerPolicyEvaluator.Action(typeof(StudentsController), name);
            Assert.True(await ControllerPolicyEvaluator.IsAuthorizedAsync(action, Role), $"{name} must be allowed to {Role}");
        }
    }

    [Fact]
    public async Task ListAndProfileRead_GuestIsDeniedAsync()
    {
        foreach (string name in ProfessionalReadActions)
        {
            var action = ControllerPolicyEvaluator.Action(typeof(StudentsController), name);
            Assert.False(await ControllerPolicyEvaluator.IsAuthorizedAsync(action), $"{name} must be denied to a Guest");
        }
    }

    [Fact]
    public async Task EveryOtherAction_StaysRestrictedToAdministratorAndOfficerAsync()
    {
        foreach (MethodInfo action in StudentActions().Where(Method => !ProfessionalReadActions.Contains(Method.Name)))
        {
            Assert.True(await ControllerPolicyEvaluator.IsAuthorizedAsync(action, Administrator), $"{action.Name} must allow an Administrator");
            Assert.True(await ControllerPolicyEvaluator.IsAuthorizedAsync(action, Officer), $"{action.Name} must allow an Officer");
            Assert.False(await ControllerPolicyEvaluator.IsAuthorizedAsync(action, Professional), $"{action.Name} must deny a Professional");
        }
    }

    [Fact]
    public void EveryAction_DeclaresItsOwnPolicy_SoANewActionIsNeverOpenByOmission()
    {
        // ADR-0004: the class policy is AnyErasRole, so each action must state who may call it.
        foreach (MethodInfo action in StudentActions())
        {
            AuthorizeAttribute? own = action.GetCustomAttributes<AuthorizeAttribute>(inherit: false).SingleOrDefault();
            Assert.True(own?.Policy is not null, $"{action.Name} must declare its own [Authorize(Policy = ...)]");
            string expected = ProfessionalReadActions.Contains(action.Name) ? ErasPolicies.AnyErasRole : ErasPolicies.AdminOrOfficer;
            Assert.Equal(expected, own!.Policy);
        }
    }
}
