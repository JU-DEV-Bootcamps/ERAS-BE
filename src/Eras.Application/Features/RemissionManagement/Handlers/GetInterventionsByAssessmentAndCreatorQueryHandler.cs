using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

using MediatR;

namespace Eras.Application.Features.RemissionManagement.Handlers;

public sealed class GetInterventionsByAssessmentAndCreatorQueryHandler(
    IAssessmentRepository Repository,
    IMapper<Assessment, AssessmentDto> ToDtoMapper
) : IRequestHandler<GetInterventionsByAssessmentAndCreatorQuery, IReadOnlyCollection<InterventionDto>>
{
    private readonly IAssessmentRepository _repository = Repository;
    private readonly IMapper<Assessment, AssessmentDto> _toDtoMapper = ToDtoMapper;

    public async Task<IReadOnlyCollection<InterventionDto>> Handle(
        GetInterventionsByAssessmentAndCreatorQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.CreatorSub);

        Assessment? assessment = await _repository.GetByIdWithInterventionsAsync(request.AssessmentId);

        if (assessment is null)
            throw new KeyNotFoundException($"Assessment '{request.AssessmentId}' not found.");

        if (assessment.CreatedBy != request.CreatorSub)
            return Array.Empty<InterventionDto>();

        return _toDtoMapper.Map(assessment).Interventions.OrderBy(i => i.DateUtc).ToList();
    }
}
