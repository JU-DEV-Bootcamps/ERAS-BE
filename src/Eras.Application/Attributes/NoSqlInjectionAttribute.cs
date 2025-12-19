using System.ComponentModel.DataAnnotations;
using Eras.Application.Utils;

namespace Eras.Application.Attributes
{
    public class NoSqlInjectionAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            return SqlInjectionValidator.IsValid(value?.ToString());
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (SqlInjectionValidator.IsValid(value?.ToString()))
            {
                return ValidationResult.Success;
            }

            string fieldName = validationContext.DisplayName ?? validationContext.MemberName ?? "Field";
            return new ValidationResult($"The field {fieldName} contains potentially unsafe content that could be used for SQL injection attacks.");
        }
    }
}