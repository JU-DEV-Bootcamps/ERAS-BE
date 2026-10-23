using Eras.Domain.Entities.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement.StatusManagement;
using Eras.Domain.Entities.AssessmentManagement.Validators;

using FluentValidation.TestHelper;

namespace Eras.Domain.Tests.Entities.Validators;
public class StatusTransitionValidatorTests
{
    private readonly AssessmentStatusTransitionValidator _assessmentValidator = new();
    private readonly InterventionStatusTransitionValidator _interventionValidator = new();

    private StatusTransitionRequest<AssessmentStatus> _assessmentStatusTransitionRequest = null!;
    private StatusTransitionRequest<InterventionStatus> _interventionStatusTransitionRequest = null!;

    private static StatusTransitionRequest<AssessmentStatus> CreateAssessmentStatusRequest(AssessmentStatus Existing, AssessmentStatus New) =>
        new(Existing, New);
    private static StatusTransitionRequest<InterventionStatus> CreateInterventionStatusRequest(InterventionStatus Existing, InterventionStatus New) =>
        new(Existing, New);

    /* Assessment status */
    [Theory]
    [InlineData(AssessmentStatus.InProgress, AssessmentStatus.InProgress)]
    [InlineData(AssessmentStatus.Remitted, AssessmentStatus.Remitted)]
    [InlineData(AssessmentStatus.Finalized, AssessmentStatus.Finalized)]
    public void Should_NotHaveError_WhenTransitionToSameAssessmentStatus(AssessmentStatus Existing, AssessmentStatus New)
    {
        _assessmentStatusTransitionRequest = CreateAssessmentStatusRequest(Existing, New);

        TestValidationResult<StatusTransitionRequest<AssessmentStatus>> result =
            _assessmentValidator.TestValidate(_assessmentStatusTransitionRequest);
        
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(AssessmentStatus.Remitted, AssessmentStatus.InProgress)]
    [InlineData(AssessmentStatus.InProgress, AssessmentStatus.Finalized)]
    public void Should_NotHaveError_WhenTransitionToValidAssessmentStatus(AssessmentStatus Existing, AssessmentStatus New)
    {
        _assessmentStatusTransitionRequest = CreateAssessmentStatusRequest(Existing, New);

        TestValidationResult<StatusTransitionRequest<AssessmentStatus>> result =
            _assessmentValidator.TestValidate(_assessmentStatusTransitionRequest);
        
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(AssessmentStatus.Remitted, AssessmentStatus.Finalized)]
    public void Should_HaveError_When_TransitionFromRemittedToInvalidAssessmentStatus(AssessmentStatus Existing, AssessmentStatus New)
    {
        _assessmentStatusTransitionRequest = CreateAssessmentStatusRequest(Existing, New);

        TestValidationResult<StatusTransitionRequest<AssessmentStatus>> result =
            _assessmentValidator.TestValidate(_assessmentStatusTransitionRequest);

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage($"Cannot transition assessment from '{Existing}' to '{New}'.");
    }

    [Theory]
    [InlineData(AssessmentStatus.InProgress, AssessmentStatus.Remitted)]
    public void Should_HaveError_When_TransitionFromInProgressToInvalidAssessmentStatus(AssessmentStatus Existing, AssessmentStatus New)
    {
        _assessmentStatusTransitionRequest = CreateAssessmentStatusRequest(Existing, New);

        TestValidationResult<StatusTransitionRequest<AssessmentStatus>> result =
            _assessmentValidator.TestValidate(_assessmentStatusTransitionRequest);

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage($"Cannot transition assessment from '{Existing}' to '{New}'.");
    }

    [Theory]
    [InlineData(AssessmentStatus.Finalized, AssessmentStatus.Remitted)]
    [InlineData(AssessmentStatus.Finalized, AssessmentStatus.InProgress)]
    public void Should_HaveError_When_TransitionFromFinalizedToInvalidAssessmentStatus(AssessmentStatus Existing, AssessmentStatus New)
    {
        _assessmentStatusTransitionRequest = CreateAssessmentStatusRequest(Existing, New);

        TestValidationResult<StatusTransitionRequest<AssessmentStatus>> result =
            _assessmentValidator.TestValidate(_assessmentStatusTransitionRequest);

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage($"Cannot transition assessment from '{Existing}' to '{New}'.");
    }

    /* Intervention status */
    [Theory]
    [InlineData(InterventionStatus.InProgress, InterventionStatus.InProgress)]
    [InlineData(InterventionStatus.Remitted, InterventionStatus.Remitted)]
    [InlineData(InterventionStatus.Finalized, InterventionStatus.Finalized)]
    public void Should_NotHaveError_WhenTransitionToSameInterventionStatus(InterventionStatus Existing, InterventionStatus New)
    {
        _interventionStatusTransitionRequest = CreateInterventionStatusRequest(Existing, New);

        TestValidationResult<StatusTransitionRequest<InterventionStatus>> result =
            _interventionValidator.TestValidate(_interventionStatusTransitionRequest);
        
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(InterventionStatus.Remitted, InterventionStatus.InProgress)]
    [InlineData(InterventionStatus.InProgress, InterventionStatus.Finalized)]
    public void Should_NotHaveError_WhenTransitionToValidInterventionStatus(InterventionStatus Existing, InterventionStatus New)
    {
        _interventionStatusTransitionRequest = CreateInterventionStatusRequest(Existing, New);

        TestValidationResult<StatusTransitionRequest<InterventionStatus>> result =
            _interventionValidator.TestValidate(_interventionStatusTransitionRequest);
        
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(InterventionStatus.Remitted, InterventionStatus.Finalized)]
    public void Should_HaveError_When_TransitionFromRemittedToInvalidInterventionStatus(InterventionStatus Existing, InterventionStatus New)
    {
        _interventionStatusTransitionRequest = CreateInterventionStatusRequest(Existing, New);

        TestValidationResult<StatusTransitionRequest<InterventionStatus>> result =
            _interventionValidator.TestValidate(_interventionStatusTransitionRequest);

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage($"Cannot transition intervention from '{Existing}' to '{New}'.");
    }

    [Theory]
    [InlineData(InterventionStatus.InProgress, InterventionStatus.Remitted)]
    public void Should_HaveError_When_TransitionFromInProgressToInvalidInterventionStatus(InterventionStatus Existing, InterventionStatus New)
    {
        _interventionStatusTransitionRequest = CreateInterventionStatusRequest(Existing, New);

        TestValidationResult<StatusTransitionRequest<InterventionStatus>> result =
            _interventionValidator.TestValidate(_interventionStatusTransitionRequest);

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage($"Cannot transition intervention from '{Existing}' to '{New}'.");
    }

    [Theory]
    [InlineData(InterventionStatus.Finalized, InterventionStatus.Remitted)]
    [InlineData(InterventionStatus.Finalized, InterventionStatus.InProgress)]
    public void Should_HaveError_When_TransitionFromFinalizedToInvalidInterventionStatus(InterventionStatus Existing, InterventionStatus New)
    {
        _interventionStatusTransitionRequest = CreateInterventionStatusRequest(Existing, New);

        TestValidationResult<StatusTransitionRequest<InterventionStatus>> result =
            _interventionValidator.TestValidate(_interventionStatusTransitionRequest);

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage($"Cannot transition intervention from '{Existing}' to '{New}'.");
    }
}