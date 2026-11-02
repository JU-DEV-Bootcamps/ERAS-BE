using Eras.Domain.Entities;

namespace Eras.Application.DTOs.StudentInterventions;

/// <summary>
/// Body to add or edit a risk-identified or support-received entry. Every field is nullable so a
/// missing one is reported as a validation error naming it; the creator and the creation time are never read from here.
/// </summary>
public sealed class SaveStudentInterventionDto
{
    public StudentInterventionKind? Kind { get; set; }
    public StudentInterventionType? Type { get; set; }
    public string? Description { get; set; }
    public DateOnly? EntryDate { get; set; }
}
