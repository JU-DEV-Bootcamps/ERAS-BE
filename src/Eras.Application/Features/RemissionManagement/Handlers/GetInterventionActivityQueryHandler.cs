using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

using MediatR;

namespace Eras.Application.Features.RemissionManagement.Handlers;

public sealed class GetInterventionActivityQueryHandler(
    IAssessmentRepository AssessmentRepository,
    IInterventionActivityRepository ActivityRepository,
    IMapper<InterventionActivity, InterventionActivityDto> Mapper)
    : IRequestHandler<GetInterventionActivityQuery, IReadOnlyCollection<InterventionActivityDto>>
{
    public async Task<IReadOnlyCollection<InterventionActivityDto>> Handle(
        GetInterventionActivityQuery Request,
        CancellationToken CancellationToken)
    {
        Assessment? assessment = await AssessmentRepository.GetByIdWithInterventionsAsync(Request.AssessmentId);
        if (assessment is null)
            throw new KeyNotFoundException($"Assessment '{Request.AssessmentId}' not found.");

        // The intervention must belong to the assessment of the route.
        if (!assessment.Interventions.Any(I => I.Id == Request.InterventionId))
            throw new KeyNotFoundException(
                $"Intervention '{Request.InterventionId}' not found in assessment '{Request.AssessmentId}'.");

        IReadOnlyList<InterventionActivity> activities =
            await ActivityRepository.GetByInterventionIdAsync(Request.InterventionId, CancellationToken);

        return activities
            .OrderBy(A => A.OccurredAtUtc)
            .ThenBy(A => A.Id)
            .Select(Mapper.Map)
            .ToList();
    }
}
