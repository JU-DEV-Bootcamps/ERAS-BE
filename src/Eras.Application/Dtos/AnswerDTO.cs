using Eras.Domain.Common;

namespace Eras.Application.DTOs;

public class AnswerDTO
{
    public string Answer { get; set; } = String.Empty;
    public decimal Score { get; set; }
    public int PollInstanceId { get; set; }
    public int PollVariableId { get; set; }
    public StudentDTO? Student { get; set; }
    public AuditInfo? Audit { get; set; } = new AuditInfo()
    {
        CreatedBy = "Default constructor",
        CreatedAt = DateTime.UtcNow,
        ModifiedAt = DateTime.UtcNow,
    };
    public VersionInfo Version { get; set; } = default!;
}