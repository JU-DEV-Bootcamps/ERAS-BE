using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Consolidator.Queries.Students;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.Consolidator.Queries;

public class GetStudentTopQueryHandlerTests
{
    private readonly Mock<ICohortRepository> _cohortRepository = new();
    private readonly Mock<IStudentCohortRepository> _studentCohortRepository = new();
    private readonly Mock<IAnswerRepository> _answerRepository = new();
    private readonly Mock<IPollRepository> _pollRepository = new();
    private readonly Mock<ILogger<GetStudentTopQueryHandler>> _logger = new();

    private GetStudentTopQueryHandler CreateHandler()
        => new(
            _cohortRepository.Object,
            _studentCohortRepository.Object,
            _answerRepository.Object,
            _pollRepository.Object,
            _logger.Object);

    [Fact]
    public async Task Handle_ShouldReturnTopStudents_WhenRequestIsValid()
    {
        // Arrange
        var poll = new Poll();
        var cohort = new Cohort { Id = 1 };

        var student1 = new Student { Uuid = "123" };
        var student2 = new Student { Uuid = "456" };

        var answers1 = new List<Answer>
        {
            new() { RiskLevel = 80 },
            new() { RiskLevel = 60 }
        };

        var answers2 = new List<Answer>
        {
            new() { RiskLevel = 40 }
        };

        _pollRepository
            .Setup(x => x.GetByNameAsync("Poll"))
            .ReturnsAsync(poll);

        _cohortRepository
            .Setup(x => x.GetByNameAsync("Cohort"))
            .ReturnsAsync(cohort);

        _studentCohortRepository
            .Setup(x => x.GetAllStudentsByCohortIdAsync(cohort.Id))
            .ReturnsAsync(new[] { student1, student2 });

        _answerRepository
            .Setup(x => x.GetByStudentIdAsync(student1.Uuid))
            .ReturnsAsync(answers1);

        _answerRepository
            .Setup(x => x.GetByStudentIdAsync(student2.Uuid))
            .ReturnsAsync(answers2);

        var handler = CreateHandler();

        var request = new GetStudentTopQuery
        {
            PollName = "Poll",
            CohortName = "Cohort",
            Take = 1
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Body);
    }

    [Fact]
    public async Task Handle_ShouldUseDefaultTake_WhenTakeIsNull()
    {
        // Arrange
        var poll = new Poll();
        var cohort = new Cohort { Id = 1 };

        var students = Enumerable.Range(1, 6)
            .Select(_ => new Student { Uuid = "123" })
            .ToList();

        _pollRepository
            .Setup(x => x.GetByNameAsync("Poll"))
            .ReturnsAsync(poll);

        _cohortRepository
            .Setup(x => x.GetByNameAsync("Cohort"))
            .ReturnsAsync(cohort);

        _studentCohortRepository
            .Setup(x => x.GetAllStudentsByCohortIdAsync(cohort.Id))
            .ReturnsAsync(students);

        foreach (var student in students)
        {
            _answerRepository
                .Setup(x => x.GetByStudentIdAsync(student.Uuid))
                .ReturnsAsync(new List<Answer>
                {
                    new() { RiskLevel = 50 }
                });
        }

        var handler = CreateHandler();

        var request = new GetStudentTopQuery
        {
            PollName = "Poll",
            CohortName = "Cohort",
            Take = null
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result.Body);
        Assert.Equal(5, result.Body.Count);
    }

