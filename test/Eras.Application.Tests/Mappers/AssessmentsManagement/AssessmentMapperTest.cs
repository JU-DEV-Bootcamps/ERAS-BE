using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Mappers.AssessmentsManagement;

public class AssessmentMapperTest
{
    private readonly Mock<IMapper<InterventionPlanDto, InterventionPlan>> _planMapper = new();
    private readonly Mock<IMapper<IndividualInterventionDto, IndividualIntervention>> _individualMapper = new();
    private readonly Mock<IMapper<GroupInterventionDto, GroupIntervention>> _groupMapper = new();

    private AssessmentMapper CreateSut() =>
        new(
            _planMapper.Object,
            _individualMapper.Object,
            _groupMapper.Object);

    [Fact]
    public void Map_ShouldMapAllAssessmentProperties()
    {
        // Arrange
        var planDto = new InterventionPlanDto();
        var plan = new InterventionPlan();

        var individualDto = new IndividualInterventionDto() {
            DateUtc = DateTime.UtcNow,
            StudentIds = [1]
        };
        var individual = new IndividualIntervention() {
            DateUtc = DateTime.UtcNow,
            StudentIds = [1]
        };

        _planMapper
            .Setup(x => x.Map(planDto))
            .Returns(plan);

        _individualMapper
            .Setup(x => x.Map(individualDto))
            .Returns(individual);

        var source = new AssessmentDto
        {
            Id = 42,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = "user-1",
            Service = "Counselling",
            AssignedProfessional = "professional-1",
            StudentIds = new[] { 1, 2 },
            Diagnosis = "Diagnosis",
            Objective = "Objective",
            Comments = "Comments",
            Plan = planDto,
            Status = AssessmentStatus.Remitted,
            Interventions = new InterventionDto[]
            {
                individualDto
            }
        };

        var sut = CreateSut();

        // Act
        var result = sut.Map(source);

        // Assert
        Assert.Equal(source.Id, result.Id);
        Assert.Equal(source.CreatedAtUtc, result.CreatedAtUtc);
        Assert.Equal(source.CreatedBy, result.CreatedBy);
        Assert.Equal(source.Service, result.Service);
        Assert.Equal(source.AssignedProfessional, result.AssignedProfessional);
        Assert.Equal(source.StudentIds, result.StudentIds);
        Assert.Equal(source.Diagnosis, result.Diagnosis);
        Assert.Equal(source.Objective, result.Objective);
        Assert.Equal(source.Comments, result.Comments);
        Assert.Same(plan, result.Plan);
        Assert.Equal(source.Status, result.Status);

        var mappedIntervention = Assert.Single(result.Interventions);
        Assert.Same(individual, mappedIntervention);

        _planMapper.Verify(x => x.Map(planDto), Times.Once);
        _individualMapper.Verify(x => x.Map(individualDto), Times.Once);
    }

    [Fact]
    public void Map_ShouldUseDefaultId_WhenIdIsNull()
    {
        // Arrange
        var source = new AssessmentDto
        {
            Id = null,
            Interventions = Array.Empty<InterventionDto>(),
            CreatedBy = "one",
            StudentIds = [1],
            Service = "call",
            Status = AssessmentStatus.Remitted
        };

        var sut = CreateSut();

        // Act
        var result = sut.Map(source);

        // Assert
        Assert.Equal(default, result.Id);
    }

    [Fact]
    public void Map_ShouldSetPlanToNull_WhenSourcePlanIsNull()
    {
        // Arrange
        var source = new AssessmentDto
        {
            Plan = null,
            Interventions = Array.Empty<InterventionDto>(),
            CreatedBy = "one",
            StudentIds = [1],
            Service = "call",
            Status = AssessmentStatus.Remitted
        };

        var sut = CreateSut();

        // Act
        var result = sut.Map(source);

        // Assert
        Assert.Null(result.Plan);

        _planMapper.Verify(x => x.Map(It.IsAny<InterventionPlanDto>()), Times.Never);
    }

