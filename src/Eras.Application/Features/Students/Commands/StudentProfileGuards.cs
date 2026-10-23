using Eras.Application.DTOs.Student;
using Eras.Error.Bussiness;

using FluentValidation;

namespace Eras.Application.Features.Students.Commands;

internal static class StudentProfileGuards
{
    public static async Task ValidateAsync(
        IValidator<StudentRegistrationDto> Validator,
        StudentRegistrationDto Dto,
        CancellationToken CancellationToken)
    {
        ArgumentNullException.ThrowIfNull(Dto);

        FluentValidation.Results.ValidationResult result = await Validator.ValidateAsync(Dto, CancellationToken);
        if (result.IsValid) return;

        string message = string.Join(" ", result.Errors.Select(Error => Error.ErrorMessage).Distinct());
        throw new BussinessException(message, 400);
    }
}
