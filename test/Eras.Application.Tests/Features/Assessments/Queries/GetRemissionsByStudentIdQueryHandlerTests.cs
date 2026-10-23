using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Features.RemissionManagement;
using Eras.Application.Features.RemissionManagement.Handlers;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Features.Assessments.Queries;

public class GetRemissionsByStudentIdQueryHandlerTests
{
    private readonly Mock<IAssessmentRepository> _repository = new();
    private readonly Mock<IMapper<Assessment, AssessmentDto>> _mapper = new();

    private GetRemissionsByStudentIdQueryHandler CreateHandler()
        => new(_repository.Object, _mapper.Object);

    [Fact]
    public async Task Handle_WhenAssessmentsExist_ReturnsMappedAssessments()
    {
        // Arrange
        var studentId = 123;

        var assessment1 = new Assessment 
        {
            CreatedBy = "me",
            Service = "workshop",
            Status = AssessmentStatus.Finalized,
            StudentIds = [1,2,123],
        };
        var assessment2 = new Assessment 
        {
            CreatedBy = "me",
            Service = "workshop",
            Status = AssessmentStatus.Finalized,
            StudentIds = [123, 2, 3],
        };

        var dto1 = new AssessmentDto
        {
            CreatedBy = "me",
            Service = "workshop",
            Status = AssessmentStatus.Finalized,
            StudentIds = [1, 2, 123],
        };
        var dto2 = new AssessmentDto 
        {
            CreatedBy = "me",
            Service = "workshop",
            Status = AssessmentStatus.Finalized,
            StudentIds = [123, 2, 3],
        };

        _repository
            .Setup(x => x.GetByStudentIdAsync(studentId))
            .ReturnsAsync(new[] { assessment1, assessment2 });

        _mapper
            .Setup(x => x.Map(assessment1))
            .Returns(dto1);

        _mapper
            .Setup(x => x.Map(assessment2))
            .Returns(dto2);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new GetRemissionsByStudentIdQuery(studentId), CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Same(dto1, result.ElementAt(0));
        Assert.Same(dto2, result.ElementAt(1));

        _repository.Verify(x => x.GetByStudentIdAsync(studentId), Times.Once);
        _mapper.Verify(x => x.Map(assessment1), Times.Once);
        _mapper.Verify(x => x.Map(assessment2), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNoAssessmentsExist_ReturnsEmptyCollection()
    {
        // Arrange
        var studentId = 123;

        _repository
            .Setup(x => x.GetByStudentIdAsync(studentId))
            .ReturnsAsync(Array.Empty<Assessment>());

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new GetRemissionsByStudentIdQuery(studentId), CancellationToken.None);

        // Assert
        Assert.Empty(result);

        _mapper.Verify(x => x.Map(It.IsAny<Assessment>()),Times.Never);
    }
}
