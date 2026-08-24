using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Features.RemissionManagement;
using Eras.Application.Features.RemissionManagement.Handlers;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities;
using Eras.Domain.Entities.AssessmentManagement;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Features.Assessments.Queries;
public sealed class GetRemissionByIdQueryHandlerTests
{
    private readonly Mock<IAssessmentRepository> _repository = new();
    private readonly Mock<IMapper<Assessment, AssessmentDto>> _mapper = new();
    private readonly Mock<ILogger<GetRemissionByIdQueryHandler>> _logger = new();
    private readonly Mock<IStudentRepository> _studentRepository = new();

    private GetRemissionByIdQueryHandler CreateHandler()
        => new(
            _repository.Object,
            _mapper.Object,
            _logger.Object,
            _studentRepository.Object);

    [Fact]
    public async Task Handle_WhenAssessmentDoesNotExist_ReturnsNull()
    {
        // Arrange
        var assessmentId = 1;

        _repository
            .Setup(x => x.GetByIdWithInterventionsAsync(assessmentId))
            .ReturnsAsync((Assessment?)null);

        var handler = CreateHandler();
        var request = new GetRemissionByIdQuery(assessmentId);
        
        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Null(result);

        _mapper.Verify(x => x.Map(It.IsAny<Assessment>()),Times.Never);

        _studentRepository.Verify(
            x => x.GetByIdsAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenAssessmentHasNoStudents_ReturnsMappedAssessmentWithEmptyStudents()
    {
        // Arrange
        var assessmentId = 12;
        var assessment = new Assessment
        {
            StudentIds = null!,
            CreatedBy = "me",
            Service = "workshop",
            Status = AssessmentStatus.Finalized,
        };

        var mappedDto = CreateAssessmentDto();

        _repository
            .Setup(x => x.GetByIdWithInterventionsAsync(assessmentId))
            .ReturnsAsync(assessment);

        _mapper
            .Setup(x => x.Map(assessment))
            .Returns(mappedDto);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new GetRemissionByIdQuery(assessmentId), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result!.Students);

        _studentRepository.Verify(
            x => x.GetByIdsAsync(
                It.IsAny<IEnumerable<int>>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _studentRepository.Verify(
            x => x.GetAverageRiskByStudentIdsAsync(
                It.IsAny<IEnumerable<int>>()), Times.Never);

        _mapper.Verify(x => x.Map(assessment), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenStudentsExist_ReturnsStudentProfilesWithAverageRisk()
    {
        // Arrange
        var assessmentId = 11;

        var assessment = new Assessment
        {
            StudentIds = new[] { 1, 2 },
            CreatedBy = "me",
            Service = "workshop",
            Status = AssessmentStatus.Finalized
        };

        var students = new[]
        {
            new Student
            {
                Id = 1,
                Name = "John Doe",
                Email = "john@example.com"
            },
            new Student
            {
                Id = 2,
                Name = "Jane Doe",
                Email = "jane@example.com"
            }
        };

        var mappedDto = CreateAssessmentDto();

        _repository
            .Setup(x => x.GetByIdWithInterventionsAsync(assessmentId))
            .ReturnsAsync(assessment);

        _studentRepository
            .Setup(x => x.GetByIdsAsync(
                It.Is<IEnumerable<int>>(ids => ids.SequenceEqual(new[] { 1, 2 })),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(students);

        _studentRepository
            .Setup(x => x.GetAverageRiskByStudentIdsAsync(
                It.Is<IEnumerable<int>>(ids => ids.SequenceEqual(new[] { 1, 2 }))))
            .ReturnsAsync(new Dictionary<int, double>
            {
                [1] = 2.5,
                [2] = 4.0
            });

        _mapper
            .Setup(x => x.Map(assessment))
            .Returns(mappedDto);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new GetRemissionByIdQuery(assessmentId), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result!.Students.Count);

        var student1 = result.Students.Single(x => x.Id == 1);
        Assert.Equal("John Doe", student1.Name);
        Assert.Equal("john@example.com", student1.Email);
        Assert.Equal(2.5, student1.AvgRiskLevel);

        var student2 = result.Students.Single(x => x.Id == 2);
        Assert.Equal("Jane Doe", student2.Name);
        Assert.Equal("jane@example.com", student2.Email);
        Assert.Equal(4.0, student2.AvgRiskLevel);
    }

    [Fact]
    public async Task Handle_WhenStudentDoesNotExist_UsesFallbackProfile()
    {
        // Arrange
        var assessmentId = 2;

        var assessment = new Assessment
        {
            StudentIds = new[] { 999 },
            CreatedBy = "me",
            Service = "workshop",
            Status = AssessmentStatus.Finalized,
        };

        _repository
            .Setup(x => x.GetByIdWithInterventionsAsync(assessmentId))
            .ReturnsAsync(assessment);

        _studentRepository
            .Setup(x => x.GetByIdsAsync(
                It.IsAny<IEnumerable<int>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Student>());

        _studentRepository
            .Setup(x => x.GetAverageRiskByStudentIdsAsync(
                It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync(new Dictionary<int, double>());

        _mapper
            .Setup(x => x.Map(assessment))
            .Returns(CreateAssessmentDto());

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new GetRemissionByIdQuery(assessmentId), CancellationToken.None);

        // Assert
        var student = Assert.Single(result!.Students);

        Assert.Equal(999, student.Id);
        Assert.Equal("ID 999", student.Name);
        Assert.Equal(string.Empty, student.Email);
        Assert.Equal(0, student.AvgRiskLevel);
    }

    [Fact]
    public async Task Handle_WhenAverageRiskIsMissing_UsesZero()
    {
        // Arrange
        var assessmentId = 12;

        var assessment = new Assessment
        {
            StudentIds = new[] { 1 },
            CreatedBy = "me",
            Service = "workshop",
            Status = AssessmentStatus.Finalized,
        };

        _repository
            .Setup(x => x.GetByIdWithInterventionsAsync(assessmentId))
            .ReturnsAsync(assessment);

        _studentRepository
            .Setup(x => x.GetByIdsAsync(
                It.IsAny<IEnumerable<int>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[]
            {
                new Student
                {
                    Id = 1,
                    Name = "John Doe",
                    Email = "john@example.com"
                }
            });

        _studentRepository
            .Setup(x => x.GetAverageRiskByStudentIdsAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync(new Dictionary<int, double>());

        _mapper
            .Setup(x => x.Map(assessment))
            .Returns(CreateAssessmentDto());

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new GetRemissionByIdQuery(assessmentId), CancellationToken.None);

        // Assert
        var student = Assert.Single(result!.Students);

        Assert.Equal(1, student.Id);
        Assert.Equal("John Doe", student.Name);
        Assert.Equal(0, student.AvgRiskLevel);
    }

    [Fact]
    public async Task Handle_PassesCancellationTokenToStudentRepository()
    {
        // Arrange
        var assessmentId = 12;
        using var cts = new CancellationTokenSource();

        var assessment = new Assessment
        {
            StudentIds = new[] { 1 },
            CreatedBy = "me",
            Service = "workshop",
            Status = AssessmentStatus.Finalized,
        };

        _repository
            .Setup(x => x.GetByIdWithInterventionsAsync(assessmentId))
            .ReturnsAsync(assessment);

        _studentRepository
            .Setup(x => x.GetByIdsAsync(
                It.Is<IEnumerable<int>>(ids => ids.SequenceEqual(new[] { 1 })),
                cts.Token))
            .ReturnsAsync(Array.Empty<Student>());

        _studentRepository
            .Setup(x => x.GetAverageRiskByStudentIdsAsync(
                It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync(new Dictionary<int, double>());

        _mapper
            .Setup(x => x.Map(assessment))
            .Returns(CreateAssessmentDto());

        var handler = CreateHandler();

        // Act
        await handler.Handle(
            new GetRemissionByIdQuery(assessmentId),
            cts.Token);

        // Assert
        _studentRepository.Verify(
            x => x.GetByIdsAsync(
                It.Is<IEnumerable<int>>(ids => ids.SequenceEqual(new[] { 1 })),
                cts.Token),
            Times.Once);
    }

    private static AssessmentDto CreateAssessmentDto()
        => new()
        {
            Students = Array.Empty<StudentProfileDto>(),
            CreatedBy = "me",
            Service = "workshop",
            Status = AssessmentStatus.Remitted,
            StudentIds = [1, 2, 3],
        };
}
