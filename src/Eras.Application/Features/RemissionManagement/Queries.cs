using Eras.Application.DTOs.AssessmentManagement;

using MediatR;

namespace Eras.Application.Features.RemissionManagement;

public sealed record GetAssessmentsByCreatorQuery(string creatorSub) : IRequest<IEnumerable<AssessmentDto>>;
public sealed record GetAssessmentsByAssignedProfessionalQuery(string assignedProfessionalSub) : IRequest<IEnumerable<AssessmentDto>>;