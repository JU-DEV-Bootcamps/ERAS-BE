
using FluentValidation;

namespace Eras.Application.Validation;

public static class ValidationHelper
{
    public static async Task ValidateAndThrowAsync<T>(
        IValidator<T> validator,
        T instance,
        CancellationToken cancellationToken)
    {
        var result = await validator.ValidateAsync(instance, cancellationToken);

        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }
    }
}