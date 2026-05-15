using System.Text.Json.Serialization;

using Eras.Domain.Entities.AssessmentManagement;

namespace Eras.Application.DTOs.AssessmentManagement;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(IndividualInterventionDto), "Individual")]
[JsonDerivedType(typeof(GroupInterventionDto), "Group")]
public abstract record InterventionDto
{
    public int? Id { get; init; }

    public required DateTime DateUtc { get; init; }
    public string? Activity { get; init; }
    public string? Area { get; init; }
    public int? NumberOfGuests { get; init; }
    public int? NumberOfParticipants { get; init; }
    public string? Professional { get; init; }
    public string? Comments { get; init; }
    public required IReadOnlyCollection<int> StudentIds { get; init; } = Array.Empty<int>();

    /// <summary>
    /// Maps StudentId → attended (true/false).
    /// Individual: pre-set to true for the single student, desmarcable.
    /// Group: set manually per student.
    /// </summary>
    public IReadOnlyDictionary<int, bool> Attendance { get; init; } = new Dictionary<int, bool>();

    public InterventionMode Mode { get; init; }
    public InterventionStatus Status { get; init; } = InterventionStatus.Created;
    public string? Remarks { get; init; }
    public IReadOnlyCollection<string> Attachments { get; init; } = Array.Empty<string>();
}