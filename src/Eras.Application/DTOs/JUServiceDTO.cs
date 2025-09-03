using System.ComponentModel.DataAnnotations;

using Eras.Domain.Common;

namespace Eras.Application.DTOs;
public class JUServiceDTO 
{
    [Required(ErrorMessage = "JUService Id is required.")]
    [Range(0, 2147483647, ErrorMessage = "Id must be greater than or equals 0.")]
    public int Id { get; set; }

    [Required(ErrorMessage = "JUService name is required.")]
    [StringLength(255, MinimumLength = 3, ErrorMessage = "JUService name must be between 3 and 255 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9\s\-&.]+$", ErrorMessage = "JUService name can only contain letters, numbers, spaces, dashes, dots, and the '&' symbol.")]
    public required string Name { get; set; }

    public AuditInfo? Audit { get; set; } = default!;
}
