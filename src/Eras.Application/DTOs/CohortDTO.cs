using Eras.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Eras.Application.DTOs;

[ExcludeFromCodeCoverage]
public class CohortDTO
{
    [Required(ErrorMessage = "Cohort name is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Cohort name must be between 3 and 50 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9\s\-\(\)]+$", ErrorMessage = "Cohort name can only contain letters, numbers, spaces, and hyphens, and parentheses.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "Course code must be less than 50 characters.")]
    [RegularExpression(@"^[A-Z0-9\-]*$", ErrorMessage = "Course code can only contain uppercase letters, numbers, and hyphens.")]
    public string CourseCode { get; set; } = string.Empty;

    public AuditInfo? Audit { get; set; } = default!;
}
