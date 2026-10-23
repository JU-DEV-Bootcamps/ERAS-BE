using System.ComponentModel.DataAnnotations;

using Eras.Application.Validation;

using FluentValidation;
using FluentValidation.Results;

using Moq;

namespace Eras.Application.Tests.DTOs;

public static class ValidationTestHelper
{
    public static IList<System.ComponentModel.DataAnnotations.ValidationResult> Validate(object Model)
    {
        var context = new ValidationContext(Model, serviceProvider: null, items: null);
        var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

        Validator.TryValidateObject(Model, context, results, validateAllProperties: true);

        return results;
    }
}

public class ValidationHelperTests
    {
        [Fact]
        public async Task ValidateAndThrowAsync_DoesNotThrow_WhenValidationIsSuccessful()
        {
            var validator = new Mock<IValidator<string>>();
            var instance = "valid";
            var cancellationToken = CancellationToken.None;

            validator
                .Setup(v => v.ValidateAsync(instance, cancellationToken))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            await ValidationHelper.ValidateAndThrowAsync(
                validator.Object,
                instance,
                cancellationToken);
        }

        [Fact]
        public async Task ValidateAndThrowAsync_ThrowsValidationException_WhenValidationFails()
        {
            var validator = new Mock<IValidator<string>>();
            var instance = "invalid";
            var cancellationToken = CancellationToken.None;

            var errors = new List<ValidationFailure>
            {
                new("Name", "Name is required")
            };

            validator
                .Setup(v => v.ValidateAsync(instance, cancellationToken))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult(errors));

            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                ValidationHelper.ValidateAndThrowAsync(
                    validator.Object,
                    instance,
                    cancellationToken));
        }
    }
//}