namespace Eras.Application.DTOs.AssessmentManagement;

public sealed record UpdateInterventionRequestDto(
    UpdateInterventionDto UpdateInterventionDto,
    int[]? AttachmentIdsToRemove,
    int? DraftSessionId
);