    [Fact]
    public void Map_ShouldReturnEmptyInterventions_WhenSourceHasNoInterventions()
    {
        // Arrange
        var source = new AssessmentDto
        {
            Interventions = Array.Empty<InterventionDto>(),
            CreatedBy = "one",
            StudentIds = [1],
            Service = "call",
            Status = AssessmentStatus.Remitted
        };

        var sut = CreateSut();

        // Act
        var result = sut.Map(source);

        // Assert
        Assert.NotNull(result.Interventions);
        Assert.Empty(result.Interventions);

        _individualMapper.Verify(x => x.Map(It.IsAny<IndividualInterventionDto>()), Times.Never);

        _groupMapper.Verify(x => x.Map(It.IsAny<GroupInterventionDto>()), Times.Never);
    }

    [Fact]
    public void Map_ShouldMapIndividualIntervention_UsingIndividualMapper()
    {
        // Arrange
        var dto = new IndividualInterventionDto() {
            DateUtc = DateTime.UtcNow,
            StudentIds = [1]
        };
        var mapped = new IndividualIntervention() {
            DateUtc = DateTime.UtcNow,
            StudentIds = [1]
        };

        _individualMapper
            .Setup(x => x.Map(dto))
            .Returns(mapped);

        var source = new AssessmentDto
        {
            Interventions = new InterventionDto[] { dto },
            CreatedBy = "one",
            StudentIds = [1],
            Service = "call",
            Status = AssessmentStatus.Remitted
        };

        var sut = CreateSut();

        // Act
        var result = sut.Map(source);

        // Assert
        var intervention = Assert.Single(result.Interventions);
        Assert.Same(mapped, intervention);

        _individualMapper.Verify(x => x.Map(dto), Times.Once);
        _groupMapper.Verify(
            x => x.Map(It.IsAny<GroupInterventionDto>()),
            Times.Never);
    }

    [Fact]
    public void Map_ShouldMapGroupIntervention_UsingGroupMapper()
    {
        // Arrange
        var dto = new GroupInterventionDto() {
            DateUtc = DateTime.UtcNow,
            StudentIds = [1]
        };
        var mapped = new GroupIntervention()
        {
            DateUtc = DateTime.UtcNow,
            StudentIds = [1]
        };

        _groupMapper
            .Setup(x => x.Map(dto))
            .Returns(mapped);

        var source = new AssessmentDto
        {
            Interventions = new InterventionDto[] { dto },
            CreatedBy = "one",
            StudentIds = [1],
            Service = "call",
            Status = AssessmentStatus.Remitted
        };

        var sut = CreateSut();

        // Act
        var result = sut.Map(source);

        // Assert
        var intervention = Assert.Single(result.Interventions);
        Assert.Same(mapped, intervention);

        _groupMapper.Verify(x => x.Map(dto), Times.Once);
        _individualMapper.Verify(
            x => x.Map(It.IsAny<IndividualInterventionDto>()), Times.Never);
    }

    [Fact]
    public void Map_ShouldMapMultipleInterventions_InSourceOrder()
    {
        // Arrange
        var individualDto = new IndividualInterventionDto()
        {
            DateUtc = DateTime.UtcNow,
            StudentIds = [1]
        };
        var groupDto = new GroupInterventionDto()
        {
            DateUtc = DateTime.UtcNow,
            StudentIds = [1]
        };

        var individual = new IndividualIntervention()
        {
            DateUtc = DateTime.UtcNow,
            StudentIds = [1]
        };
        var group = new GroupIntervention()
        {
            DateUtc = DateTime.UtcNow,
            StudentIds = [1]
        };

        _individualMapper
            .Setup(x => x.Map(individualDto))
            .Returns(individual);

        _groupMapper
            .Setup(x => x.Map(groupDto))
            .Returns(group);

        var source = new AssessmentDto
        {
            Interventions = new InterventionDto[]
            {
                individualDto,
                groupDto
            },
            CreatedBy = "one",
            StudentIds = [1],
            Service = "call",
            Status = AssessmentStatus.Remitted
        };

        var sut = CreateSut();

        // Act
        var result = sut.Map(source);

        // Assert
        Assert.Equal(2, result.Interventions.Count);

        Assert.Same(individual, result.Interventions.ElementAt(0));
        Assert.Same(group, result.Interventions.ElementAt(1));

        _individualMapper.Verify(x => x.Map(individualDto), Times.Once);
        _groupMapper.Verify(x => x.Map(groupDto), Times.Once);
    }
}
