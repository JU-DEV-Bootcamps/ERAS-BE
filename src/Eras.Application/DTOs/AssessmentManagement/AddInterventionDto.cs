namespace Eras.Application.DTOs.AssessmentManagement;

public sealed record AddInterventionDto
{
    public required int AssessmentId { get; init; }
    public required InterventionDto Intervention { get; init; }

    /// <summary>
    /// Optional id of a draft session (see <c>Eras.Domain.Entities.AttachmentDraftSession</c>)
    /// whose staged attachments should be claimed for this intervention as part of its creation.
    /// </summary>
    public int? DraftSessionId { get; init; }
}