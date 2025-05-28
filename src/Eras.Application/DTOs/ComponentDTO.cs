using System.Diagnostics.CodeAnalysis;
using Eras.Domain.Common;

namespace Eras.Application.DTOs;

[ExcludeFromCodeCoverage]
public class ComponentDTO
{
    public string Name { get; set; } = String.Empty;
    public ICollection<VariableDTO> Variables { get; set; } = [];
    public AuditInfo Audit { get; set; } = default!;
}
