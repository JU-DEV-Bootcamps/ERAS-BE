using FluentValidation;

namespace Eras.Domain.Entities;

internal static class ValidationExtensions
{
    public static void ValidateAndThrow<T>(this IValidator<T> validator, T instance)
    {
        validator.Validate(instance).ThrowIfInvalid();
    }

    public static void ThrowIfInvalid(this FluentValidation.Results.ValidationResult result)
    {
        if (result.IsValid)
        {
            return;
        }

        string message = string.Join(Environment.NewLine, result.Errors.Select(x => x.ErrorMessage));
        throw new ValidationException(message, result.Errors);
    }
}
