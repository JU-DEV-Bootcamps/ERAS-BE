using Eras.Api.Controllers.AssessmentManagement;

using Xunit;

namespace Eras.Api.Tests.Authorization;

/// <summary>Who can read the activity history of an intervention (the real policy declared on the action).</summary>
public class InterventionActivityPolicyTests
{

    private static System.Reflection.MethodInfo Action() =>
        ControllerPolicyEvaluator.Action(typeof(AssessmentsController), "GetInterventionActivity");

    [Fact]
    [Trait("Change", "680-add-activity-history-to-intervention-details")]
    [Trait("AC", "680-add-activity-history-to-intervention-details/AC-8")]
    public async Task GetInterventionActivity_StudentServicesOfficer_IsAuthorizedAsync()
    {
        // Arrange (GIVEN) an ERAS Student Services Officer
        // Act (WHEN) they request the activity history of an intervention
        bool allowed = await ControllerPolicyEvaluator.IsAuthorizedAsync(Action(), "ERAS Student Services Officer");

        // Assert (THEN) the action is allowed (200 OK from the pipeline's point of view)
        Assert.True(allowed);
    }

    [Fact]
    [Trait("Change", "680-add-activity-history-to-intervention-details")]
    [Trait("AC", "680-add-activity-history-to-intervention-details/AC-9")]
    public async Task GetInterventionActivity_Professional_IsAuthorizedAsync()
    {
        bool allowed = await ControllerPolicyEvaluator.IsAuthorizedAsync(Action(), "ERAS Professional");

        Assert.True(allowed);
    }

    [Fact]
    [Trait("Change", "680-add-activity-history-to-intervention-details")]
    [Trait("AC", "680-add-activity-history-to-intervention-details/AC-10")]
    public async Task GetInterventionActivity_Guest_IsForbiddenAsync()
    {
        // Arrange (GIVEN) a user with no ERAS role (Guest)
        // Act (WHEN) they request the activity history of an intervention
        bool allowed = await ControllerPolicyEvaluator.IsAuthorizedAsync(Action());

        // Assert (THEN) the action is denied (403 Forbidden)
        Assert.False(allowed);
    }
}
