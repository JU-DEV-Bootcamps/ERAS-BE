using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

using FluentAssertions;

namespace Eras.Application.Tests.Mappers.AssessmentsManagement;

public sealed class AssessmentToDtoMapperTests
{
    [Fact]
    public void Map_ShouldMapAssessmentProperties()
    {
        Assessment source = new()
        {
            Id = 1,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = "user",
            Service = "Service",
            AssignedProfessional = "Professional",
            StudentIds = [1],
            Diagnosis = "Diagnosis",
            Objective = "Objective",
            Comments = "Comments",
            Status = AssessmentStatus.Finalized,
            Interventions = []
        };

        AssessmentDto result = CreateSut().Map(source);

        Assert.Equal(source.Id, result.Id);
        Assert.Equal(source.CreatedAtUtc, result.CreatedAtUtc);
        Assert.Equal(source.CreatedBy, result.CreatedBy);
        Assert.Equal(source.Service, result.Service);
        Assert.Equal(source.AssignedProfessional, result.AssignedProfessional);
        Assert.Equal(source.StudentIds, result.StudentIds);
        Assert.Equal(source.Diagnosis, result.Diagnosis);
        Assert.Equal(source.Objective, result.Objective);
        Assert.Equal(source.Comments, result.Comments);
        Assert.Equal(source.Status, result.Status);
    }

    [Fact]
    public void Map_WhenPlanExists_ShouldMapPlan()
    {
        var plan = new InterventionPlan();
        var planDto = new InterventionPlanDto();

        var planMapper = new FakeMapper<InterventionPlan, InterventionPlanDto>(planDto);

        Assessment source = new()
        {
            Plan = plan,
            Interventions = [],
            CreatedBy = "one",
            StudentIds = [1],
            Service = "call",
            Status = AssessmentStatus.Remitted
        };

        AssessmentDto result = CreateSut(planMapper).Map(source);

        Assert.Same(planDto, result.Plan);
        Assert.Same(plan, planMapper.MappedSource);
    }

    [Fact]
    public void Map_WhenPlanIsNull_ShouldReturnNullPlan()
    {
        Assessment source = new()
        {
            Plan = null,
            Interventions = [],
            CreatedBy = "one",
            StudentIds = [1],
            Service = "call",
            Status = AssessmentStatus.Remitted
        };

        AssessmentDto result = CreateSut().Map(source);

        Assert.Null(result.Plan);
    }

    [Fact]
    public void Map_WhenNoInterventions_ShouldReturnEmptyCollection()
    {
        Assessment source = new()
        {
            Interventions = [],
            CreatedBy = "one",
            StudentIds = [1],
            Service = "call",
            Status = AssessmentStatus.Remitted
        };

        AssessmentDto result = CreateSut().Map(source);

        Assert.Empty(result.Interventions);
    }

    [Fact]
    public void Map_WhenIndividualIntervention_ShouldMapIndividual()
    {
        var intervention = new IndividualIntervention() {
            DateUtc = DateTime.UtcNow,
            StudentIds = [1]
        };
        var interventionDto = new IndividualInterventionDto() {
            DateUtc = DateTime.UtcNow,
            StudentIds = [1]
        };

        var individualMapper =
            new FakeMapper<IndividualIntervention, IndividualInterventionDto>(interventionDto);

        Assessment source = new()
        {
            Interventions = [intervention],
            CreatedBy = "one",
            StudentIds = [1],
            Service = "call",
            Status = AssessmentStatus.Remitted
        };

        AssessmentDto result = CreateSut(individualMapper: individualMapper).Map(source);

        Assert.Single(result.Interventions);
        Assert.Same(interventionDto, result.Interventions.Single());
        Assert.Same(intervention, individualMapper.MappedSource);
    }

    [Fact]
    public void Map_WhenGroupIntervention_ShouldMapGroup()
    {
        var intervention = new GroupIntervention() {
            DateUtc = DateTime.UtcNow,
            StudentIds = [1, 3]
        };
        var interventionDto = new GroupInterventionDto() {
            DateUtc = DateTime.UtcNow,
            StudentIds = [1, 3]
        };

        var groupMapper = new FakeMapper<GroupIntervention, GroupInterventionDto>(interventionDto);

        Assessment source = new()
        {
            Interventions = [intervention],
            CreatedBy = "one",
            StudentIds = [1],
            Service = "call",
            Status = AssessmentStatus.Remitted
        };

        AssessmentDto result = CreateSut(groupMapper: groupMapper).Map(source);

        Assert.Single(result.Interventions);
        Assert.Same(interventionDto, result.Interventions.Single());
        Assert.Same(intervention, groupMapper.MappedSource);
    }

    [Fact]
    public void Map_WhenUnsupportedInterventionType_ShouldThrow()
    {
        var intervention = new UnsupportedIntervention() {
            DateUtc = DateTime.UtcNow,
            StudentIds = [1]
        };

        Assessment source = new()
        {
            Interventions = [intervention],
            CreatedBy = "one",
            StudentIds = [1],
            Service = "call",
            Status = AssessmentStatus.Remitted
        };

        Assert.Throws<NotSupportedException>(() => CreateSut().Map(source));
    }

    private static AssessmentToDtoMapper CreateSut(
        IMapper<InterventionPlan, InterventionPlanDto>? planMapper = null,
        IMapper<IndividualIntervention, IndividualInterventionDto>? individualMapper = null,
        IMapper<GroupIntervention, GroupInterventionDto>? groupMapper = null)
    {
        return new AssessmentToDtoMapper(
            planMapper ?? new FakeMapper<InterventionPlan, InterventionPlanDto>(new()),
            individualMapper ?? new FakeMapper<IndividualIntervention, IndividualInterventionDto>(new() {
                DateUtc = DateTime.UtcNow,
                StudentIds = [2]
            }),
            groupMapper ?? new FakeMapper<GroupIntervention, GroupInterventionDto>(new() {
                DateUtc = DateTime.UtcNow,
                StudentIds = [1, 3]
            }));
    }

    private sealed class FakeMapper<TSource, TDestination> : IMapper<TSource, TDestination>
    {
        private readonly TDestination _destination;

        public FakeMapper(TDestination destination)
        {
            _destination = destination;
        }

        public TSource? MappedSource { get; private set; }

        public TDestination Map(TSource source)
        {
            MappedSource = source;
            return _destination;
        }
    }

    private sealed class UnsupportedIntervention : Intervention
    {
    }
}

