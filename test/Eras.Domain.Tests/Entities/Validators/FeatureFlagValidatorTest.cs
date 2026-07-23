using Eras.Domain.Common;
using Eras.Domain.Entities.FeatureFlagManagement;
using FluentValidation.TestHelper;

namespace Eras.Domain.Tests.Entities.Validators;
public class FeatureFlagValidatorTests
{
    private readonly FeatureFlagValidator _validator = new();

    private static FeatureFlag CreateFeatureFlag(
        string Name = "Valid Flag Name",
        string Description = "A valid description under 1000 characters.",
        AuditInfo? Audit = null) => new()
        {
            Name = Name,
            Description = Description,
            Audit = Audit ?? new AuditInfo()
        };

    [Fact]
    public void Should_NotHaveAnyValidationErrors_When_FeatureFlagIsValid()
    {
        FeatureFlag flag = CreateFeatureFlag();

        TestValidationResult<FeatureFlag> result = _validator.TestValidate(flag);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_HaveError_When_NameIsEmpty()
    {
        FeatureFlag flag = CreateFeatureFlag(Name: string.Empty);

        TestValidationResult<FeatureFlag> result = _validator.TestValidate(flag);

        result.ShouldHaveValidationErrorFor(f => f.Name)
            .WithErrorMessage("Name is required.");
    }

    [Fact]
    public void Should_HaveError_When_NameIsNull()
    {
        FeatureFlag flag = CreateFeatureFlag(Name: null!);

        TestValidationResult<FeatureFlag> result = _validator.TestValidate(flag);

        result.ShouldHaveValidationErrorFor(f => f.Name)
            .WithErrorMessage("Name is required.");
    }

    [Fact]
    public void Should_HaveError_When_NameExceedsMaximumLength()
    {
        FeatureFlag flag = CreateFeatureFlag(Name: new string('a', 101));

        TestValidationResult<FeatureFlag> result = _validator.TestValidate(flag);

        result.ShouldHaveValidationErrorFor(f => f.Name)
            .WithErrorMessage("Name must be under 100 characters.");
    }

    [Fact]
    public void Should_NotHaveError_When_NameIsExactlyMaximumLength()
    {
        FeatureFlag flag = CreateFeatureFlag(Name: new string('a', 100));

        TestValidationResult<FeatureFlag> result = _validator.TestValidate(flag);

        result.ShouldNotHaveValidationErrorFor(f => f.Name);
    }

    [Fact]
    public void Should_NotHaveError_When_NameIsValid()
    {
        FeatureFlag flag = CreateFeatureFlag();

        TestValidationResult<FeatureFlag> result = _validator.TestValidate(flag);

        result.ShouldNotHaveValidationErrorFor(f => f.Name);
    }

    [Fact]
    public void Should_HaveError_When_DescriptionIsEmpty()
    {
        FeatureFlag flag = CreateFeatureFlag(Description: string.Empty);

        TestValidationResult<FeatureFlag> result = _validator.TestValidate(flag);

        result.ShouldHaveValidationErrorFor(f => f.Description)
            .WithErrorMessage("Description is required.");
    }

    [Fact]
    public void Should_HaveError_When_DescriptionIsNull()
    {
        FeatureFlag flag = CreateFeatureFlag(Description: null!);

        TestValidationResult<FeatureFlag> result = _validator.TestValidate(flag);

        result.ShouldHaveValidationErrorFor(f => f.Description)
            .WithErrorMessage("Description is required.");
    }

    [Fact]
    public void Should_HaveError_When_DescriptionExceedsMaximumLength()
    {
        FeatureFlag flag = CreateFeatureFlag(Description: new string('a', 1001));

        TestValidationResult<FeatureFlag> result = _validator.TestValidate(flag);

        result.ShouldHaveValidationErrorFor(f => f.Description)
            .WithErrorMessage("Description must be under 1000 characters.");
    }

    [Fact]
    public void Should_NotHaveError_When_DescriptionIsExactlyMaximumLength()
    {
        FeatureFlag flag = CreateFeatureFlag(Description: new string('a', 1000));

        TestValidationResult<FeatureFlag> result = _validator.TestValidate(flag);

        result.ShouldNotHaveValidationErrorFor(f => f.Description);
    }

    [Fact]
    public void Should_NotHaveError_When_DescriptionIsValid()
    {
        FeatureFlag flag = CreateFeatureFlag();

        TestValidationResult<FeatureFlag> result = _validator.TestValidate(flag);

        result.ShouldNotHaveValidationErrorFor(f => f.Description);
    }

    [Fact]
    public void Should_NotHaveError_When_AuditIsProvided()
    {
        FeatureFlag flag = CreateFeatureFlag();

        TestValidationResult<FeatureFlag> result = _validator.TestValidate(flag);

        result.ShouldNotHaveValidationErrorFor(f => f.Audit);
    }

    [Fact]
    public void Should_HaveError_When_AuditCreatedByExceedsMaximumLength()
    {
        FeatureFlag flag = CreateFeatureFlag(Audit: new AuditInfo
        {
            CreatedBy = new string('a', 51),
            ModifiedBy = "System"
        });

        TestValidationResult<FeatureFlag> result = _validator.TestValidate(flag);

        result.ShouldHaveValidationErrorFor(f => f.Audit.CreatedBy)
            .WithErrorMessage("Audit.CreatedBy must be under 50 characters.");
    }

    [Fact]
    public void Should_NotHaveError_When_AuditCreatedByIsExactlyMaximumLength()
    {
        FeatureFlag flag = CreateFeatureFlag(Audit: new AuditInfo
        {
            CreatedBy = new string('a', 50),
            ModifiedBy = "System"
        });

        TestValidationResult<FeatureFlag> result = _validator.TestValidate(flag);

        result.ShouldNotHaveValidationErrorFor(f => f.Audit.CreatedBy);
    }

    [Fact]
    public void Should_NotHaveError_When_AuditCreatedByIsNullOrEmpty()
    {
        FeatureFlag flagWithNull = CreateFeatureFlag(Audit: new AuditInfo
        {
            CreatedBy = null!,
            ModifiedBy = "System"
        });
        FeatureFlag flagWithEmpty = CreateFeatureFlag(Audit: new AuditInfo
        {
            CreatedBy = string.Empty,
            ModifiedBy = "System"
        });

        _validator.TestValidate(flagWithNull).ShouldNotHaveValidationErrorFor(f => f.Audit.CreatedBy);
        _validator.TestValidate(flagWithEmpty).ShouldNotHaveValidationErrorFor(f => f.Audit.CreatedBy);
    }

    [Fact]
    public void Should_HaveError_When_AuditModifiedByExceedsMaximumLength()
    {
        FeatureFlag flag = CreateFeatureFlag(Audit: new AuditInfo
        {
            CreatedBy = "System",
            ModifiedBy = new string('a', 51)
        });

        TestValidationResult<FeatureFlag> result = _validator.TestValidate(flag);

        result.ShouldHaveValidationErrorFor(f => f.Audit.ModifiedBy)
            .WithErrorMessage("Audit.ModifiedBy must be under 50 characters.");
    }

    [Fact]
    public void Should_NotHaveError_When_AuditModifiedByIsExactlyMaximumLength()
    {
        FeatureFlag flag = CreateFeatureFlag(Audit: new AuditInfo
        {
            CreatedBy = "System",
            ModifiedBy = new string('a', 50)
        });

        TestValidationResult<FeatureFlag> result = _validator.TestValidate(flag);

        result.ShouldNotHaveValidationErrorFor(f => f.Audit.ModifiedBy);
    }

    [Fact]
    public void Should_NotHaveError_When_AuditModifiedByIsNullOrEmpty()
    {
        FeatureFlag flagWithNull = CreateFeatureFlag(Audit: new AuditInfo
        {
            CreatedBy = "System",
            ModifiedBy = null!
        });
        FeatureFlag flagWithEmpty = CreateFeatureFlag(Audit: new AuditInfo
        {
            CreatedBy = "System",
            ModifiedBy = string.Empty
        });

        _validator.TestValidate(flagWithNull).ShouldNotHaveValidationErrorFor(f => f.Audit.ModifiedBy);
        _validator.TestValidate(flagWithEmpty).ShouldNotHaveValidationErrorFor(f => f.Audit.ModifiedBy);
    }
}