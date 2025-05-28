using Eras.Domain.Common;
using System.Diagnostics.CodeAnalysis;

namespace Eras.Application.DTOs;

[ExcludeFromCodeCoverage]
public class CohortDTO
{
    public string Name { get; set; } = string.Empty;
    public string CourseCode { get; set; } = string.Empty;
    public AuditInfo Audit { get; set; } = default!;
}
