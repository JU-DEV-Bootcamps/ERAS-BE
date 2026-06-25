using FluentValidation;

namespace Eras.Domain.Entities.FeatureFlagManagement;

public sealed class FeatureFlagValidator : AbstractValidator<FeatureFlag>
{
    public FeatureFlagValidator()
    {
        RuleFor(F => F.Name)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Name is required");
        RuleFor(F => F.Description)
            .NotEmpty()
            .MaximumLength(1000)
            .WithMessage("Description is required and should be under 1000 characters");
        
        RuleFor(F => F.Audit)
            .NotEmpty()
            .WithMessage("Audit information is required");
        RuleFor(F => F.Audit.CreatedBy)
            .MaximumLength(50)
            .WithMessage("Audit.CreatedBy should be under 50 characters");
        RuleFor(F => F.Audit.ModifiedBy)
            .MaximumLength(50)
            .WithMessage("Audit.ModifiedBy should be under 50 characters");
    }
}