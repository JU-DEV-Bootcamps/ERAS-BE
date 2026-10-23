using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Features.RemissionManagement;
using Eras.Application.Features.RemissionManagement.Handlers;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement.StatusManagement;

using FluentValidation;
using FluentValidation.Results;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Features.Assessments.Commands;

public sealed class UpsertInterventionsCommandHandlerTests
{
    private readonly Mock<IAssessmentRepository> _repository = new();
    private readonly Mock<IMapper<IndividualInterventionDto, IndividualIntervention>> _individualMapper = new();
    private readonly Mock<IMapper<GroupInterventionDto, GroupIntervention>> _groupMapper = new();
    private readonly Mock<IMapper<Assessment, AssessmentDto>> _toDtoMapper = new();
    private readonly Mock<IValidator<StatusTransitionRequest<InterventionStatus>>> _statusValidator = new();

    private UpsertInterventionsCommandHandler CreateHandler()
        => new(
            _repository.Object,
            _individualMapper.Object,
            _groupMapper.Object,
            _toDtoMapper.Object,
            _statusValidator.Object);

    [Fact]
    public async Task Handle_WhenAssessmentDoesNotExist_ThrowsKeyNotFoundException()
    {
        // Arrange
        var assessmentId = 1;

        _repository
            .Setup(x => x.GetByIdWithInterventionsAsync(assessmentId))
            .ReturnsAsync((Assessment?)null);

        var handler = CreateHandler();

        var command = new UpsertInterventionsCommand(assessmentId, Array.Empty<InterventionDto>());

        // Act
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));

        // Assert
        Assert.Equal(
            $"Assessment '{assessmentId}' not found.", exception.Message);

        _repository.Verify(
            x => x.ReplaceInterventionsAsync(
                It.IsAny<int>(),
                It.IsAny<IReadOnlyCollection<Intervention>>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenIndividualAndGroupInterventionsAreProvided_MapsAndReplacesThem()
    {
        // Arrange
        var assessmentId = 10;

        var individualDto = new IndividualInterventionDto
        {
            DateUtc = DateTime.UtcNow,
            StudentIds = [10],
            Id = 1,
            Status = InterventionStatus.Remitted
        };

        var groupDto = new GroupInterventionDto
        {
            DateUtc = DateTime.UtcNow,
            StudentIds = [10, 20],
            Id = 2,
            Status = InterventionStatus.Remitted
        };

        var individual = new IndividualIntervention
        {
            DateUtc = DateTime.UtcNow,
            StudentIds = [10],
            Id = individualDto.Id!.Value,
            Status = individualDto.Status
        };

        var group = new GroupIntervention
        {
            DateUtc = DateTime.UtcNow,
            StudentIds = [10, 20],
            Id = groupDto.Id!.Value,
            Status = groupDto.Status
        };

        var assessment = new Assessment
        {
            Interventions = Array.Empty<Intervention>(),
            CreatedBy = "me",
            Service = "workshop",
            Status = AssessmentStatus.Finalized,
            StudentIds = [1, 2, 3],
        };

        var interventions = new InterventionDto[]
        {
            individualDto,
            groupDto
        };

        var resultInterventions = new List<Intervention>
        {
            individual, group
        };

        _repository
            .Setup(x => x.GetByIdWithInterventionsAsync(assessmentId))
            .ReturnsAsync(assessment);

        _individualMapper
            .Setup(x => x.Map(individualDto))
            .Returns(individual);

        _groupMapper
            .Setup(x => x.Map(groupDto))
            .Returns(group);

        _repository
            .Setup(x => x.ReplaceInterventionsAsync(
                assessmentId,
                It.IsAny<IReadOnlyCollection<Intervention>>()))
            .ReturnsAsync(resultInterventions);

        var handler = CreateHandler();

        var command = new UpsertInterventionsCommand(assessmentId, interventions);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Same(interventions, result);

        _individualMapper.Verify(x => x.Map(individualDto), Times.Once);

        _groupMapper.Verify(x => x.Map(groupDto), Times.Once);

        _repository.Verify(
            x => x.ReplaceInterventionsAsync(
                assessmentId,
                It.Is<IReadOnlyCollection<Intervention>>(items =>
                    items.Count == 2 && items.Contains(individual) && items.Contains(group))),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenIncomingInterventionDoesNotMatchExistingId_DoesNotValidateStatus()
    {
        // Arrange
        var assessmentId = 1;

        var existing = new IndividualIntervention
        {
            DateUtc = DateTime.UtcNow,
            StudentIds = [10],
            Id = 2,
            Status = InterventionStatus.Remitted
        };

        var incoming = new IndividualInterventionDto
        {
            DateUtc = DateTime.UtcNow,
            StudentIds = [10],
            Id = 3,
            Status = InterventionStatus.Remitted
        };

        var mapped = new IndividualIntervention
        {
            DateUtc = incoming.DateUtc,
            StudentIds = incoming.StudentIds,
            Id = incoming.Id!.Value,
            Status = incoming.Status
        };

        _repository
            .Setup(x => x.GetByIdWithInterventionsAsync(assessmentId))
            .ReturnsAsync(new Assessment
            {
                Interventions = new Intervention[] { existing },
                CreatedBy = "me",
                Service = "workshop",
                Status = AssessmentStatus.Finalized,
                StudentIds = [1, 2, 3],
            });

        _individualMapper
            .Setup(x => x.Map(incoming))
            .Returns(mapped);

        var command = new UpsertInterventionsCommand(assessmentId, new InterventionDto[] { incoming });

        var handler = CreateHandler();

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _statusValidator.Verify(
            x => x.ValidateAsync(
                It.IsAny<ValidationContext<StatusTransitionRequest<InterventionStatus>>>(),
                It.IsAny<CancellationToken>()), Times.Never);

        _repository.Verify(
            x => x.ReplaceInterventionsAsync(
                assessmentId, It.IsAny<IReadOnlyCollection<Intervention>>()),
            Times.Once);
    }

}
