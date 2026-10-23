using Eras.Domain.Entities.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement.Validators;

using FluentValidation.TestHelper;

namespace Eras.Domain.Tests.Entities.Validators;
public class IndividualInterventionValidatorTests
{
    private readonly IndividualInterventionValidator _validator = new();
    private ValidatableIntervention _validatableIntervention = new();

    private static IndividualIntervention CreateIntervention(ValidatableIntervention Data)
        => new IndividualIntervention
        {
            DateUtc = Data.DateUtc,
            Activity = Data.Activity,
            Area = Data.Area,
            Professional = Data.Professional,
            Comments = Data.Comments,
            StudentIds = Data.StudentIds,
            Remarks = Data.Remarks,
            Attachments = Data.Attachments,
            RiskLevel = Data.RiskLevel
        };
    
    [Fact]
    public void Should_NotHaveAnyValidationErrors_When_InterventionIsValid()
    {
        IndividualIntervention intervention = CreateIntervention(_validatableIntervention);

        TestValidationResult<IndividualIntervention> result = _validator.TestValidate(intervention);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_HaveError_When_DateUtcIsEmpty()
    {
        _validatableIntervention.DateUtc = default;
        IndividualIntervention intervention = CreateIntervention(_validatableIntervention);

        TestValidationResult<IndividualIntervention> result = _validator.TestValidate(intervention);

        result.ShouldHaveValidationErrorFor(x => x.DateUtc);
    }

    [Theory]
    [InlineData(200, false)]
    [InlineData(201, true)]
    public void Should_ValidateMaximumLength_ForActivity(int Length, bool ExpectError)
    {
        _validatableIntervention.Activity = new string('a', Length);
        IndividualIntervention intervention = CreateIntervention(_validatableIntervention);

        TestValidationResult<IndividualIntervention> result = _validator.TestValidate(intervention);

        if (ExpectError)
            result.ShouldHaveValidationErrorFor(x => x.Activity);
        else
            result.ShouldNotHaveValidationErrorFor(x => x.Activity);
    }

    [Theory]
    [InlineData(200, false)]
    [InlineData(201, true)]
    public void Should_ValidateMaximumLength_ForArea(int Length, bool ExpectError)
    {
        _validatableIntervention.Area = new string('a', Length);
        IndividualIntervention intervention = CreateIntervention(_validatableIntervention);

        TestValidationResult<IndividualIntervention> result = _validator.TestValidate(intervention);

        if (ExpectError)
            result.ShouldHaveValidationErrorFor(x => x.Area);
        else
            result.ShouldNotHaveValidationErrorFor(x => x.Area);
    }

    [Theory]
    [InlineData(200, false)]
    [InlineData(201, true)]
    public void Should_ValidateMaximumLength_ForProfessional(int Length, bool ExpectError)
    {
        _validatableIntervention.Professional = new string('a', Length);
        IndividualIntervention intervention = CreateIntervention(_validatableIntervention);

        TestValidationResult<IndividualIntervention> result = _validator.TestValidate(intervention);

        if (ExpectError)
            result.ShouldHaveValidationErrorFor(x => x.Professional);
        else
            result.ShouldNotHaveValidationErrorFor(x => x.Professional);
    }

    [Theory]
    [InlineData(4000, false)]
    [InlineData(4001, true)]
    public void Should_ValidateMaximumLength_ForComments(int Length, bool ExpectError)
    {
        _validatableIntervention.Comments = new string('a', Length);
        IndividualIntervention intervention = CreateIntervention(_validatableIntervention);

        TestValidationResult<IndividualIntervention> result = _validator.TestValidate(intervention);

        if (ExpectError)
            result.ShouldHaveValidationErrorFor(x => x.Comments);
        else
            result.ShouldNotHaveValidationErrorFor(x => x.Comments);
    }

    [Fact]
    public void Should_HaveError_When_StudentIdsIsNull()
    {
        _validatableIntervention.StudentIds = null!;
        IndividualIntervention intervention = CreateIntervention(_validatableIntervention);

        TestValidationResult<IndividualIntervention> result = _validator.TestValidate(intervention);

        result.ShouldHaveValidationErrorFor(x => x.StudentIds);
    }

    [Fact]
    public void Should_HaveError_When_StudentIdsIsEmptyList()
    {
        _validatableIntervention.StudentIds = [];
        IndividualIntervention intervention = CreateIntervention(_validatableIntervention);

        TestValidationResult<IndividualIntervention> result = _validator.TestValidate(intervention);

        result.ShouldHaveValidationErrorFor(x => x.StudentIds);
    }

    [Theory]
    [InlineData(1000, false)]
    [InlineData(1001, true)]
    public void Should_ValidateMaximumLength_ForRemarks(int Length, bool ExpectError)
    {
        _validatableIntervention.Remarks = new string('a', Length);
        IndividualIntervention intervention = CreateIntervention(_validatableIntervention);

        TestValidationResult<IndividualIntervention> result = _validator.TestValidate(intervention);

        if (ExpectError)
            result.ShouldHaveValidationErrorFor(x => x.Remarks);
        else
            result.ShouldNotHaveValidationErrorFor(x => x.Remarks);
    }

    [Fact]
    public void Should_HaveError_When_AttachmentExceedsMaximumLength()
    {
        _validatableIntervention.Attachments = [new string('a', 1001)];
        IndividualIntervention intervention = CreateIntervention(_validatableIntervention);

        TestValidationResult<IndividualIntervention> result = _validator.TestValidate(intervention);

        result.ShouldHaveValidationErrorFor("Attachments[0]");
    }

    [Fact]
    public void Should_HaveError_When_AttachmentsIsNull()
    {
        _validatableIntervention.Attachments = [null!];
        IndividualIntervention intervention = CreateIntervention(_validatableIntervention);

        TestValidationResult<IndividualIntervention> result = _validator.TestValidate(intervention);

        result.ShouldHaveValidationErrorFor(x => x.Attachments);
    }

    [Theory]
    [InlineData(-1, true)]
    [InlineData(-0.1, true)]
    [InlineData(0, false)]
    [InlineData(5, false)]
    [InlineData(5.1, true)]
    public void Should_ValidateRange_ForRiskLevel(double RiskLevel, bool ExpectError)
    {
        _validatableIntervention.RiskLevel = RiskLevel;
        IndividualIntervention intervention = CreateIntervention(_validatableIntervention);

        TestValidationResult<IndividualIntervention> result = _validator.TestValidate(intervention);

        if (ExpectError)
            result.ShouldHaveValidationErrorFor(x => x.RiskLevel);
        else
            result.ShouldNotHaveValidationErrorFor(x => x.RiskLevel);
    }
}