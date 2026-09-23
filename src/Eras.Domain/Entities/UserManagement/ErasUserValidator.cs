using FluentValidation;

namespace Eras.Domain.Entities.UserManagement;

public sealed class ErasUserValidator : AbstractValidator<ErasUser>
{
    public ErasUserValidator()
    {
        RuleFor(U => U.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .Matches("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$")
            .WithMessage("Email is not valid.");
        RuleFor(U => U.FirstName)
            .NotEmpty()
            .WithMessage("First name is required.")
            .MaximumLength(100)
            .WithMessage("First name must be under 100 characters.");
        RuleFor(U => U.LastName)
            .NotEmpty()
            .WithMessage("Last name is required.")
            .MaximumLength(100)
            .WithMessage("Last name must be under 100 characters.");
        RuleFor(U => U.Role)
            .NotEmpty()
            .WithMessage("Role is required.")
            .Custom((role, context) =>
            {
                IEnumerable<string> validRoles = ErasRole.ListLabels();
                if (!validRoles.Contains(role))
                {
                    context.AddFailure($"{role} is not a valid Eras role.");
                }
            });
        RuleFor(U => U.Audit)
            .NotEmpty()
            .WithMessage("Audit information is required");
        RuleFor(U => U.Audit.CreatedBy)
            .MaximumLength(50)
            .WithMessage("Audit.CreatedBy must be under 50 characters.");
        RuleFor(U => U.Audit.ModifiedBy)
            .MaximumLength(50)
            .WithMessage("Audit.ModifiedBy must be under 50 characters.");
    }
}