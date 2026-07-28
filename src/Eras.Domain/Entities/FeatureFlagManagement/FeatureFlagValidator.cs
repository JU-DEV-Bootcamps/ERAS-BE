using FluentValidation;

namespace Eras.Domain.Entities.FeatureFlagManagement;

public sealed class FeatureFlagValidator : AbstractValidator<FeatureFlag>
{
    public FeatureFlagValidator()
    {
        RuleFor(F => F.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(100)
            .WithMessage("Name must be under 100 characters.");
        RuleFor(F => F.Description)
            .NotEmpty()
            .WithMessage("Description is required.")
            .MaximumLength(1000)
            .WithMessage("Description must be under 1000 characters.");
        
        RuleFor(F => F.Audit)
            .NotEmpty()
            .WithMessage("Audit information is required");
        RuleFor(F => F.Audit.CreatedBy)
            .MaximumLength(50)
            .WithMessage("Audit.CreatedBy must be under 50 characters.");
        RuleFor(F => F.Audit.ModifiedBy)
            .MaximumLength(50)
            .WithMessage("Audit.ModifiedBy must be under 50 characters.");
    }
}