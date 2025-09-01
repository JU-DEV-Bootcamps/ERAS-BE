using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Eras.Application.Attributes;
using Eras.Domain.Common;

namespace Eras.Application.DTOs;

[ExcludeFromCodeCoverage]
public class ComponentDTO
{
    [Required(ErrorMessage = "Component name is required.")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "Component name must be between 3 and 30 characters.")]
    [NoSqlInjection]
    public string Name { get; set; } = string.Empty;

    public ICollection<VariableDTO> Variables { get; set; } = [];
    public AuditInfo? Audit { get; set; } = default!;
}
