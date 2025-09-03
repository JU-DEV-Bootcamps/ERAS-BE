using System.ComponentModel.DataAnnotations;

using Eras.Domain.Common;

namespace Eras.Application.DTOs;
public class JUInterventionDTO
{
    [Required(ErrorMessage = "Intervention Id is required.")]
    [Range(0, 2147483647, ErrorMessage = "Id must be greater than or equals 0.")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Intervention diagnostic is required.")]
    [StringLength(255, MinimumLength = 3, ErrorMessage = "Intervention diagnostic must be between 3 and 255 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9\s\-&.]+$", ErrorMessage = "Intervention diagnostic can only contain letters, numbers, spaces, dashes, dots, and the '&' symbol.")]
    public required string Diagnostic { get; set; }

    [Required(ErrorMessage = "Intervention Objective is required.")]
    [StringLength(255, MinimumLength = 3, ErrorMessage = "Intervention Objective must be between 3 and 255 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9\s\-&.]+$", ErrorMessage = "Intervention Objective can only contain letters, numbers, spaces, dashes, dots, and the '&' symbol.")]
    public required string Objective { get; set; }

    public required int StudentId { get; set; }
    public IEnumerable<JURemissionDTO> Remissions { get; set; } = [];
    public AuditInfo? Audit { get; set; } = default!;
}