    [Fact]
    public async Task Handle_ShouldUseDefaultTake_WhenTakeIsZero()
    {
        // Arrange
        var poll = new Poll();
        var cohort = new Cohort { Id = 1 };
        var students = Enumerable.Range(1, 6)
            .Select(_ => new Student { Uuid = "1" })
            .ToList();

        _pollRepository
            .Setup(x => x.GetByNameAsync("Poll"))
            .ReturnsAsync(poll);

        _cohortRepository
            .Setup(x => x.GetByNameAsync("Cohort"))
            .ReturnsAsync(cohort);

        _studentCohortRepository
            .Setup(x => x.GetAllStudentsByCohortIdAsync(cohort.Id))
            .ReturnsAsync(students);

        foreach (var student in students)
        {
            _answerRepository
                .Setup(x => x.GetByStudentIdAsync(student.Uuid))
                .ReturnsAsync(new List<Answer>
                {
                    new() { RiskLevel = 50 }
                });
        }

        var handler = CreateHandler();

        var request = new GetStudentTopQuery
        {
            PollName = "Poll",
            CohortName = "Cohort",
            Take = 0
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result.Body);
        Assert.Equal(5, result.Body.Count);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenPollDoesNotExist()
    {
        // Arrange
        _pollRepository
            .Setup(x => x.GetByNameAsync("Poll"))
            .ReturnsAsync((Poll?)null);

        var handler = CreateHandler();

        var request = new GetStudentTopQuery
        {
            PollName = "Poll",
            CohortName = "Cohort",
            Take = 5
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Empty(result.Body!);
        Assert.False(result.Success);
        Assert.Equal("Failed to retrieve top risk students. Error: Poll not found", result.Message);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCohortNameIsNull()
    {
        // Arrange
        _pollRepository
            .Setup(x => x.GetByNameAsync("Poll"))
            .ReturnsAsync(new Poll());

        var handler = CreateHandler();

        var request = new GetStudentTopQuery
        {
            PollName = "Poll",
            CohortName = null!,
            Take = 5
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Empty(result.Body!);
        Assert.False(result.Success);
        Assert.Equal("Failed to retrieve top risk students. Error: No students found for the cohort", result.Message);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCohortHasNoStudents()
    {
        // Arrange
        var cohort = new Cohort { Id = 1 };

        _pollRepository
            .Setup(x => x.GetByNameAsync("Poll"))
            .ReturnsAsync(new Poll());

        _cohortRepository
            .Setup(x => x.GetByNameAsync("Cohort"))
            .ReturnsAsync(cohort);

        _studentCohortRepository
            .Setup(x => x.GetAllStudentsByCohortIdAsync(cohort.Id))
            .ReturnsAsync((IEnumerable<Student>?)null);

        var handler = CreateHandler();

        var request = new GetStudentTopQuery
        {
            PollName = "Poll",
            CohortName = "Cohort",
            Take = 5
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Empty(result.Body!);
        Assert.False(result.Success);
        Assert.Equal("Failed to retrieve top risk students. Error: No students found for the cohort", result.Message);
    }

    [Fact]
    public async Task Handle_ShouldSkipStudentsWithNoAnswers()
    {
        // Arrange
        var cohort = new Cohort { Id = 1 };

        var studentWithoutAnswers = new Student { Uuid = "120" };
        var studentWithAnswers = new Student { Uuid = "123" };

        _pollRepository
            .Setup(x => x.GetByNameAsync("Poll"))
            .ReturnsAsync(new Poll());

        _cohortRepository
            .Setup(x => x.GetByNameAsync("Cohort"))
            .ReturnsAsync(cohort);

        _studentCohortRepository
            .Setup(x => x.GetAllStudentsByCohortIdAsync(cohort.Id))
            .ReturnsAsync(new[]
            {
                studentWithoutAnswers,
                studentWithAnswers
            });

        _answerRepository
            .Setup(x => x.GetByStudentIdAsync(studentWithoutAnswers.Uuid))
            .ReturnsAsync(new List<Answer>());

        _answerRepository
            .Setup(x => x.GetByStudentIdAsync(studentWithAnswers.Uuid))
            .ReturnsAsync(new List<Answer>
            {
                new() { RiskLevel = 80 }
            });

        var handler = CreateHandler();

        var request = new GetStudentTopQuery
        {
            PollName = "Poll",
            CohortName = "Cohort",
            Take = 5
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result.Body);
        Assert.True(result.Success);
    }
}

