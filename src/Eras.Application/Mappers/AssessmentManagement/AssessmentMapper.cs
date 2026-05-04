using Eras.Application.DTOs.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

namespace Eras.Application.Mappers.AssessmentManagement;

public sealed class AssessmentMapper : IMapper<AssessmentDto, Assessment>
{
    private readonly IMapper<InterventionPlanDto, InterventionPlan> _planMapper;
    private readonly IMapper<IndividualInterventionDto, IndividualIntervention> _individualMapper;
    private readonly IMapper<GroupInterventionDto, GroupIntervention> _groupMapper;

    public AssessmentMapper(
        IMapper<InterventionPlanDto, InterventionPlan> planMapper,
        IMapper<IndividualInterventionDto, IndividualIntervention> individualMapper,
        IMapper<GroupInterventionDto, GroupIntervention> groupMapper)
    {
        _planMapper = planMapper;
        _individualMapper = individualMapper;
        _groupMapper = groupMapper;
    }

    public Assessment Map(AssessmentDto source)
    {
        return new Assessment
        {
            Id = source.Id ?? default,

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

    private IReadOnlyCollection<Intervention> MapInterventions(
        IReadOnlyCollection<InterventionDto> interventions)
    {
        if (interventions.Count == 0)
        {
            return Array.Empty<Intervention>();
        }

        List<Intervention> mapped = new(interventions.Count);

        foreach (InterventionDto dto in interventions)
        {
            mapped.Add(MapIntervention(dto));
        }

        return mapped;
    }

    private Intervention MapIntervention(InterventionDto source)
    {
        return source switch
        {
            IndividualInterventionDto individual => _individualMapper.Map(individual),
            GroupInterventionDto group => _groupMapper.Map(group),
            _ => throw new NotSupportedException(
                $"Intervention DTO type '{source.GetType().Name}' is not supported.")
        };
    }
}
