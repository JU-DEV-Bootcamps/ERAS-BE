using Eras.Application.DTOs.Student;

using FluentValidation;

namespace Eras.Application.Validation;

public sealed class StudentRegistrationDtoValidator : AbstractValidator<StudentRegistrationDto>
{
    private const string NamePattern = @"^[\p{L}][\p{L}'\-\.\s]*$";
    private const string IdPassportPattern = @"^[A-Za-z0-9][A-Za-z0-9\-\s]*$";
    private const string PhonePattern = @"^\+?[0-9\s\-\(\)]{6,20}$";
    private const string EmailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

    public StudentRegistrationDtoValidator()
    {
        RuleFor(Dto => Dto.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100).WithMessage("First name must be under 100 characters.")
            .Matches(NamePattern).WithMessage("First name can only contain letters, spaces, apostrophes, dots and hyphens.");

        RuleFor(Dto => Dto.MiddleName)
            .MaximumLength(100).WithMessage("Middle name must be under 100 characters.")
            .Matches(NamePattern).WithMessage("Middle name can only contain letters, spaces, apostrophes, dots and hyphens.")
            .When(Dto => !string.IsNullOrWhiteSpace(Dto.MiddleName));

        RuleFor(Dto => Dto.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100).WithMessage("Last name must be under 100 characters.")
            .Matches(NamePattern).WithMessage("Last name can only contain letters, spaces, apostrophes, dots and hyphens.");

        RuleFor(Dto => Dto.IdPassportNumber)
            .NotEmpty().WithMessage("ID / Passport number is required.")
            .MaximumLength(50).WithMessage("ID / Passport number must be under 50 characters.")
            .Matches(IdPassportPattern).WithMessage("ID / Passport number can only contain letters, numbers, spaces and hyphens.");

        RuleFor(Dto => Dto.PrimaryEmail)
            .NotEmpty().WithMessage("Primary email is required.")
            .MaximumLength(255).WithMessage("Primary email must be under 255 characters.")
            .Matches(EmailPattern).WithMessage("Primary email is not valid.");

        RuleFor(Dto => Dto.SecondaryEmail)
            .MaximumLength(255).WithMessage("Secondary email must be under 255 characters.")
            .Matches(EmailPattern).WithMessage("Secondary email is not valid.")
            .When(Dto => !string.IsNullOrWhiteSpace(Dto.SecondaryEmail));

        RuleFor(Dto => Dto.SecondaryEmail)
            .Must((Dto, Secondary) => !string.Equals(
                Secondary?.Trim(), Dto.PrimaryEmail?.Trim(), StringComparison.OrdinalIgnoreCase))
            .WithMessage("Secondary email must be different from the primary email.")
            .When(Dto => !string.IsNullOrWhiteSpace(Dto.SecondaryEmail));

        RuleFor(Dto => Dto.DateOfBirth)
            .Must(Date => Date!.Value <= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Date of birth cannot be in the future.")
            .Must(Date => Date!.Value.Year >= 1900)
            .WithMessage("Date of birth is not valid.")
            .When(Dto => Dto.DateOfBirth.HasValue);

        RuleFor(Dto => Dto.MobileNumber)
            .Matches(PhonePattern).WithMessage("Mobile number is not valid.")
            .When(Dto => !string.IsNullOrWhiteSpace(Dto.MobileNumber));

        RuleFor(Dto => Dto.Gender).MaximumLength(50).WithMessage("Gender must be under 50 characters.");
        RuleFor(Dto => Dto.Nationality).MaximumLength(100).WithMessage("Nationality must be under 100 characters.");
        RuleFor(Dto => Dto.CountryOfBirth).MaximumLength(100).WithMessage("Country of birth must be under 100 characters.");
        RuleFor(Dto => Dto.Country).MaximumLength(100).WithMessage("Country must be under 100 characters.");
        RuleFor(Dto => Dto.StateProvince).MaximumLength(100).WithMessage("State / Province must be under 100 characters.");
        RuleFor(Dto => Dto.City).MaximumLength(100).WithMessage("City must be under 100 characters.");
        RuleFor(Dto => Dto.Street).MaximumLength(200).WithMessage("Street must be under 200 characters.");
        RuleFor(Dto => Dto.PostalCode).MaximumLength(20).WithMessage("Postal code must be under 20 characters.");
        RuleFor(Dto => Dto.LevelOfStudy).MaximumLength(100).WithMessage("Level of study must be under 100 characters.");
        RuleFor(Dto => Dto.FacultySchool).MaximumLength(150).WithMessage("Faculty / School must be under 150 characters.");
        RuleFor(Dto => Dto.StudyModality).MaximumLength(100).WithMessage("Study modality must be under 100 characters.");
        RuleFor(Dto => Dto.PreviousInstitution).MaximumLength(200).WithMessage("Previous institution must be under 200 characters.");
    }
}
