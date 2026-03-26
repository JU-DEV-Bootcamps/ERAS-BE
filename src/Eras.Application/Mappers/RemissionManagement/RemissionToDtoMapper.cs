
using Eras.Application.DTOs.RemissionManagement;
using Eras.Domain.Entities.RemissionsManagement;

namespace Eras.Application.Mappers.RemissionManagement;

public sealed class RemissionToDtoMapper : IMapper<Remission, RemissionDto>
{
    private readonly IMapper<InterventionPlan, InterventionPlanDto> _planMapper;
    private readonly IMapper<IndividualIntervention, IndividualInterventionDto> _individualMapper;
    private readonly IMapper<GroupIntervention, GroupInterventionDto> _groupMapper;

    public RemissionToDtoMapper(
        IMapper<InterventionPlan, InterventionPlanDto> planMapper,
        IMapper<IndividualIntervention, IndividualInterventionDto> individualMapper,
        IMapper<GroupIntervention, GroupInterventionDto> groupMapper)
    {
        _planMapper = planMapper;
        _individualMapper = individualMapper;
        _groupMapper = groupMapper;
    }

    public RemissionDto Map(Remission source)
    {
        return new RemissionDto
        {
            Id = source.Id,
            CreatedAtUtc = source.CreatedAtUtc,
            CreatedBy = source.CreatedBy,
            Service = source.Service,
            AssignedProfessional = source.AssignedProfessional,
            StudentIds = source.StudentIds,
            Diagnosis = source.Diagnosis,
            Objective = source.Objective,
            Comments = source.Comments,
            Plan = source.Plan is null ? null : _planMapper.Map(source.Plan),
            Status = source.Status,
            Interventions = MapInterventions(source.Interventions)
        };
    }

    private IReadOnlyCollection<InterventionDto> MapInterventions(IReadOnlyCollection<Intervention> interventions)
    {
        if (interventions.Count == 0)
        {
            return Array.Empty<InterventionDto>();
        }

        List<InterventionDto> mapped = new(interventions.Count);

        foreach (Intervention intervention in interventions)
        {
            mapped.Add(MapIntervention(intervention));
        }

        return mapped;
    }

    private InterventionDto MapIntervention(Intervention source)
    {
        return source switch
        {
            IndividualIntervention individual => _individualMapper.Map(individual),
            GroupIntervention group => _groupMapper.Map(group),
            _ => throw new NotSupportedException($"Intervention type '{source.GetType().Name}' is not supported.")
        };
    }
}