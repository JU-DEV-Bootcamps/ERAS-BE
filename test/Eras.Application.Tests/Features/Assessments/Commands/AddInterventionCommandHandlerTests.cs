
using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Features.RemissionManagement;
using Eras.Application.Features.RemissionManagement.Handlers;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

using Moq;

namespace Eras.Application.Tests.Features.Assessments.Commands;

internal sealed class TestIntervention : Intervention
{
    public override InterventionKind Kind => InterventionKind.Individual;
}

public class AddInterventionCommandHandlerTests
{
    private readonly Mock<IAssessmentRepository> _mockRepository;
    private readonly Mock<IMapper<IndividualInterventionDto, IndividualIntervention>> _mockIndividualMapper;
    private readonly Mock<IMapper<GroupInterventionDto, GroupIntervention>> _mockGroupMapper;
    private readonly AddInterventionCommandHandler _handler;

    public AddInterventionCommandHandlerTests()
    {
        _mockRepository = new Mock<IAssessmentRepository>();
        _mockIndividualMapper = new Mock<IMapper<IndividualInterventionDto, IndividualIntervention>>();
        _mockGroupMapper = new Mock<IMapper<GroupInterventionDto, GroupIntervention>>();
        _handler = new AddInterventionCommandHandler(
            _mockRepository.Object,
            _mockIndividualMapper.Object,
            _mockGroupMapper.Object);
    }

    [Fact]
    public async Task HandleAddIndividualIntervention()
    {
        // Arrange
        var dto = new IndividualInterventionDto
        {
            DateUtc = DateTime.UtcNow,
            Status = InterventionStatus.Remitted,
            StudentIds = [1],
            Activity = "Interview"
        };

        var command = new AddInterventionCommand(1, dto);

        var assessment = new Assessment
        {
            Id = 1,
            CreatedBy = "",
            Service = "",
            Status = AssessmentStatus.Remitted,
            StudentIds = [1, 2]
        };

        var mappedIntervention = new IndividualIntervention
        {
            DateUtc = dto.DateUtc,
            Status = dto.Status,
            StudentIds = dto.StudentIds,
            Activity = dto.Activity
        };

        var persisted = new IndividualIntervention
        {
            Id = 10,
            DateUtc = dto.DateUtc,
            Status = dto.Status,
            StudentIds = dto.StudentIds,
            Activity = dto.Activity
        };

        _mockRepository
            .Setup(r => r.GetByIdWithInterventionsAsync(1))
            .ReturnsAsync(assessment);

        _mockIndividualMapper
            .Setup(m => m.Map(dto))
            .Returns(mappedIntervention);

        _mockRepository
            .Setup(r => r.AddInterventionAsync(1, mappedIntervention))
            .ReturnsAsync(persisted);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<IndividualInterventionDto>(result);

        Assert.Equal(10, result.Id);
        Assert.Equal(dto.Activity, result.Activity);
        Assert.Equal(dto.Status, result.Status);

        _mockRepository.Verify(r => r.GetByIdWithInterventionsAsync(1), Times.Once);
        _mockIndividualMapper.Verify(m => m.Map(dto), Times.Once);
        _mockRepository.Verify(r => r.AddInterventionAsync(1, mappedIntervention), Times.Once);

        _mockGroupMapper.Verify(m => m.Map(It.IsAny<GroupInterventionDto>()), Times.Never);
    }

    [Fact]
    public async Task HandleAddGroupIntervention()
    {
        // Arrange
        var dto = new GroupInterventionDto
        {
            DateUtc = DateTime.UtcNow,
            Status = InterventionStatus.Remitted,
            StudentIds = [1, 2],
            Activity = "Interview"
        };

        var command = new AddInterventionCommand(1, dto);

        var assessment = new Assessment {
            Id = 1,
            CreatedBy = "",
            Service = "",
            Status = AssessmentStatus.Remitted,
            StudentIds = [1, 2]
        };

        var mappedIntervention = new GroupIntervention
        {
            DateUtc = dto.DateUtc,
            Status = dto.Status,
            StudentIds = dto.StudentIds,
            Activity = dto.Activity
        };

        var persisted = new GroupIntervention
        {
            Id = 11,
            DateUtc = dto.DateUtc,
            Status = dto.Status,
            StudentIds = dto.StudentIds,
            Activity = dto.Activity
        };

        _mockRepository
            .Setup(r => r.GetByIdWithInterventionsAsync(1))
            .ReturnsAsync(assessment);

        _mockGroupMapper
            .Setup(m => m.Map(dto))
            .Returns(mappedIntervention);

        _mockRepository
            .Setup(r => r.AddInterventionAsync(1, mappedIntervention))
            .ReturnsAsync(persisted);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<GroupInterventionDto>(result);

        Assert.Equal(11, result.Id);
        Assert.Equal(dto.Activity, result.Activity);
        Assert.Equal(dto.Status, result.Status);

        _mockRepository.Verify(r => r.GetByIdWithInterventionsAsync(1), Times.Once);
        _mockGroupMapper.Verify(m => m.Map(dto), Times.Once);
        _mockRepository.Verify(r => r.AddInterventionAsync(1, mappedIntervention), Times.Once);

        _mockIndividualMapper.Verify(m => m.Map(It.IsAny<IndividualInterventionDto>()), Times.Never);
    }
}
