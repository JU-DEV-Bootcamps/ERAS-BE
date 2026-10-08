using Eras.Application.DTOs.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

using MediatR;

namespace Eras.Application.Features.RemissionManagement;

public sealed record CreateRemissionCommand(AssessmentDto Remission) : IRequest<AssessmentDto>;

public sealed record UpdateRemissionCommand(AssessmentDto Remission) : IRequest<AssessmentDto>;

public sealed record GetRemissionByIdQuery(int Id) : IRequest<AssessmentDto?>;

public sealed record GetRemissionsByStudentIdQuery(int StudentId) : IRequest<IReadOnlyCollection<AssessmentDto>>;

public sealed record GetRemissionsByStatusQuery(AssessmentStatus Status)
    : IRequest<IReadOnlyCollection<AssessmentDto>>;

public sealed record GetAllRemissionsQuery() : IRequest<IReadOnlyCollection<AssessmentDto>>;

public sealed record DeleteAssessmentCommand(int AssessmentId) : IRequest;

public sealed record UpsertInterventionsCommand(int AssessmentId, IReadOnlyCollection<InterventionDto> Interventions)
    : IRequest<IReadOnlyCollection<InterventionDto>>;

public sealed record GetInterventionsByAssessmentQuery(int AssessmentId)
    : IRequest<IReadOnlyCollection<InterventionDto>>;

/// <summary>
/// Interventions of the assessment, but only when the assessment was created by
/// <paramref name="CreatorSub"/> — otherwise empty (#545, Student Services Officer scope).
/// </summary>
public sealed record GetInterventionsByAssessmentAndCreatorQuery(int AssessmentId, string CreatorSub)
    : IRequest<IReadOnlyCollection<InterventionDto>>;

/// <summary>
/// Interventions of the assessment scoped to a Professional: those created by
/// <paramref name="ProfessionalSub"/> (<see cref="Eras.Domain.Entities.AssessmentManagement.Intervention.CreatedBy"/>)
/// or assigned to them (<see cref="Eras.Domain.Entities.AssessmentManagement.Intervention.Professional"/> ==
/// <paramref name="ProfessionalName"/>), combined with OR (#545).
/// </summary>
public sealed record GetInterventionsByAssessmentAndAssignedProfessionalQuery(
    int AssessmentId,
    string ProfessionalSub,
    string? ProfessionalName = null)
    : IRequest<IReadOnlyCollection<InterventionDto>>;

/// <param name="DraftSessionId">
/// Optional id of a draft session (see <c>Eras.Domain.Entities.AttachmentDraftSession</c>) whose
/// staged attachments should be claimed for this intervention as part of its creation.
/// </param>
public sealed record AddInterventionCommand(int AssessmentId, InterventionDto Intervention, int? DraftSessionId = null)
    : IRequest<InterventionDto>;

public sealed record DeleteInterventionCommand(int AssessmentId, int InterventionId)
    : IRequest;

public sealed record UploadInterventionAttachmentsCommand(
    int InterventionId,
    IReadOnlyCollection<(Stream Stream, string FileName)> Files
) : IRequest<IReadOnlyCollection<string>>;

public sealed record DeleteInterventionAttachmentCommand(
    int InterventionId,
    string FileName
) : IRequest;

public sealed record UpdateInterventionCommand(int AssessmentId, int InterventionId,
    UpdateInterventionDto Intervention, int[]? AttachmentIdsToRemove, int? DraftSessionId) : IRequest<UpdateInterventionDto>;

public sealed record ReplaceInterventionCommand(int AssessmentId, int OldInterventionId,
    UpdateInterventionDto NewIntervention, int[]? AttachmentIdsToRemove, int? DraftSessionId) : IRequest<UpdateInterventionDto>;
