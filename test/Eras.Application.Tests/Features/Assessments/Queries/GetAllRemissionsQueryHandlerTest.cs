using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Features.RemissionManagement;
using Eras.Application.Features.RemissionManagement.Handlers;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities;
using Eras.Domain.Entities.AssessmentManagement;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Features.Assessments.Queries;

public sealed class GetAllRemissionsQueryHandlerTests
{
    private readonly Mock<IAssessmentRepository> _repository = new();
    private readonly Mock<IMapper<Assessment, AssessmentDto>> _mapper = new();
    private readonly Mock<IStudentRepository> _studentRepository = new();

    private GetAllRemissionsQueryHandler CreateHandler()
        => new(_repository.Object, _mapper.Object, _studentRepository.Object);

    [Fact]
    public async Task Handle_WhenThereAreNoAssessments_ReturnsEmptyCollection()
    {
        // Arrange
        _repository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(Array.Empty<Assessment>());

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new GetAllRemissionsQuery(),
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        _studentRepository.Verify(
            x => x.GetByIdsAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _studentRepository.Verify(
            x => x.GetAverageRiskByStudentIdsAsync(It.IsAny<IEnumerable<int>>()),
            Times.Never);

        _mapper.Verify(
            x => x.Map(It.IsAny<Assessment>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenAssessmentHasEmptyStudentIds_DoesNotQueryStudentRepository()
    {
        // Arrange
        var assessment = new Assessment
        {
            StudentIds = Array.Empty<int>(),
            CreatedBy = "me",
            Service = "workshop",
            Status = AssessmentStatus.Remitted,
        };

        var mappedDto = CreateAssessmentDto();

        _repository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(new[] { assessment });

        _mapper
            .Setup(x => x.Map(assessment))
            .Returns(mappedDto);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new GetAllRemissionsQuery(), CancellationToken.None);

        // Assert
        var dto = Assert.Single(result);

        Assert.Empty(dto.Students);

        _studentRepository.Verify(
            x => x.GetByIdsAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _studentRepository.Verify(
            x => x.GetAverageRiskByStudentIdsAsync(It.IsAny<IEnumerable<int>>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenStudentsExist_MapsStudentInformation()
    {
        // Arrange
        var assessment = new Assessment
        {
            StudentIds = new[] { 1, 2 },
            CreatedBy = "me",
            Service = "workshop",
            Status = AssessmentStatus.Remitted,
        };

        var student1 = new Student
        {
            Id = 1,
            Name = "John Doe",
            Email = "john@example.com"
        };

        var student2 = new Student
        {
            Id = 2,
            Name = "Jane Doe",
            Email = "jane@example.com"
        };

        var mappedDto = CreateAssessmentDto();

        _repository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(new[] { assessment });

        _mapper
            .Setup(x => x.Map(assessment))
            .Returns(mappedDto);

        _studentRepository
            .Setup(x => x.GetByIdsAsync(
                It.Is<IEnumerable<int>>(ids => ids.SequenceEqual(new[] { 1, 2 })),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { student1, student2 });

        _studentRepository
            .Setup(x => x.GetAverageRiskByStudentIdsAsync(
                It.Is<IEnumerable<int>>(ids => ids.SequenceEqual(new[] { 1, 2 }))))
            .ReturnsAsync(new Dictionary<int, double>
            {
                [1] = 2.5,
                [2] = 4.0
            });

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new GetAllRemissionsQuery(), CancellationToken.None);

        // Assert
        var dto = Assert.Single(result);

        Assert.Equal(2, dto.Students.Count);

        var mappedStudent1 = dto.Students.Single(x => x.Id == 1);
        Assert.Equal("John Doe", mappedStudent1.Name);
        Assert.Equal("john@example.com", mappedStudent1.Email);
        Assert.Equal(2.5, mappedStudent1.AvgRiskLevel);

        var mappedStudent2 = dto.Students.Single(x => x.Id == 2);
        Assert.Equal("Jane Doe", mappedStudent2.Name);
        Assert.Equal("jane@example.com", mappedStudent2.Email);
        Assert.Equal(4.0, mappedStudent2.AvgRiskLevel);
    }

    [Fact]
    public async Task Handle_WhenAverageRiskDoesNotExist_UsesZeroRisk()
    {
        // Arrange
        var assessment = new Assessment
        {
            StudentIds = new[] { 1 },
            CreatedBy = "me",
            Service = "workshop",
            Status = AssessmentStatus.Remitted,
        };

        var student = new Student
        {
            Id = 1,
            Name = "John Doe",
            Email = "john@example.com"
        };

        var mappedDto = CreateAssessmentDto();

        _repository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(new[] { assessment });

        _mapper
            .Setup(x => x.Map(assessment))
            .Returns(mappedDto);

        _studentRepository
            .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { student });

        _studentRepository
            .Setup(x => x.GetAverageRiskByStudentIdsAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync(new Dictionary<int, double>());

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new GetAllRemissionsQuery(), CancellationToken.None);

        // Assert
        var dto = Assert.Single(result);
        var studentDto = Assert.Single(dto.Students);

        Assert.Equal(1, studentDto.Id);
        Assert.Equal("John Doe", studentDto.Name);
        Assert.Equal("john@example.com", studentDto.Email);
        Assert.Equal(0, studentDto.AvgRiskLevel);
    }

    [Fact]
    public async Task Handle_WhenStudentCannotBeFound_UsesFallbackStudentProfile()
    {
        // Arrange
        var assessment = new Assessment
        {
            StudentIds = new[] { 999 },
            CreatedBy = "me",
            Service = "workshop",
            Status = AssessmentStatus.Remitted,
        };

        var mappedDto = CreateAssessmentDto();

        _repository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(new[] { assessment });

        _mapper
            .Setup(x => x.Map(assessment))
            .Returns(mappedDto);

        _studentRepository
            .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Student>());

        _studentRepository
            .Setup(x => x.GetAverageRiskByStudentIdsAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync(new Dictionary<int, double>());

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new GetAllRemissionsQuery(), CancellationToken.None);

        // Assert
        var dto = Assert.Single(result);
        var studentDto = Assert.Single(dto.Students);

        Assert.Equal(999, studentDto.Id);
        Assert.Equal("ID 999", studentDto.Name);
        Assert.Equal(string.Empty, studentDto.Email);
        Assert.Equal(0, studentDto.AvgRiskLevel);
    }

    [Fact]
    public async Task Handle_WhenStudentIsMissing_ButRiskExists_StillUsesFallbackProfile()
    {
        // Arrange
        var assessment = new Assessment
        {
            StudentIds = new[] { 999 },
            CreatedBy = "me",
            Service = "workshop",
            Status = AssessmentStatus.Remitted,
        };

        var mappedDto = CreateAssessmentDto();

        _repository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(new[] { assessment });

        _mapper
            .Setup(x => x.Map(assessment))
            .Returns(mappedDto);

        _studentRepository
            .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Student>());

        _studentRepository
            .Setup(x => x.GetAverageRiskByStudentIdsAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync(new Dictionary<int, double>
            {
                [999] = 3.5
            });

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new GetAllRemissionsQuery(), CancellationToken.None);

        // Assert
        var studentDto = Assert.Single(Assert.Single(result).Students);

        Assert.Equal(999, studentDto.Id);
        Assert.Equal("ID 999", studentDto.Name);
        Assert.Equal(string.Empty, studentDto.Email);
        Assert.Equal(0, studentDto.AvgRiskLevel);
    }

    [Fact]
    public async Task Handle_WhenStudentIdsContainDuplicates_QueriesEachStudentOnlyOnce()
    {
        // Arrange
        var assessment = new Assessment
        {
            StudentIds = new[] { 1, 1, 2, 2, 1 },
            CreatedBy = "me",
            Service = "workshop",
            Status = AssessmentStatus.Remitted,
        };

        var student1 = new Student
        {
            Id = 1,
            Name = "John",
            Email = "john@example.com"
        };

        var student2 = new Student
        {
            Id = 2,
            Name = "Jane",
            Email = "jane@example.com"
        };

        _repository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(new[] { assessment });

        _mapper
            .Setup(x => x.Map(assessment))
            .Returns(CreateAssessmentDto());

        _studentRepository
            .Setup(x => x.GetByIdsAsync(
                It.Is<IEnumerable<int>>(ids => ids.SequenceEqual(new[] { 1, 2 })),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { student1, student2 });

        _studentRepository
            .Setup(x => x.GetAverageRiskByStudentIdsAsync(
                It.Is<IEnumerable<int>>(ids => ids.SequenceEqual(new[] { 1, 2 }))))
            .ReturnsAsync(new Dictionary<int, double>
            {
                [1] = 1.5,
                [2] = 2.5
            });

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new GetAllRemissionsQuery(), CancellationToken.None);

        // Assert
        _studentRepository.Verify(
            x => x.GetByIdsAsync(
                It.Is<IEnumerable<int>>(ids => ids.SequenceEqual(new[] { 1, 2 })),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _studentRepository.Verify(
            x => x.GetAverageRiskByStudentIdsAsync(
                It.Is<IEnumerable<int>>(ids => ids.SequenceEqual(new[] { 1, 2 }))),
            Times.Once);

        var students = Assert.Single(result).Students;

        Assert.Equal(5, students.Count);
    }

    [Fact]
    public async Task Handle_WhenMultipleAssessmentsExist_MapsAllAssessments()
    {
        // Arrange
        var assessment1 = new Assessment
        {
            StudentIds = new[] { 1 },
            CreatedBy = "me",
            Service = "workshop",
            Status = AssessmentStatus.Remitted,
        };

        var assessment2 = new Assessment
        {
            StudentIds = new[] { 2 },
            CreatedBy = "me",
            Service = "workshop",
            Status = AssessmentStatus.Remitted,
        };

        var dto1 = CreateAssessmentDto();
        var dto2 = CreateAssessmentDto();

        _repository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(new[]
            {
                assessment1,
                assessment2
            });

        _mapper
            .Setup(x => x.Map(assessment1))
            .Returns(dto1);

        _mapper
            .Setup(x => x.Map(assessment2))
            .Returns(dto2);

        _studentRepository
            .Setup(x => x.GetByIdsAsync(
                It.IsAny<IEnumerable<int>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[]
            {
                new Student
                {
                    Id = 1,
                    Name = "John",
                    Email = "john@example.com"
                },
                new Student
                {
                    Id = 2,
                    Name = "Jane",
                    Email = "jane@example.com"
                }
            });

        _studentRepository
            .Setup(x => x.GetAverageRiskByStudentIdsAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync(new Dictionary<int, double>
            {
                [1] = 1,
                [2] = 2
            });

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new GetAllRemissionsQuery(), CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);

        _mapper.Verify(x => x.Map(assessment1), Times.Once);
        _mapper.Verify(x => x.Map(assessment2), Times.Once);
    }

    [Fact]
    public async Task Handle_PassesCancellationTokenToStudentRepository()
    {
        // Arrange
        var assessment = new Assessment
        {
            StudentIds = new[] { 1 },
            CreatedBy = "me",
            Service = "workshop",
            Status = AssessmentStatus.Remitted,
        };

        var cancellationToken = new CancellationTokenSource().Token;

        _repository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(new[] { assessment });

        _mapper
            .Setup(x => x.Map(assessment))
            .Returns(CreateAssessmentDto());

        _studentRepository
            .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<int>>(), cancellationToken))
            .ReturnsAsync(new[]
            {
                new Student
                {
                    Id = 1,
                    Name = "John",
                    Email = "john@example.com"
                }
            });

        _studentRepository
            .Setup(x => x.GetAverageRiskByStudentIdsAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync(new Dictionary<int, double>
            {
                [1] = 2
            });

        var handler = CreateHandler();

        // Act
        await handler.Handle(new GetAllRemissionsQuery(), cancellationToken);

        // Assert
        _studentRepository.Verify(
            x => x.GetByIdsAsync(
                It.Is<IEnumerable<int>>(ids => ids.SequenceEqual(new[] { 1 })),
                cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrows_PropagatesException()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Database unavailable.");

        _repository
            .Setup(x => x.GetAllAsync())
            .ThrowsAsync(expectedException);

        var handler = CreateHandler();

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(new GetAllRemissionsQuery(), CancellationToken.None));

        // Assert
        Assert.Same(expectedException, exception);

        _mapper.Verify(
            x => x.Map(It.IsAny<Assessment>()),
            Times.Never);

        _studentRepository.Verify(
            x => x.GetByIdsAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenStudentRepositoryThrows_PropagatesException()
    {
        // Arrange
        var assessment = new Assessment
        {
            StudentIds = new[] { 1 },
            CreatedBy = "me",
            Service = "workshop",
            Status = AssessmentStatus.Remitted,
        };

        var expectedException = new InvalidOperationException("Student database unavailable.");

        _repository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(new[] { assessment });

        _studentRepository
            .Setup(x => x.GetByIdsAsync(
                It.IsAny<IEnumerable<int>>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        var handler = CreateHandler();

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(new GetAllRemissionsQuery(), CancellationToken.None));

        // Assert
        Assert.Same(expectedException, exception);

        _studentRepository.Verify(
            x => x.GetAverageRiskByStudentIdsAsync(It.IsAny<IEnumerable<int>>()),
            Times.Never);

        _mapper.Verify(
            x => x.Map(It.IsAny<Assessment>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenRiskRepositoryThrows_PropagatesException()
    {
        // Arrange
        var assessment = new Assessment
        {
            StudentIds = new[] { 1 },
            CreatedBy = "me",
            Service = "workshop",
            Status = AssessmentStatus.Remitted,
        };

        var expectedException = new InvalidOperationException("Risk calculation unavailable.");

        _repository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(new[] { assessment });

        _studentRepository
            .Setup(x => x.GetByIdsAsync(
                It.IsAny<IEnumerable<int>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[]
            {
                new Student
                {
                    Id = 1,
                    Name = "John",
                    Email = "john@example.com"
                }
            });

        _studentRepository
            .Setup(x => x.GetAverageRiskByStudentIdsAsync(It.IsAny<IEnumerable<int>>()))
            .ThrowsAsync(expectedException);

        var handler = CreateHandler();

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(new GetAllRemissionsQuery(), CancellationToken.None));

        // Assert
        Assert.Same(expectedException, exception);

        _mapper.Verify(
            x => x.Map(It.IsAny<Assessment>()),Times.Never);
    }

    private static AssessmentDto CreateAssessmentDto()
    {
        return new AssessmentDto
        {
            Students = Array.Empty<StudentProfileDto>(),
            CreatedBy = "me",
            Service = "workshop",
            Status = AssessmentStatus.Remitted,
            StudentIds = Array.Empty<int>(),
        };
    }
}
