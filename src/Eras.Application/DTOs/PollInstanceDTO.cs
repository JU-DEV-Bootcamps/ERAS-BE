using System.ComponentModel.DataAnnotations;

using Eras.Domain.Common;

namespace Eras.Application.DTOs;

public class PollInstanceDTO
{
    public int Id { get; set; }

    [Required(ErrorMessage = "UUID is required.")]
    [RegularExpression(@"^[a-fA-F0-9\-]{36}$", ErrorMessage = "UUID format is invalid.")]
    public string Uuid { get; set; } = string.Empty;
    public StudentDTO Student { get; set; } = default!;
    public ICollection<AnswerDTO> Answers { get; set; } = [];
    public AuditInfo Audit { get; set; } = default!;
    public DateTime FinishedAt { get; set; }

    [Required(ErrorMessage = "Last version is required.")]
    [Range(1, 32767, ErrorMessage = "Last version must be at least 1.")]
    public int LastVersion { get; set; }
    
    [Required(ErrorMessage = "Last version date is required.")]
    [DataType(DataType.DateTime)]
    public DateTime LastVersionDate { get; set; }
    public int? EvaluationId { get; set; }

    [StringLength(64, ErrorMessage = "Answers hash must be 64 characters long.")]
    public string? AnswersHash { get; set; }
}
