using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Features.RemissionManagement;
using Eras.Application.Features.RemissionManagement.Handlers;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Features.Assessments.Queries;

public record BuildIntervention : InterventionDto { }

public class GetInterventionsByAssessmentQueryHandlerTests
{
    private readonly Mock<IAssessmentRepository> _repository;
    private readonly Mock<IMapper<Assessment, AssessmentDto>> _mapper;
    private readonly GetInterventionsByAssessmentQueryHandler _handler;

    public GetInterventionsByAssessmentQueryHandlerTests()
    {
        _repository = new Mock<IAssessmentRepository>();
        _mapper = new Mock<IMapper<Assessment, AssessmentDto>>();
        _handler = new GetInterventionsByAssessmentQueryHandler(_repository.Object, _mapper.Object);
    }

    [Fact]
    public async Task Handle_WhenAssessmentExists_ReturnsInterventionsOrderedByDate()
    {
        // Arrange
        var assessmentId = 2;

        var older = new BuildIntervention
        {
            DateUtc = new DateTime(2026, 1, 1),
            StudentIds = [2]
        };

        var newer = new BuildIntervention
        {
            DateUtc = new DateTime(2026, 2, 1),
            StudentIds = [3]
        };

        var assessment = new Assessment
        {
            CreatedBy = "me",
            Service = "workshop",
            Status = AssessmentStatus.Finalized,
            StudentIds = [3],
        };

        var assessmentDto = new AssessmentDto
        {
            CreatedBy = "me",
            Service = "workshop",
            Status = AssessmentStatus.Finalized,
            StudentIds = [3],
            Interventions = new[]
            {
                newer,
                older
            }
        };

        _repository
            .Setup(x => x.GetByIdWithInterventionsAsync(assessmentId))
            .ReturnsAsync(assessment);

        _mapper
            .Setup(x => x.Map(assessment))
            .Returns(assessmentDto);

        // Act
        var result = await _handler.Handle(new GetInterventionsByAssessmentQuery(assessmentId), CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Same(older, result.ElementAt(0));
        Assert.Same(newer, result.ElementAt(1));

        _repository.Verify(x => x.GetByIdWithInterventionsAsync(assessmentId), Times.Once);
        _mapper.Verify(x => x.Map(assessment), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenAssessmentDoesNotExist_ThrowsKeyNotFoundException()
    {
        // Arrange
        var assessmentId = 1;

        _repository
            .Setup(x => x.GetByIdWithInterventionsAsync(assessmentId))
            .ReturnsAsync((Assessment?)null);

        // Act
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(
                new GetInterventionsByAssessmentQuery(assessmentId), CancellationToken.None));

        // Assert
        Assert.Equal(
            $"Assessment '{assessmentId}' not found.",
            exception.Message);

        _mapper.Verify(x => x.Map(It.IsAny<Assessment>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenAssessmentHasNoInterventions_ReturnsEmptyCollection()
    {
        // Arrange
        var assessmentId = 2;
        var assessment = new Assessment 
        {
            CreatedBy = "me",
            Service = "workshop",
            Status = AssessmentStatus.Finalized,
            StudentIds = [3],
        };

        var assessmentDto = new AssessmentDto
        {
            Interventions = Array.Empty<InterventionDto>(),
            CreatedBy = "me",
            Service = "workshop",
            Status = AssessmentStatus.Finalized,
            StudentIds = [3],
        };

        _repository
            .Setup(x => x.GetByIdWithInterventionsAsync(assessmentId))
            .ReturnsAsync(assessment);

        _mapper
            .Setup(x => x.Map(assessment))
            .Returns(assessmentDto);

        // Act
        var result = await _handler.Handle(new GetInterventionsByAssessmentQuery(assessmentId), CancellationToken.None);

        // Assert
        Assert.Empty(result);

        _mapper.Verify(x => x.Map(assessment), Times.Once);
    }
}
