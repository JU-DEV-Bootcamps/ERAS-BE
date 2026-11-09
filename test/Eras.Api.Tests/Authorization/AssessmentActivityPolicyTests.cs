using Eras.Api.Controllers.AssessmentManagement;

using Xunit;

namespace Eras.Api.Tests.Authorization;

/// <summary>Who can read the activity history of an assessment (the real policy declared on the action).</summary>
public class AssessmentActivityPolicyTests
{
    private static System.Reflection.MethodInfo Action() =>
        ControllerPolicyEvaluator.Action(typeof(AssessmentsController), "GetAssessmentActivity");

    [Fact]
    [Trait("Change", "681-add-activity-history-to-assessment-details")]
    [Trait("AC", "681-add-activity-history-to-assessment-details/AC-8")]
    public async Task GetAssessmentActivity_StudentServicesOfficer_IsAuthorizedAsync()
    {
        // Arrange (GIVEN) an ERAS Student Services Officer
        // Act (WHEN) they request the activity history of an assessment
        bool allowed = await ControllerPolicyEvaluator.IsAuthorizedAsync(Action(), "ERAS Student Services Officer");

        // Assert (THEN) the action is allowed (200 OK from the pipeline's point of view)
        Assert.True(allowed);
    }

    [Fact]
    [Trait("Change", "681-add-activity-history-to-assessment-details")]
    [Trait("AC", "681-add-activity-history-to-assessment-details/AC-9")]
    public async Task GetAssessmentActivity_Professional_IsAuthorizedAsync()
    {
        bool allowed = await ControllerPolicyEvaluator.IsAuthorizedAsync(Action(), "ERAS Professional");

        Assert.True(allowed);
    }

    [Fact]
    [Trait("Change", "681-add-activity-history-to-assessment-details")]
    [Trait("AC", "681-add-activity-history-to-assessment-details/AC-10")]
    public async Task GetAssessmentActivity_Guest_IsForbiddenAsync()
    {
        // Arrange (GIVEN) a user with no ERAS role (Guest)
        // Act (WHEN) they request the activity history of an assessment
        bool allowed = await ControllerPolicyEvaluator.IsAuthorizedAsync(Action());

        // Assert (THEN) the action is denied (403 Forbidden)
        Assert.False(allowed);
    }
}
