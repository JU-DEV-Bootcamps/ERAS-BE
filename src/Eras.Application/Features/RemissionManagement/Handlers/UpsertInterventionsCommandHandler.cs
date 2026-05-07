using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

using MediatR;

namespace Eras.Application.Features.RemissionManagement.Handlers;

public sealed class UpsertInterventionsCommandHandler
    : IRequestHandler<UpsertInterventionsCommand, IReadOnlyCollection<InterventionDto>>
{
    private readonly IAssessmentRepository _repository;
    private readonly IMapper<IndividualInterventionDto, IndividualIntervention> _individualMapper;
    private readonly IMapper<GroupInterventionDto, GroupIntervention> _groupMapper;
    private readonly IMapper<Assessment, AssessmentDto> _toDtoMapper;

    public UpsertInterventionsCommandHandler(
        IAssessmentRepository repository,
        IMapper<IndividualInterventionDto, IndividualIntervention> individualMapper,
        IMapper<GroupInterventionDto, GroupIntervention> groupMapper,
        IMapper<Assessment, AssessmentDto> toDtoMapper)
    {
        _repository = repository;
        _individualMapper = individualMapper;
        _groupMapper = groupMapper;
        _toDtoMapper = toDtoMapper;
    }

    public async Task<IReadOnlyCollection<InterventionDto>> Handle(
        UpsertInterventionsCommand request,
        CancellationToken cancellationToken)
    {
        Assessment? assessment = await _repository.GetByIdWithInterventionsAsync(request.AssessmentId);

        if (assessment is null)
            throw new KeyNotFoundException($"Assessment '{request.AssessmentId}' not found.");

        IReadOnlyCollection<Intervention> newInterventions = MapInterventions(request.Interventions);

        await _repository.ReplaceInterventionsAsync(request.AssessmentId, newInterventions);

        return request.Interventions;
    }

    private IReadOnlyCollection<Intervention> MapInterventions(
        IReadOnlyCollection<InterventionDto> dtos)
    {
        List<Intervention> result = new(dtos.Count);

        foreach (InterventionDto dto in dtos)
        {
            Intervention mapped = dto switch
            {
                IndividualInterventionDto individual => _individualMapper.Map(individual),
                GroupInterventionDto group => _groupMapper.Map(group),
                _ => throw new NotSupportedException(
                    $"Intervention DTO type '{dto.GetType().Name}' is not supported.")
            };

            result.Add(mapped);
        }

        return result;
    }
}