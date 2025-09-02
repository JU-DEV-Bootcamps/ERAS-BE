using System.ComponentModel.DataAnnotations;

using Eras.Domain.Common;

namespace Eras.Application.DTOs;
public class JUProfessionalDTO
{
    [Required(ErrorMessage = "Configuration Id is required.")]
    [Range(1, 2147483647, ErrorMessage = "Id must be greater than 0.")]
    public int? Id { get; set; }

    [Required(ErrorMessage = "Professional name is required.")]
    [StringLength(255, MinimumLength = 3, ErrorMessage = "Professional name must be between 3 and 255 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9\s\-&.]+$", ErrorMessage = "Professional name can only contain letters, numbers, spaces, dashes, dots, and the '&' symbol.")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Professional UUID is required.")]
    [StringLength(36, MinimumLength = 36, ErrorMessage = "Professional UUID must be exactly 36 characters.")]
    [RegularExpression(@"^[a-fA-F0-9\-]{36}$", ErrorMessage = "Professional must follow a valid GUID format.")]
    public string Uuid { get; set; } = string.Empty;
    public AuditInfo Audit { get; set; }
}
