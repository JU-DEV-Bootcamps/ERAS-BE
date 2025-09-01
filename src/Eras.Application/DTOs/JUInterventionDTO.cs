using System.ComponentModel.DataAnnotations;

using Eras.Domain.Common;

namespace Eras.Application.DTOs;
public class JUInterventionDTO
{
    [Required(ErrorMessage = "Intervention Id is required.")]
    [Range(1, 2147483647, ErrorMessage = "Id must be greater than 0.")]
    public int? Id { get; set; }

    [Required(ErrorMessage = "Intervention diagnostic is required.")]
    [StringLength(255, MinimumLength = 3, ErrorMessage = "Intervention diagnostic must be between 3 and 255 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9\s\-&.]+$", ErrorMessage = "Intervention diagnostic can only contain letters, numbers, spaces, dashes, dots, and the '&' symbol.")]
    public string Diagnostic { get; set; }

    [Required(ErrorMessage = "Intervention Objective is required.")]
    [StringLength(255, MinimumLength = 3, ErrorMessage = "Intervention Objective must be between 3 and 255 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9\s\-&.]+$", ErrorMessage = "Intervention Objective can only contain letters, numbers, spaces, dashes, dots, and the '&' symbol.")]
    public string Objective { get; set; }

    [Required(ErrorMessage = "Intervention UUID is required.")]
    [StringLength(36, MinimumLength = 36, ErrorMessage = "Intervention UUID must be exactly 36 characters.")]
    [RegularExpression(@"^[a-fA-F0-9\-]{36}$", ErrorMessage = "Intervention must follow a valid GUID format.")]
    public string Uuid { get; set; } = string.Empty;

    public StudentDTO Student { get; set; } = default!;
    public IEnumerable<JURemissionDTO> Remissions { get; set; } = [];
    public AuditInfo Audit { get; set; }
}
