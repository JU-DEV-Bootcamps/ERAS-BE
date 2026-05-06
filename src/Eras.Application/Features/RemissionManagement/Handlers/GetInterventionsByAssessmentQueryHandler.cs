using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

using MediatR;

namespace Eras.Application.Features.RemissionManagement.Handlers;

public sealed class GetInterventionsByAssessmentQueryHandler
    : IRequestHandler<GetInterventionsByAssessmentQuery, IReadOnlyCollection<InterventionDto>>
{
    private readonly IAssessmentRepository _repository;
    private readonly IMapper<Assessment, AssessmentDto> _toDtoMapper;

    public GetInterventionsByAssessmentQueryHandler(
        IAssessmentRepository repository,
        IMapper<Assessment, AssessmentDto> toDtoMapper)
    {
        _repository = repository;
        _toDtoMapper = toDtoMapper;
    }

    public async Task<IReadOnlyCollection<InterventionDto>> Handle(
        GetInterventionsByAssessmentQuery request,
        CancellationToken cancellationToken)
    {
        Assessment? assessment = await _repository.GetByIdWithInterventionsAsync(request.AssessmentId);

        if (assessment is null)
            throw new KeyNotFoundException($"Assessment '{request.AssessmentId}' not found.");

        return _toDtoMapper.Map(assessment).Interventions;
    }
}