using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Eras.Domain.Common;

namespace Eras.Application.DTOs;

[ExcludeFromCodeCoverage]
public class ComponentDTO
{
    [Required(ErrorMessage = "Component name is required.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Component name must be between 3 and 100 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9\s\-_.]+$", ErrorMessage = "Component name can only contain letters, numbers, spaces, dashes, underscores, and dots.")]
    public string Name { get; set; } = string.Empty;
    public ICollection<VariableDTO> Variables { get; set; } = [];
    public AuditInfo? Audit { get; set; } = default!;
}
