using Eras.Domain.Entities.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement.Validators;

using FluentValidation.TestHelper;

namespace Eras.Domain.Tests.Entities.Validators;
public class AssessmentValidatorTests
{
    private readonly AssessementValidator _validator = new();
    private ValidatableAssessment _validatableAssessment = new();

    private static Assessment CreateAssessment(ValidatableAssessment Data)
        => new Assessment
        {
            CreatedAtUtc = Data.CreatedAtUtc,
            CreatedBy = Data.CreatedBy,
            Service = Data.Service,
            AssignedProfessional = Data.AssignedProfessional,
            StudentIds = Data.StudentIds,
            Diagnosis = Data.Diagnosis,
            Objective = Data.Objective,
            Comments = Data.Comments,
            Status = AssessmentStatus.Remitted,
            Plan = Data.Plan,
            Interventions = Data.Interventions
        };
    
    [Fact]
    public void Should_NotHaveAnyValidationErrors_When_AssessmentIsValid()
    {
        Assessment assessment = CreateAssessment(_validatableAssessment);

        TestValidationResult<Assessment> result = _validator.TestValidate(assessment);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_HaveError_When_CreatedAtUtcIsEmpty()
    {
        _validatableAssessment.CreatedAtUtc = default;
        Assessment Assessment = CreateAssessment(_validatableAssessment);

        TestValidationResult<Assessment> result = _validator.TestValidate(Assessment);

        result.ShouldHaveValidationErrorFor(x => x.CreatedAtUtc);
    }

    [Fact]
    public void Should_HaveError_When_CreatedByIsEmpty()
    {
        _validatableAssessment.CreatedBy = "";
        Assessment Assessment = CreateAssessment(_validatableAssessment);

        TestValidationResult<Assessment> result = _validator.TestValidate(Assessment);

        result.ShouldHaveValidationErrorFor(x => x.CreatedBy);
    }

    [Fact]
    public void Should_HaveError_When_CreatedByIsNull()
    {
        _validatableAssessment.CreatedBy = null!;
        Assessment Assessment = CreateAssessment(_validatableAssessment);

        TestValidationResult<Assessment> result = _validator.TestValidate(Assessment);

        result.ShouldHaveValidationErrorFor(x => x.CreatedBy);
    }

    [Theory]
    [InlineData(200, false)]
    [InlineData(201, true)]
    public void Should_ValidateMaximumLength_ForCreatedBy(int Length, bool ExpectError)
    {
        _validatableAssessment.CreatedBy = new string('a', Length);
        Assessment Assessment = CreateAssessment(_validatableAssessment);

        TestValidationResult<Assessment> result = _validator.TestValidate(Assessment);

        if (ExpectError)
            result.ShouldHaveValidationErrorFor(x => x.CreatedBy);
        else
            result.ShouldNotHaveValidationErrorFor(x => x.CreatedBy);
    }

    [Fact]
    public void Should_HaveError_When_ServiceIsEmpty()
    {
        _validatableAssessment.Service = "";
        Assessment Assessment = CreateAssessment(_validatableAssessment);

        TestValidationResult<Assessment> result = _validator.TestValidate(Assessment);

        result.ShouldHaveValidationErrorFor(x => x.Service);
    }

    [Fact]
    public void Should_HaveError_When_ServiceIsNull()
    {
        _validatableAssessment.Service = null!;
        Assessment Assessment = CreateAssessment(_validatableAssessment);

        TestValidationResult<Assessment> result = _validator.TestValidate(Assessment);

        result.ShouldHaveValidationErrorFor(x => x.Service);
    }

    [Theory]
    [InlineData(200, false)]
    [InlineData(201, true)]
    public void Should_ValidateMaximumLength_ForService(int Length, bool ExpectError)
    {
        _validatableAssessment.Service = new string('a', Length);
        Assessment Assessment = CreateAssessment(_validatableAssessment);

        TestValidationResult<Assessment> result = _validator.TestValidate(Assessment);

        if (ExpectError)
            result.ShouldHaveValidationErrorFor(x => x.Service);
        else
            result.ShouldNotHaveValidationErrorFor(x => x.Service);
    }

    [Theory]
    [InlineData(200, false)]
    [InlineData(201, true)]
    public void Should_ValidateMaximumLength_ForAssignedProfessional(int Length, bool ExpectError)
    {
        _validatableAssessment.AssignedProfessional = new string('a', Length);
        Assessment Assessment = CreateAssessment(_validatableAssessment);

        TestValidationResult<Assessment> result = _validator.TestValidate(Assessment);

        if (ExpectError)
            result.ShouldHaveValidationErrorFor(x => x.AssignedProfessional);
        else
            result.ShouldNotHaveValidationErrorFor(x => x.AssignedProfessional);
    }

    [Fact]
    public void Should_HaveError_When_StudentIdsIsEmptyList()
    {
        _validatableAssessment.StudentIds = [];
        Assessment Assessment = CreateAssessment(_validatableAssessment);

        TestValidationResult<Assessment> result = _validator.TestValidate(Assessment);

        result.ShouldHaveValidationErrorFor(x => x.StudentIds)
            .WithErrorMessage("At least one student must be assigned to the assessment.");
    }

    [Theory]
    [InlineData(4000, false)]
    [InlineData(4001, true)]
    public void Should_ValidateMaximumLength_ForDiagnosis(int Length, bool ExpectError)
    {
        _validatableAssessment.Diagnosis = new string('a', Length);
        Assessment Assessment = CreateAssessment(_validatableAssessment);

        TestValidationResult<Assessment> result = _validator.TestValidate(Assessment);

        if (ExpectError)
            result.ShouldHaveValidationErrorFor(x => x.Diagnosis);
        else
            result.ShouldNotHaveValidationErrorFor(x => x.Diagnosis);
    }

    [Theory]
    [InlineData(4000, false)]
    [InlineData(4001, true)]
    public void Should_ValidateMaximumLength_ForObjective(int Length, bool ExpectError)
    {
        _validatableAssessment.Objective = new string('a', Length);
        Assessment Assessment = CreateAssessment(_validatableAssessment);

        TestValidationResult<Assessment> result = _validator.TestValidate(Assessment);

        if (ExpectError)
            result.ShouldHaveValidationErrorFor(x => x.Objective);
        else
            result.ShouldNotHaveValidationErrorFor(x => x.Objective);
    }

    [Theory]
    [InlineData(4000, false)]
    [InlineData(4001, true)]
    public void Should_ValidateMaximumLength_ForComments(int Length, bool ExpectError)
    {
        _validatableAssessment.Comments = new string('a', Length);
        Assessment Assessment = CreateAssessment(_validatableAssessment);

        TestValidationResult<Assessment> result = _validator.TestValidate(Assessment);

        if (ExpectError)
            result.ShouldHaveValidationErrorFor(x => x.Comments);
        else
            result.ShouldNotHaveValidationErrorFor(x => x.Comments);
    }

    [Fact]
    public void Should_HaveError_When_InvalidPlanIsProvided()
    {
        InterventionPlan plan = new()
        {
            SessionsPerWeek = 0
        };

        _validatableAssessment.Plan = plan;
        Assessment Assessment = CreateAssessment(_validatableAssessment);

        TestValidationResult<Assessment> result = _validator.TestValidate(Assessment);

        result.ShouldHaveValidationErrorFor("Plan.SessionsPerWeek");
    }

    [Fact]
    public void Should_HaveError_When_InvalidIndividualInterventionIsProvided()
    {
        IndividualIntervention intervention = new()
        {
            DateUtc = DateTime.Now,
            StudentIds = null!
        };

        _validatableAssessment.Interventions = [intervention];
        Assessment Assessment = CreateAssessment(_validatableAssessment);

        TestValidationResult<Assessment> result = _validator.TestValidate(Assessment);

        result.ShouldHaveValidationErrorFor("Interventions[0].StudentIds");
    }

    [Fact]
    public void Should_HaveError_When_InvalidGroupInterventionIsProvided()
    {
        GroupIntervention intervention = new()
        {
            DateUtc = DateTime.Now,
            StudentIds = null!
        };

        _validatableAssessment.Interventions = [intervention];
        Assessment Assessment = CreateAssessment(_validatableAssessment);

        TestValidationResult<Assessment> result = _validator.TestValidate(Assessment);

        result.ShouldHaveValidationErrorFor("Interventions[0].StudentIds");
    }
}