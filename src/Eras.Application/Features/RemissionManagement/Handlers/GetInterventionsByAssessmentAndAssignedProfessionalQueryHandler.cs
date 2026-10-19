using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

using MediatR;

namespace Eras.Application.Features.RemissionManagement.Handlers;

public sealed class GetInterventionsByAssessmentAndAssignedProfessionalQueryHandler(
    IAssessmentRepository Repository,
    IMapper<Assessment, AssessmentDto> ToDtoMapper
) : IRequestHandler<GetInterventionsByAssessmentAndAssignedProfessionalQuery, IReadOnlyCollection<InterventionDto>>
{
    private readonly IAssessmentRepository _repository = Repository;
    private readonly IMapper<Assessment, AssessmentDto> _toDtoMapper = ToDtoMapper;

    public async Task<IReadOnlyCollection<InterventionDto>> Handle(
        GetInterventionsByAssessmentAndAssignedProfessionalQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.ProfessionalSub);

        Assessment? assessment = await _repository.GetByIdWithInterventionsAsync(request.AssessmentId);

        if (assessment is null)
            throw new KeyNotFoundException($"Assessment '{request.AssessmentId}' not found.");

        return _toDtoMapper.Map(assessment).Interventions
            .Where(i => i.CreatedBy == request.ProfessionalSub
                     || (request.ProfessionalName is not null && i.Professional == request.ProfessionalName))
            .OrderBy(i => i.DateUtc)
            .ToList();
    }
}
