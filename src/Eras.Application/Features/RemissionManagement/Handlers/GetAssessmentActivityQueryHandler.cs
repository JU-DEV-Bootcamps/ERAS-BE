using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;
using Eras.Error.Bussiness;

using MediatR;

namespace Eras.Application.Features.RemissionManagement.Handlers;

public sealed class GetAssessmentActivityQueryHandler(
    IAssessmentRepository AssessmentRepository,
    IAssessmentActivityRepository ActivityRepository,
    IMapper<AssessmentActivity, AssessmentActivityDto> Mapper)
    : IRequestHandler<GetAssessmentActivityQuery, IReadOnlyCollection<AssessmentActivityDto>>
{
    public async Task<IReadOnlyCollection<AssessmentActivityDto>> Handle(
        GetAssessmentActivityQuery Request,
        CancellationToken CancellationToken)
    {
        Assessment? assessment = await AssessmentRepository.GetByIdAsync(Request.AssessmentId);
        if (assessment is null)
            throw new NotFoundException($"Assessment '{Request.AssessmentId}' not found.");

        IReadOnlyList<AssessmentActivity> activities =
            await ActivityRepository.GetByAssessmentIdAsync(Request.AssessmentId, CancellationToken);

        return activities
            .OrderBy(A => A.OccurredAtUtc)
            .ThenBy(A => A.Id)
            .Select(Mapper.Map)
            .ToList();
    }
}
