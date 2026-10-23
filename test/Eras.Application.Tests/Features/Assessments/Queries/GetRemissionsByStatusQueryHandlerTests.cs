
using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Features.RemissionManagement;
using Eras.Application.Features.RemissionManagement.Handlers;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

using Moq;

namespace Eras.Application.Tests.Features.Assessments.Queries;

public class GetRemissionsByStatusQueryHandlerTests
{
    private readonly Mock<IAssessmentRepository> _mockRepository;
    private readonly Mock<IMapper<Assessment, AssessmentDto>> _mockMapper;
    private readonly GetRemissionsByStatusQueryHandler _handler;

    public GetRemissionsByStatusQueryHandlerTests()
    {
        _mockRepository = new Mock<IAssessmentRepository>();
        _mockMapper = new Mock<IMapper<Assessment, AssessmentDto>>();

        _handler = new GetRemissionsByStatusQueryHandler(
            _mockRepository.Object,
            _mockMapper.Object);
    }

    [Fact]
    public async Task HandleGetMappedRemissionsAsync()
    {
        // Arrange
        var query = new GetRemissionsByStatusQuery(AssessmentStatus.Remitted);

        var entities = new[]
        {
            new Assessment
            {
                Id = 1,
                Status = AssessmentStatus.Remitted,
                StudentIds = [1],
                CreatedBy = "User1",
                Service = "Service1"
            },
             new Assessment
            {
                Id = 2,
                Status = AssessmentStatus.Remitted,
                StudentIds = [1],
                CreatedBy = "User2",
                Service = "Service2"
            }
        };
        var dto1 = new AssessmentDto
        {
            Id = 1,
            Status = AssessmentStatus.Remitted,
            StudentIds = [1],
            CreatedBy = "User2",
            Service = "Service2"
        };
        var dto2 = new AssessmentDto
        {
            Id = 2,
            Status = AssessmentStatus.Remitted,
            StudentIds = [1],
            CreatedBy = "User2",
            Service = "Service2"
        };

        _mockRepository
            .Setup(r => r.GetByStatusAsync(AssessmentStatus.Remitted))
            .ReturnsAsync(entities);

        _mockMapper
            .Setup(m => m.Map(entities[0]))
            .Returns(dto1);

        _mockMapper
            .Setup(m => m.Map(entities[1]))
            .Returns(dto2);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        Assert.Contains(result, r => r.Id == dto1.Id);
        Assert.Contains(result, r => r.Id == dto2.Id);

        _mockRepository.Verify(
            r => r.GetByStatusAsync(AssessmentStatus.Remitted),
            Times.Once);

        _mockMapper.Verify(m => m.Map(entities[0]), Times.Once);
        _mockMapper.Verify(m => m.Map(entities[1]), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyCollection_WhenRepositoryReturnsNoResults()
    {
        // Arrange
        var query = new GetRemissionsByStatusQuery(AssessmentStatus.Remitted);

        _mockRepository
            .Setup(r => r.GetByStatusAsync(AssessmentStatus.Remitted))
            .ReturnsAsync(Array.Empty<Assessment>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _mockRepository.Verify(
            r => r.GetByStatusAsync(AssessmentStatus.Remitted),
            Times.Once);

        _mockMapper.Verify(
            m => m.Map(It.IsAny<Assessment>()),
            Times.Never);
    }
}
