using Eras.Domain.Entities.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement.Validators;

using FluentValidation.TestHelper;

namespace Eras.Domain.Tests.Entities.Validators;
public class InterventionPlanValidatorTests
{
    private readonly InterventionPlanValidator _validator = new();
    private ValidatableInterventionPlan _validatableInterventionPlan = new();

    private static InterventionPlan CreateInterventionPlan(ValidatableInterventionPlan Data)
        => new InterventionPlan
        {
            SessionsPerWeek = Data.SessionsPerWeek,
            ScheduleNotes = Data.ScheduleNotes
        };
    
    [Fact]
    public void Should_NotHaveAnyValidationErrors_When_PlanIsValid()
    {
        InterventionPlan plan = CreateInterventionPlan(_validatableInterventionPlan);

        TestValidationResult<InterventionPlan> result = _validator.TestValidate(plan);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_NotHaveErrors_When_SessionsPerWeekIsNull()
    {
        _validatableInterventionPlan.SessionsPerWeek = null;
        InterventionPlan plan = CreateInterventionPlan(_validatableInterventionPlan);

        TestValidationResult<InterventionPlan> result = _validator.TestValidate(plan);

        result.ShouldNotHaveValidationErrorFor(x => x.SessionsPerWeek);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_HaveError_When_SessionsPerWeekIsLessThanOne(int InvalidAmount)
    {
        _validatableInterventionPlan.SessionsPerWeek = InvalidAmount;
        InterventionPlan plan = CreateInterventionPlan(_validatableInterventionPlan);

        TestValidationResult<InterventionPlan> result = _validator.TestValidate(plan);

        result.ShouldHaveValidationErrorFor(x => x.SessionsPerWeek);
    }

    [Theory]
    [InlineData(2000, false)]
    [InlineData(2001, true)]
    public void Should_ValidateMaximumLength_ForScheduleNotes(int Length, bool ExpectError)
    {
        _validatableInterventionPlan.ScheduleNotes = new string('a', Length);
        InterventionPlan plan = CreateInterventionPlan(_validatableInterventionPlan);

        TestValidationResult<InterventionPlan> result = _validator.TestValidate(plan);

        if (ExpectError)
            result.ShouldHaveValidationErrorFor(x => x.ScheduleNotes);
        else
            result.ShouldNotHaveValidationErrorFor(x => x.ScheduleNotes);
    }
}