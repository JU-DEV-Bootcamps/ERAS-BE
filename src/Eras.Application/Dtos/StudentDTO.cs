using Eras.Domain.Common;

namespace Eras.Application.DTOs;

public class StudentDTO
{
    public int Id { get; set; }
    public string Uuid { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsImported { get; set; }
    public StudentDetailDTO? StudentDetail { get; set; }
    public CohortDTO Cohort { get; set; } = default!;
    public AuditInfo? Audit { get; set; } = default!;
}