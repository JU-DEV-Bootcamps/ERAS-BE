using System.ComponentModel.DataAnnotations;
using Eras.Application.Attributes;
using Eras.Domain.Common;

namespace Eras.Application.DTOs;

public class VariableDTO
{
    [Required(ErrorMessage = "Variable name is required.")]
    [StringLength(250, MinimumLength = 2, ErrorMessage = "Variable name must be between 2 and 250 characters.")]
    [NoSqlInjection]
    public string Name { get; set; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "Position must be zero or a positive integer.")]
    public int Position { get; set; } = 0;

    [Required(ErrorMessage = "Type is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Type must be between 3 and 50 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9_\-]+$", ErrorMessage = "Type can only contain letters, numbers, dashes, and underscores.")]
    public string Type { get; set; } = string.Empty;

    public AnswerDTO? Answer { get; set; }
    public AuditInfo? Audit { get; set; } = default!;
    public VersionInfo Version { get; set; } = default!;
}
