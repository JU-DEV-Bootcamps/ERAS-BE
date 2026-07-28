using System.ComponentModel.DataAnnotations;

namespace Eras.Application.Tests.DTOs;
public static class ValidationTestHelper
{
    public static IList<ValidationResult> Validate(object Model)
    {
        var context = new ValidationContext(Model, serviceProvider: null, items: null);
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(Model, context, results, validateAllProperties: true);

        return results;
    }
}