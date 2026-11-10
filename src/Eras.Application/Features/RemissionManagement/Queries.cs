using Eras.Application.DTOs.AssessmentManagement;

using MediatR;

namespace Eras.Application.Features.RemissionManagement;

public sealed record GetAssessmentsByCreatorQuery(string creatorSub) : IRequest<IEnumerable<AssessmentDto>>;
public sealed record GetAssessmentsByAssignedProfessionalQuery(string assignedProfessionalSub) : IRequest<IEnumerable<AssessmentDto>>;

/// <summary>The activity history of one intervention of the assessment, oldest event first.</summary>
public sealed record GetInterventionActivityQuery(int AssessmentId, int InterventionId)
    : IRequest<IReadOnlyCollection<InterventionActivityDto>>;

/// <summary>The activity history of one assessment, oldest event first.</summary>
public sealed record GetAssessmentActivityQuery(int AssessmentId)
    : IRequest<IReadOnlyCollection<AssessmentActivityDto>>;
