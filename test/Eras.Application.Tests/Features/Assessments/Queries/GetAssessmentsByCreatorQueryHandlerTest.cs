
using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Features.RemissionManagement;
using Eras.Application.Features.RemissionManagement.Handlers.QueryHandlers;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

using Moq;

namespace Eras.Application.Tests.Features.Assessments.Queries;

public class GetAssessmentsByCreatorQueryHandlerTests
{
    private readonly Mock<IAssessmentRepository> _mockRepository;
    private readonly Mock<IMapper<Assessment, AssessmentDto>> _mockMapper;
    private readonly GetAssessmentsByCreatorQueryHandler _handler;

    public GetAssessmentsByCreatorQueryHandlerTests()
    {
        _mockRepository = new Mock<IAssessmentRepository>();
        _mockMapper = new Mock<IMapper<Assessment, AssessmentDto>>();

        _handler = new GetAssessmentsByCreatorQueryHandler(
            _mockRepository.Object,
            _mockMapper.Object);
    }

    [Fact]
    public async Task Handler_ShouldGetAssessmentsByCreatorAsync()
    {
        var query = new GetAssessmentsByCreatorQuery("2e9h0ag2-e3e7-4069-8e14-764d7e7b7526");

        Assessment[] entities =
        [
            new Assessment
            {
                Id = 1,
                Status = AssessmentStatus.Remitted,
                StudentIds = [1],
                CreatedBy = "2e9h0ag2-e3e7-4069-8e14-764d7e7b7526",
                Service = "Service1",
                AssignedProfessional = "d8398ece-a6d0-4474-9607-dc5854115229"
            },
             new Assessment
            {
                Id = 2,
                Status = AssessmentStatus.Remitted,
                StudentIds = [1],
                CreatedBy = "2e9h0ag2-e3e7-4069-8e14-764d7e7b7526",
                Service = "Service2",
                AssignedProfessional = "d8398ece-a6d0-4474-9607-dc5854115229"
            }
        ];
        var dto1 = new AssessmentDto
        {
            Id = 1,
            Status = AssessmentStatus.Remitted,
            StudentIds = [1],
            CreatedBy = "2e9h0ag2-e3e7-4069-8e14-764d7e7b7526",
            Service = "Service1",
            AssignedProfessional = "d8398ece-a6d0-4474-9607-dc5854115229"
        };
        var dto2 = new AssessmentDto
        {
            Id = 2,
            Status = AssessmentStatus.Remitted,
            StudentIds = [1],
            CreatedBy = "2e9h0ag2-e3e7-4069-8e14-764d7e7b7526",
            Service = "Service2",
            AssignedProfessional = "d8398ece-a6d0-4474-9607-dc5854115229"
        };

        _mockRepository
            .Setup(r => r.GetByCreatorAsync(It.IsAny<string>()))
            .ReturnsAsync(entities);

        _mockMapper
            .Setup(m => m.Map(entities[0]))
            .Returns(dto1);

        _mockMapper
            .Setup(m => m.Map(entities[1]))
            .Returns(dto2);

        IEnumerable<AssessmentDto> result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());

        Assert.Contains(result, r => r.Id == dto1.Id);
        Assert.Contains(result, r => r.Id == dto2.Id);

        _mockRepository.Verify(
            r => r.GetByCreatorAsync(It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task Handler_ShouldReturnEmptyCollection_WhenNoAssessmentsFound()
    {
        var query = new GetAssessmentsByCreatorQuery("2e9h0ag2-e3e7-4069-8e14-764d7e7b7526");

        _mockRepository
            .Setup(r => r.GetByCreatorAsync(It.IsAny<string>()))
            .ReturnsAsync([]);

        IEnumerable<AssessmentDto> result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
        _mockRepository.Verify(
            r => r.GetByCreatorAsync(It.IsAny<string>()),
            Times.Once);

        _mockMapper.Verify(
            m => m.Map(It.IsAny<Assessment>()),
            Times.Never);
    }
}
