using Eras.Domain.Entities;

namespace Eras.Application.DTOs.StudentInterventions;

public sealed class StudentInterventionDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public StudentInterventionKind Kind { get; set; }
    public StudentInterventionType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateOnly EntryDate { get; set; }

    /// <summary>Keycloak sub of the creator; the FE compares it with the signed-in user to know whose entry it is.</summary>
    public string CreatedBy { get; set; } = string.Empty;
    public string CreatedByName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
