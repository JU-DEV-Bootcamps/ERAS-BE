using Eras.Domain.Common;

namespace Eras.Application.DTOs;

public class AnswerDTO
{
    public string Answer { get; set; } = String.Empty;
    public double Score { get; set; }
    public int PollInstanceId { get; set; }
    public int PollVariableId { get; set; }
    public StudentDTO? Student { get; set; }
    public AuditInfo? Audit { get; set; } = default!;
    public VersionInfo Version { get; set; } = default!;
}