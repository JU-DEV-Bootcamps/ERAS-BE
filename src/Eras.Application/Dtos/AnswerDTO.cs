using System.ComponentModel.DataAnnotations;
using Eras.Domain.Common;

namespace Eras.Application.DTOs;

public class AnswerDTO
{
    [Required(ErrorMessage = "Answer text is required.")]
    [StringLength(500, MinimumLength = 1, ErrorMessage = "Answer must be between 1 and 500 characters.")]
    public string Answer { get; set; } = string.Empty;

    [Range(0, 100, ErrorMessage = "Score must be between 0 and 100.")]
    public decimal Score { get; set; }

    [Required(ErrorMessage = "PollInstanceId is required.")]
    [Range(1, 2147483647, ErrorMessage = "Poll Instance Id must be greater than 0.")]
    public int PollInstanceId { get; set; }

    [Required(ErrorMessage = "PollVariableId is required.")]
    [Range(1, 2147483647, ErrorMessage = "Poll Variable Id must be greater than 0.")]
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