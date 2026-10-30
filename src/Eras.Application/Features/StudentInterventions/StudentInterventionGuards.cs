using Eras.Application.DTOs.StudentInterventions;
using Eras.Error.Bussiness;

using FluentValidation;

namespace Eras.Application.Features.StudentInterventions;

internal static class StudentInterventionGuards
{
    public static async Task ValidateAsync(
        IValidator<SaveStudentInterventionDto> Validator,
        SaveStudentInterventionDto Dto,
        CancellationToken CancellationToken)
    {
        ArgumentNullException.ThrowIfNull(Dto);

        FluentValidation.Results.ValidationResult result = await Validator.ValidateAsync(Dto, CancellationToken);
        if (result.IsValid) return;

        string message = string.Join(" ", result.Errors.Select(Error => Error.ErrorMessage).Distinct());
        throw new BussinessException(message, 400);
    }
}
