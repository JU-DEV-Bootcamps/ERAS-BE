using Eras.Domain.Entities.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement.Validators;

using FluentValidation.TestHelper;

namespace Eras.Domain.Tests.Entities.AssessmentManagement.Validators;

public class InterventionValidatorTests
{
    private readonly InterventionValidator _validator = new();

    private static Intervention CreateValidIntervention(
        List<string>? attachments = null,
        List<int>? studentIds = null,
        int? riskLevel = null)
    {
        return new Intervention
        {
            DateUtc = DateTime.UtcNow,
            Activity = "Activity",
            Area = "Area",
            Professional = "Professional",
            Comments = "Some comments",
            StudentIds = studentIds ?? new List<int> { 1 },
            Remarks = "Some remarks",
            Attachments = attachments ?? new List<string> { "file1.pdf" },
            RiskLevel = riskLevel ?? 2,
        };
    }

    [Fact]
    public void Should_Not_Have_Error_When_Attachments_Are_Within_Max()
    {
        var intervention = CreateValidIntervention(
            new List<string> { "a.pdf", "b.pdf", "c.pdf" });

        var result = _validator.TestValidate(intervention);

        result.ShouldNotHaveValidationErrorFor(x => x.Attachments);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Attachments_Are_Empty()
    {
        var intervention = CreateValidIntervention(new List<string>());

        var result = _validator.TestValidate(intervention);

        result.ShouldNotHaveValidationErrorFor(x => x.Attachments);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Attachments_Are_Exactly_At_Max()
    {
        var intervention = CreateValidIntervention(
            new List<string> { "a.pdf", "b.pdf", "c.pdf", "d.pdf", "e.pdf" });

        var result = _validator.TestValidate(intervention);

        result.ShouldNotHaveValidationErrorFor(x => x.Attachments);
    }

    [Fact]
    public void Should_Have_Error_When_Attachments_Exceed_Max()
    {
        var intervention = CreateValidIntervention(
            new List<string> { "a.pdf", "b.pdf", "c.pdf", "d.pdf", "e.pdf", "f.pdf" });

        var result = _validator.TestValidate(intervention);

        result.ShouldHaveValidationErrorFor(x => x.Attachments)
            .WithErrorMessage("An intervention cannot have more than 5 attached documents.");
    }

    [Fact]
    public void Should_Have_Error_When_An_Individual_Attachment_Is_Empty()
    {
        var intervention = CreateValidIntervention(
            new List<string> { "a.pdf", "" });

        var result = _validator.TestValidate(intervention);

        result.ShouldHaveValidationErrorFor("Attachments[1]");
    }

    [Fact]
    public void Should_Have_Error_When_StudentIds_Is_Empty()
    {
        var intervention = CreateValidIntervention(studentIds: new List<int>());

        var result = _validator.TestValidate(intervention);

        result.ShouldHaveValidationErrorFor(x => x.StudentIds);
    }

    [Fact]
    public void Should_Have_Error_When_RiskLevel_Is_Out_Of_Range()
    {
        var intervention = CreateValidIntervention(riskLevel: 6);

        var result = _validator.TestValidate(intervention);

        result.ShouldHaveValidationErrorFor(x => x.RiskLevel);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Intervention_Is_Fully_Valid()
    {
        var intervention = CreateValidIntervention();

        var result = _validator.TestValidate(intervention);

        result.ShouldNotHaveAnyValidationErrors();
    }
}