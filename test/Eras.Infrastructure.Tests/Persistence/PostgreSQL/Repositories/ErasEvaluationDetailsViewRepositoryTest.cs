using Eras.Application.Contracts.Persistence;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;

using Microsoft.EntityFrameworkCore;

using MockQueryable.Moq;

using Moq;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories;

public class ErasEvaluationDetailsViewRepositoryTest
{
    private Mock<AppDbContext> _mockContext;
    private IErasEvaluationDetailsViewRepository? _repository;

    public ErasEvaluationDetailsViewRepositoryTest()
    {
        _mockContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
    }

    [Fact]
    public async Task GetRecentAlertsStudentAsync_Should_Return_First_PageAsync()
    {
        // Arrange
        var data = new List<ErasEvaluationDetailsViewEntity>
            {
                new()
                {
                    StudentId = 1,
                    StudentName = "Student 1",
                    EvaluationName= "Test",
                    Status = "Completed",
                    StudentEmail = "test@m",
                    AnswerText = "Test",
                    ComponentName = "Academic",
                    PollName = "A",
                    PollUuid = "A",
                    VariableName = "A",
                    RiskLevel = 3
                },
                new()
                {
                    StudentId = 2,
                    StudentName = "Student 2",
                    EvaluationName= "Test 2",
                    Status = "InProgress",
                    StudentEmail = "test@m",
                    AnswerText = "Test",
                    ComponentName = "Academic",
                    PollName = "A",
                    PollUuid = "A",
                    VariableName = "A",
                    RiskLevel = 1
                },
                new()
                {
                    StudentId = 3,
                    StudentName = "Student 3",
                    EvaluationName= "Test 3",
                    Status = "Completed",
                    StudentEmail = "test@m",
                    AnswerText = "Test",
                    ComponentName = "Academic",
                    PollName = "A",
                    PollUuid = "A",
                    VariableName = "A",
                    RiskLevel = 1,
                }
            }.AsQueryable().BuildMockDbSet();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _mockContext = new Mock<AppDbContext>(options);

        _mockContext
            .Setup(C => C.ErasEvaluationDetailsView)
            .Returns(data.Object);

        _repository = new ErasEvaluationDetailsViewRepository(_mockContext.Object);

        // Act
        var result = (await _repository.GetRecentAlertsStudentAsync(1, 2)).ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("1", result[0].StudentId);
        Assert.Equal("2", result[1].StudentId);
    }
    [Fact]
    public async Task GetRecentAlertsStudentAsync_Should_Return_Second_PageAsync()
    {
        // Arrange
        var data = new List<ErasEvaluationDetailsViewEntity>
            {
                 new()
                {
                    StudentId = 1,
                    StudentName = "Student 1",
                    EvaluationName= "Test",
                    Status = "Completed",
                    StudentEmail = "test@m",
                    AnswerText = "Test",
                    ComponentName = "Academic",
                    PollName = "A",
                    PollUuid = "A",
                    VariableName = "A",
                    RiskLevel=1
                },
                new()
                {
                    StudentId = 2,
                    StudentName = "Student 2",
                    EvaluationName= "Test 2",
                    Status = "Completed",
                    StudentEmail = "test@m",
                    AnswerText = "Test",
                    ComponentName = "Academic",
                    PollName = "A",
                    PollUuid = "A",
                    VariableName = "A",
                    RiskLevel = 2
                },
                new()
                {
                    StudentId = 3,
                    StudentName = "Student 3",
                    EvaluationName= "Test 3",
                    Status = "Completed",
                    StudentEmail = "test@m",
                    AnswerText = "Test",
                    ComponentName = "Academic",
                    PollName = "A",
                    PollUuid = "A",
                    VariableName = "A",
                    RiskLevel=2
                },
                new() {
                    StudentId = 4,
                    StudentName = "Student 4",
                    EvaluationName= "Test 4",
                    Status = "Completed",
                    StudentEmail = "test@m",
                    AnswerText = "Test",
                    ComponentName = "Academic",
                    PollName = "A",
                    PollUuid = "A",
                    VariableName = "A",
                    RiskLevel=3
                }
            }.AsQueryable().BuildMockDbSet();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _mockContext = new Mock<AppDbContext>(options);

        _mockContext
            .Setup(C => C.ErasEvaluationDetailsView)
            .Returns(data.Object);

        _repository = new ErasEvaluationDetailsViewRepository(_mockContext.Object);

        // Act
        var result = (await _repository.GetRecentAlertsStudentAsync(2, 2)).ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("2", result[0].StudentId);
        Assert.Equal("3", result[1].StudentId);
    }

    [Fact]
    public async Task GetRecentAlertsStudentAsync_Should_ReturnEmpty_EvaluationsAreNotCompleted_InProgresssAsync()
    {
        // Arrange
        var data = new List<ErasEvaluationDetailsViewEntity>
            {
                 new()
                 {
                     StudentId = 1,
                     StudentName = "Student 1",
                     EvaluationName= "Test",
                     Status = "Pending",
                     StudentEmail = "test@m",
                     AnswerText = "Test",
                     ComponentName = "Academic",
                     PollName = "A",
                     PollUuid = "A",
                     VariableName = "A",
                 },
                 new()
                 {
                     StudentId = 2,
                     StudentName = "Student 2",
                     EvaluationName= "Test 2",
                     Status = "Pending",
                     StudentEmail = "test@m",
                     AnswerText = "Test",
                     ComponentName = "Academic",
                     PollName = "A",
                     PollUuid = "A",
                     VariableName = "A",
                 }
            }.AsQueryable().BuildMockDbSet();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "RecentAlertsTest_Empty")
            .Options;

        _mockContext = new Mock<AppDbContext>(options);

        _mockContext
            .Setup(C => C.ErasEvaluationDetailsView)
            .Returns(data.Object);

        _repository = new ErasEvaluationDetailsViewRepository(_mockContext.Object);

        // Act
        var result = await _repository.GetRecentAlertsStudentAsync(1, 2);
        var resultLength = await _repository.CountRecentAlerts();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, resultLength);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByFiltersAsync_WhenAllFiltersMatch_ReturnsMatchingEntityAsync()
    {
        // Arrange
        var data = new List<ErasEvaluationDetailsViewEntity>
        {
            new()
            {
                PollId = 10,
                CohortId = 20,
                ComponentId = 30,
                VariableId = 40,
                StudentId = 1,
                StudentName = "Student 1",
                ComponentName = "Academic",
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "st@mail.com",
                PollUuid = "1",
                AnswerText = "I lN",
                VariableName = "ofhr",
                Status = "Progress"
            },
            new()
            {
                PollId = 99,
                CohortId = 20,
                ComponentId = 30,
                VariableId = 40,
                StudentId = 2,
                StudentName = "Student 2",
                ComponentName = "Academic",
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "st@mail.com",
                PollUuid = "1",
                AnswerText = "I lN",
                VariableName = "ofhr",
                Status = "Done"
            }
        }.AsQueryable().BuildMockDbSet();

        _mockContext
            .Setup(C => C.Set<ErasEvaluationDetailsViewEntity>())
            .Returns(data.Object);

        _repository = new ErasEvaluationDetailsViewRepository(_mockContext.Object);

        // Act
        var result = await _repository.GetByFiltersAsync(10, [30], [20], [40]);

        // Assert
        var item = Assert.Single(result);
    }


    [Fact]
    public async Task GetByFiltersAsync_WhenPollIdIsNull_DoesNotFilterByPollAsync()
    {
        // Arrange
        var data = new List<ErasEvaluationDetailsViewEntity>
        {
            new()
            {
                PollId = 10,
                CohortId = 20,
                ComponentId = 30,
                VariableId = 40,
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "st@mail.com",
                PollUuid = "1",
                AnswerText = "I lN",
                VariableName = "ofhr",
                Status = "Done",
                StudentName = "student1",
                ComponentName = "Academic"
            },
            new()
            {
                PollId = 20,
                CohortId = 20,
                ComponentId = 30,
                VariableId = 40,
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "st@mail.com",
                PollUuid = "1",
                AnswerText = "I lN",
                VariableName = "ofhr",
                Status = "Done",
                StudentName = "studn2",
                ComponentName = "academic"
            }
        }.AsQueryable().BuildMockDbSet();

        _mockContext
            .Setup(C => C.Set<ErasEvaluationDetailsViewEntity>())
            .Returns(data.Object);

        _repository = new ErasEvaluationDetailsViewRepository(_mockContext.Object);

        // Act
        var result = await _repository.GetByFiltersAsync(null, [30], [20], [40]);

        // Assert
        Assert.Equal(2, result.Count);
    }


    [Fact]
    public async Task GetByFiltersAsync_WhenComponentIdsAreNull_DoesNotFilterByComponentAsync()
    {
        // Arrange
        var data = new List<ErasEvaluationDetailsViewEntity>
    {
        new()
        {
            PollId = 10,
            CohortId = 20,
            ComponentId = 30,
            VariableId = 40,
            EvaluationName = "Eval1",
            PollName = "Poll",
            StudentEmail = "st@mail.com",
            PollUuid = "1",
            AnswerText = "I lN",
            VariableName = "ofhr",
            Status = "Done",
            ComponentName = "Psyco",
            StudentName = "abby"
        },
        new()
        {
            PollId = 10,
            CohortId = 20,
            ComponentId = 99,
            VariableId = 40,
            EvaluationName = "Eval1",
            PollName = "Poll",
            StudentEmail = "st@mail.com",
            PollUuid = "1",
            AnswerText = "I lN",
            VariableName = "ofhr",
            Status = "Done",
            ComponentName = "Normal",
            StudentName = "abby"
        }
    }.AsQueryable().BuildMockDbSet();

        _mockContext
            .Setup(C => C.Set<ErasEvaluationDetailsViewEntity>())
            .Returns(data.Object);

        _repository = new ErasEvaluationDetailsViewRepository(_mockContext.Object);

        // Act
        var result = await _repository.GetByFiltersAsync(10, null, [20], [40]);

        // Assert
        Assert.Equal(2, result.Count);
    }


    [Fact]
    public async Task GetByFiltersAsync_WhenCohortIdsAreNull_DoesNotFilterByCohortAsync()
    {
        // Arrange
        var data = new List<ErasEvaluationDetailsViewEntity>
        {
            new()
            {
                PollId = 10,
                CohortId = 20,
                ComponentId = 30,
                VariableId = 40,
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "st@mail.com",
                PollUuid = "1",
                AnswerText = "I lN",
                VariableName = "ofhr",
                Status = "Done",
                ComponentName = "Normal",
                StudentName = "stu1"
            },
            new()
            {
                PollId = 10,
                CohortId = 99,
                ComponentId = 30,
                VariableId = 40,
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "st@mail.com",
                PollUuid = "1",
                AnswerText = "I lN",
                VariableName = "ofhr",
                Status = "Done",
                ComponentName = "Normal",
                StudentName = "stu1"
            }
        }.AsQueryable().BuildMockDbSet();

        _mockContext
            .Setup(C => C.Set<ErasEvaluationDetailsViewEntity>())
            .Returns(data.Object);

        _repository = new ErasEvaluationDetailsViewRepository(_mockContext.Object);

        // Act
        var result = await _repository.GetByFiltersAsync(10, [30], null, [40]);

        // Assert
        Assert.Equal(2, result.Count);
    }


    [Fact]
    public async Task GetByFiltersAsync_WhenVariableIdsAreNull_DoesNotFilterByVariableAsync()
    {
        // Arrange
        var data = new List<ErasEvaluationDetailsViewEntity>
        {
            new()
            {
                PollId = 10,
                CohortId = 20,
                ComponentId = 30,
                VariableId = 40,
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "st@mail.com",
                PollUuid = "1",
                AnswerText = "I lN",
                VariableName = "ofhr",
                Status = "Done",
                ComponentName = "Normal",
                StudentName = "stu1"
            },
            new()
            {
                PollId = 10,
                CohortId = 20,
                ComponentId = 30,
                VariableId = 99,
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "st@mail.com",
                PollUuid = "1",
                AnswerText = "I lN",
                VariableName = "ofhr",
                Status = "Done",
                ComponentName = "Normal",
                StudentName = "stu1"
            }
        }.AsQueryable().BuildMockDbSet();

        _mockContext
            .Setup(C => C.Set<ErasEvaluationDetailsViewEntity>())
            .Returns(data.Object);

        _repository = new ErasEvaluationDetailsViewRepository(_mockContext.Object);

        // Act
        var result = await _repository.GetByFiltersAsync(10, [30], [20], null);

        // Assert
        Assert.Equal(2, result.Count);
    }


    [Fact]
    public async Task GetByFiltersAsync_WhenOptionalListsAreEmpty_DoesNotApplyThoseFiltersAsync()
    {
        // Arrange
        var data = new List<ErasEvaluationDetailsViewEntity>
        {
            new()
            {
                PollId = 10,
                CohortId = 20,
                ComponentId = 30,
                VariableId = 40,
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "st@mail.com",
                PollUuid = "1",
                AnswerText = "I lN",
                VariableName = "ofhr",
                Status = "Done",
                ComponentName = "Normal",
                StudentName = "stu1"
            },
            new()
            {
                PollId = 10,
                CohortId = 99,
                ComponentId = 99,
                VariableId = 99,
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "st@mail.com",
                PollUuid = "1",
                AnswerText = "I lN",
                VariableName = "ofhr",
                Status = "Done",
                ComponentName = "Normal",
                StudentName = "stu1"
            }
        }.AsQueryable().BuildMockDbSet();

        _mockContext
            .Setup(C => C.Set<ErasEvaluationDetailsViewEntity>())
            .Returns(data.Object);

        _repository = new ErasEvaluationDetailsViewRepository(_mockContext.Object);

        // Act
        var result = await _repository.GetByFiltersAsync(10, [], [], []);

        // Assert
        Assert.Equal(2, result.Count);
    }


    [Fact]
    public async Task GetByFiltersAsync_WhenNoEntityMatches_ReturnsEmptyListAsync()
    {
        // Arrange
        var data = new List<ErasEvaluationDetailsViewEntity>
    {
        new()
        {
            PollId = 10,
            CohortId = 20,
            ComponentId = 30,
            VariableId = 40,
            PollName = "Poll",
            EvaluationName = "Eval1",
            StudentEmail = "st@mail.com",
            PollUuid = "1",
            AnswerText = "I lN",
            VariableName = "ofhr",
            Status = "Done",
            ComponentName = "Normal",
            StudentName = "stu1"
        }
    }.AsQueryable().BuildMockDbSet();

        _mockContext
            .Setup(C => C.Set<ErasEvaluationDetailsViewEntity>())
            .Returns(data.Object);

        _repository = new ErasEvaluationDetailsViewRepository(_mockContext.Object);

        // Act
        var result = await _repository.GetByFiltersAsync(999, [999], [999], [999]);

        // Assert
        Assert.Empty(result);
    }


    [Fact]
    public async Task GetStudentsByEvaluationIdFilters_ReturnsMatchingStudentsAsync()
    {
        // Arrange
        var startDate = new DateTime(2026, 1, 1);
        var endDate = new DateTime(2026, 1, 31);

        var data = new List<ErasEvaluationDetailsViewEntity>
        {
            new()
            {
                EvaluationId = 10,
                CohortId = 20,
                ComponentName = "Academic",
                FinishedAt = new DateTime(2026, 1, 15),
                StudentId = 1,
                StudentName = "Student 1",
                StudentEmail = "student1@test.com",
                AnswerId = 100,
                AnswerText = "Yes",
                RiskLevel = 2,
                EvaluationName = "Eval1",
                PollName = "Poll",
                PollUuid = "1",
                VariableName = "ofhr",
                Status = "Done"
            },
            new()
            {
                EvaluationId = 99,
                CohortId = 20,
                ComponentName = "Academic",
                FinishedAt = new DateTime(2026, 1, 15),
                StudentId = 2,
                StudentName = "Student 2",
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "st@mail.com",
                PollUuid = "1",
                AnswerText = "I lN",
                VariableName = "ofhr",
                Status = "InProgress"
            }
        }.AsQueryable().BuildMockDbSet();

        _mockContext
            .Setup(C => C.Set<ErasEvaluationDetailsViewEntity>())
            .Returns(data.Object);

        _repository = new ErasEvaluationDetailsViewRepository(_mockContext.Object);

        // Act
        var result = await _repository.GetStudentsByEvaluationIdFilters(10, ["Academic"], [20], null, null, startDate, endDate);

        // Assert
        var student = Assert.Single(result);

        Assert.Equal(1, student.Id);
        Assert.Equal("Student 1", student.Name);
        Assert.Equal("student1@test.com", student.Email);
        Assert.Equal(100, student.AnswerId);
        Assert.Equal("Yes", student.AnswerText);
        Assert.Equal(2, student.RiskLevel);
    }


    [Fact]
    public async Task GetStudentsByEvaluationIdFilters_WhenVariableIdsAreProvided_ReturnsOnlyMatchingVariablesAsync()
    {
        // Arrange
        var startDate = new DateTime(2026, 1, 1);
        var endDate = new DateTime(2026, 1, 31);

        var data = new List<ErasEvaluationDetailsViewEntity>
        {
            new()
            {
                EvaluationId = 10,
                CohortId = 20,
                ComponentName = "Academic",
                VariableId = 100,
                FinishedAt = new DateTime(2026, 1, 15),
                StudentId = 1,
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "abb@mail.com",
                PollUuid = "1",
                AnswerText = "I lN",
                VariableName = "ofhr",
                Status = "Done",
                StudentName = "abby",
            }
        }.AsQueryable().BuildMockDbSet();
        _mockContext
            .Setup(C => C.Set<ErasEvaluationDetailsViewEntity>())
            .Returns(data.Object);

        _repository = new ErasEvaluationDetailsViewRepository(_mockContext.Object);

        // Act
        var result = await _repository.GetStudentsByEvaluationIdFilters(10, ["Academic"], [20],[100], null, startDate, endDate);

        // Assert
        var student = Assert.Single(result);
        Assert.Equal(1, student.Id);
    }


    [Fact]
    public async Task GetStudentsByEvaluationIdFilters_WhenRiskLevelsProvided_FiltersByRiskGroupAsync()
    {
        // Arrange
        var startDate = new DateTime(2026, 1, 1);
        var endDate = new DateTime(2026, 1, 31);

        var data = new List<ErasEvaluationDetailsViewEntity>
        {
            new()
            {
                EvaluationId = 10,
                CohortId = 20,
                ComponentName = "Academic",
                FinishedAt = new DateTime(2026, 1, 15),
                StudentId = 1,
                RiskLevel = 1.2m,
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "st@mail.com",
                PollUuid = "1",
                AnswerText = "I lN",
                VariableName = "ofhr",
                Status = "Done",
                StudentName = "studen1",
            },
            new()
            {
                EvaluationId = 10,
                CohortId = 20,
                ComponentName = "Academic",
                FinishedAt = new DateTime(2026, 1, 15),
                StudentId = 2,
                RiskLevel = 3.2m,
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "st@mail.com",
                PollUuid = "1",
                AnswerText = "I lN",
                VariableName = "ofhr",
                Status = "Done",
                StudentName = "studen1",
            }
        }.AsQueryable().BuildMockDbSet();

        _mockContext
            .Setup(C => C.Set<ErasEvaluationDetailsViewEntity>())
            .Returns(data.Object);

        _repository = new ErasEvaluationDetailsViewRepository(_mockContext.Object);

        // Act
        var result = await _repository.GetStudentsByEvaluationIdFilters(10, ["Academic"], [20], null, [1], startDate, endDate);

        // Assert
        var student = Assert.Single(result);
        Assert.Equal(1, student.Id);
    }


    [Fact]
    public async Task GetStudentsByEvaluationIdFilters_WhenRiskLevelsAreNull_DoesNotFilterByRiskAsync()
    {
        // Arrange
        var startDate = new DateTime(2026, 1, 1);
        var endDate = new DateTime(2026, 1, 31);

        var data = new List<ErasEvaluationDetailsViewEntity>
        {
            new()
            {
                EvaluationId = 10,
                CohortId = 20,
                ComponentName = "Academic",
                FinishedAt = new DateTime(2026, 1, 15),
                StudentId = 1,
                RiskLevel = 1,
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "st@mail.com",
                PollUuid = "1",
                AnswerText = "I lN",
                VariableName = "ofhr",
                Status = "Done",
                StudentName = "studen1",
            },
            new()
            {
                EvaluationId = 10,
                CohortId = 20,
                ComponentName = "Academic",
                FinishedAt = new DateTime(2026, 1, 15),
                StudentId = 2,
                RiskLevel = 5,
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "st@mail.com",
                PollUuid = "1",
                AnswerText = "I lN",
                VariableName = "ofhr",
                Status = "Done",
                StudentName = "studen1",
            }
        }.AsQueryable().BuildMockDbSet();

        _mockContext
            .Setup(C => C.Set<ErasEvaluationDetailsViewEntity>())
            .Returns(data.Object);

        _repository = new ErasEvaluationDetailsViewRepository(_mockContext.Object);

        // Act
        var result = await _repository.GetStudentsByEvaluationIdFilters(10, ["Academic"], [20], null, null, startDate, endDate);

        // Assert
        Assert.Equal(2, result.Count);
    }


    [Fact]
    public async Task GetStudentsByEvaluationIdFilters_RemovesDuplicateResponsesAsync()
    {
        // Arrange
        var startDate = new DateTime(2026, 1, 1);
        var endDate = new DateTime(2026, 1, 31);

        var data = new List<ErasEvaluationDetailsViewEntity>
        {
            new()
            {
                EvaluationId = 10,
                CohortId = 20,
                ComponentName = "Academic",
                FinishedAt = new DateTime(2026, 1, 15),
                StudentId = 1,
                StudentName = "Student 1",
                StudentEmail = "student@test.com",
                AnswerId = 100,
                AnswerText = "Yes",
                RiskLevel = 2,
                EvaluationName = "Eval1",
                PollName = "Poll",
                PollUuid = "1",
                VariableName = "large",
                Status = "Done",
            },
            new()
            {
                EvaluationId = 10,
                CohortId = 20,
                ComponentName = "Academic",
                FinishedAt = new DateTime(2026, 1, 15),
                StudentId = 1,
                StudentName = "Student 1",
                StudentEmail = "student@test.com",
                AnswerId = 100,
                AnswerText = "No",
                RiskLevel = 2,
                EvaluationName = "Eval1",
                PollName = "Poll",
                PollUuid = "1",
                VariableName = "large",
                Status = "Done"
            }
        }.AsQueryable().BuildMockDbSet();

        _mockContext
            .Setup(C => C.Set<ErasEvaluationDetailsViewEntity>())
            .Returns(data.Object);

        _repository = new ErasEvaluationDetailsViewRepository(_mockContext.Object);

        // Act
        var result = await _repository.GetStudentsByEvaluationIdFilters(10, ["Academic"], [20], null, null, startDate, endDate);

        // Assert
        Assert.Equal(2,result.Count);
    }


    [Fact]
    public async Task GetStudentsByFilters_WhenVariableIdsAreNull_ReturnsMatchingStudentsAsync()
    {
        // Arrange
        var startDate = new DateTime(2026, 1, 1);
        var endDate = new DateTime(2026, 1, 31);

        var data = new List<ErasEvaluationDetailsViewEntity>
        {
            new()
            {
                PollUuid = "poll-1",
                CohortId = 20,
                ComponentName = "Academic",
                VariableId = 100,
                FinishedAt = new DateTime(2026, 1, 15),
                StudentId = 1,
                StudentName = "Student 1",
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "st@mail.com",
                Status = "1",
                AnswerText = "I lN",
                VariableName = "ofhr"
            },
            new()
            {
                PollUuid = "poll-2",
                CohortId = 20,
                ComponentName = "Academic",
                VariableId = 100,
                FinishedAt = new DateTime(2026, 1, 15),
                StudentId = 2,
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "st@mail.com",
                Status = "1",
                AnswerText = "I lN",
                VariableName = "ofhr",
                StudentName = "k"
            }
        }.AsQueryable().BuildMockDbSet();

        _mockContext
            .Setup(C => C.Set<ErasEvaluationDetailsViewEntity>())
            .Returns(data.Object);

        _repository = new ErasEvaluationDetailsViewRepository(_mockContext.Object);

        // Act
        var result = (await _repository.GetStudentsByFilters(
            "poll-1", ["Academic"], [20], null, null, 1, 10, startDate, endDate)).ToList();

        // Assert
        var student = Assert.Single(result);
        Assert.Equal(1, student.StudentId);
    }


    [Fact]
    public async Task GetStudentsByFilters_WhenVariableIdsProvided_FiltersVariablesAsync()
    {
        // Arrange
        var startDate = new DateTime(2026, 1, 1);
        var endDate = new DateTime(2026, 1, 31);

        var data = new List<ErasEvaluationDetailsViewEntity>
        {
            new()
            {
                PollUuid = "poll-1",
                CohortId = 20,
                ComponentName = "Academic",
                VariableId = 100,
                FinishedAt = new DateTime(2026, 1, 15),
                StudentId = 1,
                StudentName = "Student 1",
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "st@mail.com",
                Status = "1",
                AnswerText = "I lN",
                VariableName = "ofhr"
            },
            new()
            {
                PollUuid = "poll-1",
                CohortId = 20,
                ComponentName = "Academic",
                VariableId = 200,
                FinishedAt = new DateTime(2026, 1, 15),
                StudentId = 2,
                StudentName = "Student 2",
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "st@mail.com",
                Status = "1",
                AnswerText = "I lN",
                VariableName = "ofhr"
            }
        }.AsQueryable().BuildMockDbSet();

        _mockContext
            .Setup(C => C.Set<ErasEvaluationDetailsViewEntity>())
            .Returns(data.Object);

        _repository = new ErasEvaluationDetailsViewRepository(_mockContext.Object);

        // Act
        var result = (await _repository.GetStudentsByFilters("poll-1",["Academic"], [20], [100],null,1, 10, startDate, endDate)).ToList();

        // Assert
        var student = Assert.Single(result);
        Assert.Equal(1, student.StudentId);
    }


    [Fact]
    public async Task GetStudentsByFilters_WhenEvaluationIdProvided_FiltersByEvaluationAsync()
    {
        // Arrange
        var startDate = new DateTime(2026, 1, 1);
        var endDate = new DateTime(2026, 1, 31);

        var data = new List<ErasEvaluationDetailsViewEntity>
        {
            new()
            {
                PollUuid = "poll-1",
                CohortId = 20,
                ComponentName = "Academic",
                FinishedAt = new DateTime(2026, 1, 15),
                EvaluationId = 10,
                StudentId = 1,
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "st@mail.com",
                Status = "1",
                AnswerText = "I lN",
                VariableName = "ofhr",
                StudentName = "studnet"
            },
            new()
            {
                PollUuid = "poll-1",
                CohortId = 20,
                ComponentName = "Academic",
                FinishedAt = new DateTime(2026, 1, 15),
                EvaluationId = 20,
                StudentId = 2,
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "st@mail.com",
                Status = "1",
                AnswerText = "I lN",
                VariableName = "ofhr",
                StudentName = "studnet"
            }
        }.AsQueryable().BuildMockDbSet();

        _mockContext
            .Setup(C => C.Set<ErasEvaluationDetailsViewEntity>())
            .Returns(data.Object);

        _repository = new ErasEvaluationDetailsViewRepository(_mockContext.Object);

        // Act
        var result = (await _repository.GetStudentsByFilters(
            "poll-1", ["Academic"], [20], null, null, 1, 10,startDate, endDate, 10)).ToList();

        // Assert
        var student = Assert.Single(result);
        Assert.Equal(1, student.StudentId);
    }


    [Fact]
    public async Task GetStudentsByFilters_WhenEvaluationIdIsNull_DoesNotFilterByEvaluationAsync()
    {
        // Arrange
        var startDate = new DateTime(2026, 1, 1);
        var endDate = new DateTime(2026, 1, 31);

        var data = new List<ErasEvaluationDetailsViewEntity>
        {
            new()
            {
                PollUuid = "poll-1",
                CohortId = 20,
                ComponentName = "Academic",
                FinishedAt = new DateTime(2026, 1, 15),
                EvaluationId = 10,
                StudentId = 1,
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "st@mail.com",
                Status = "1",
                AnswerText = "I lN",
                VariableName = "ofhr",
                StudentName = "stu1"
            },
            new()
            {
                PollUuid = "poll-1",
                CohortId = 20,
                ComponentName = "Academic",
                FinishedAt = new DateTime(2026, 1, 15),
                EvaluationId = 20,
                StudentId = 2,
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "st@mail.com",
                Status = "1",
                AnswerText = "I lN",
                VariableName = "ofhr",
                StudentName = "st2"
            }
        }.AsQueryable().BuildMockDbSet();

        _mockContext
            .Setup(C => C.Set<ErasEvaluationDetailsViewEntity>())
            .Returns(data.Object);

        _repository = new ErasEvaluationDetailsViewRepository(_mockContext.Object);

        // Act
        var result = (await _repository.GetStudentsByFilters(
            "poll-1",
            ["Academic"],
            [20],
            null,
            null,
            1,
            10,
            startDate,
            endDate)).ToList();

        // Assert
        Assert.Equal(2, result.Count);
    }


    [Fact]
    public async Task GetStudentsByFilters_WhenRiskLevelsProvided_FiltersStudentsAsync()
    {
        // Arrange
        var startDate = new DateTime(2026, 1, 1);
        var endDate = new DateTime(2026, 1, 31);

        var data = new List<ErasEvaluationDetailsViewEntity>
    {
        new()
        {
            PollUuid = "poll-1",
            CohortId = 20,
            ComponentName = "Academic",
            FinishedAt = new DateTime(2026, 1, 15),
            StudentId = 1,
            StudentName = "Student 1",
            RiskLevel = 1,
            EvaluationName = "Eval1",
            PollName = "Poll",
            StudentEmail = "st@mail.com",
            Status = "1",
            AnswerText = "I lN",
            VariableName = "ofhr",
        },
        new()
        {
            PollUuid = "poll-1",
            CohortId = 20,
            ComponentName = "Academic",
            FinishedAt = new DateTime(2026, 1, 15),
            StudentId = 2,
            StudentName = "Student 2",
            RiskLevel = 5,
            EvaluationName = "Eval1",
            PollName = "Poll",
            StudentEmail = "st@mail.com",
            Status = "1",
            AnswerText = "I lN",
            VariableName = "ofhr"
        }
    }.AsQueryable().BuildMockDbSet();

        _mockContext
            .Setup(C => C.Set<ErasEvaluationDetailsViewEntity>())
            .Returns(data.Object);

        _repository = new ErasEvaluationDetailsViewRepository(_mockContext.Object);

        // Act
        var result = (await _repository.GetStudentsByFilters("poll-1", ["Academic"], [20], null, [1], 1, 10, startDate, endDate)).ToList();

        // Assert
        var student = Assert.Single(result);
        Assert.Equal(1, student.StudentId);
    }


    [Fact]
    public async Task GetStudentsByFilters_WhenRiskLevelsAreEmpty_DoesNotFilterRiskAsync()
    {
        // Arrange
        var startDate = new DateTime(2026, 1, 1);
        var endDate = new DateTime(2026, 1, 31);

        var data = new List<ErasEvaluationDetailsViewEntity>
        {
            new()
            {
                PollUuid = "poll-1",
                CohortId = 20,
                ComponentName = "Academic",
                FinishedAt = new DateTime(2026, 1, 15),
                StudentId = 1,
                RiskLevel = 1,
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "st@mail.com",
                Status = "1",
                AnswerText = "I lN",
                VariableName = "ofhr",
                StudentName = "stud1"
            },
            new()
            {
                PollUuid = "poll-1",
                CohortId = 20,
                ComponentName = "Academic",
                FinishedAt = new DateTime(2026, 1, 15),
                StudentId = 2,
                RiskLevel = 5,
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "st@mail.com",
                Status = "1",
                AnswerText = "I lN",
                VariableName = "ofhr",
                StudentName = "stud2"
            }
        }.AsQueryable().BuildMockDbSet();

        _mockContext
            .Setup(C => C.Set<ErasEvaluationDetailsViewEntity>())
            .Returns(data.Object);

        _repository = new ErasEvaluationDetailsViewRepository(_mockContext.Object);

        // Act
        var result = (await _repository.GetStudentsByFilters("poll-1", ["Academic"], [20], null, [], 1, 10, startDate, endDate)).ToList();

        // Assert
        Assert.Equal(2, result.Count);
    }


    [Fact]
    public async Task GetStudentsByFilters_GroupsByStudentAndReturnsFirstRecordAsync()
    {
        // Arrange
        var startDate = new DateTime(2026, 1, 1);
        var endDate = new DateTime(2026, 1, 31);

        var data = new List<ErasEvaluationDetailsViewEntity>
        {
            new()
            {
                PollUuid = "poll-1",
                CohortId = 20,
                ComponentName = "Academic",
                FinishedAt = new DateTime(2026, 1, 15),
                StudentId = 1,
                StudentName = "Student 1",
                RiskLevel = 2,
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "st@mail.com",
                AnswerText = "I lN",
                VariableName = "ofhr",
                Status = "Done"
            },
            new()
            {
                PollUuid = "poll-1",
                CohortId = 20,
                ComponentName = "Academic",
                FinishedAt = new DateTime(2026, 1, 16),
                StudentId = 1,
                StudentName = "Student 1",
                RiskLevel = 3,
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "st@mail.com",
                AnswerText = "I lN",
                VariableName = "ofhr",
                Status = "Complete"
            },
            new()
            {
                PollUuid = "poll-1",
                CohortId = 20,
                ComponentName = "Academic",
                FinishedAt = new DateTime(2026, 1, 17),
                StudentId = 2,
                StudentName = "Student 2",
                RiskLevel = 4,
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "st@mail.com",
                AnswerText = "I lN",
                VariableName = "ofhr",
                Status = "Done"
            }
        }.AsQueryable().BuildMockDbSet();

        _mockContext
            .Setup(C => C.Set<ErasEvaluationDetailsViewEntity>())
            .Returns(data.Object);

        _repository = new ErasEvaluationDetailsViewRepository(_mockContext.Object);

        // Act
        var result = (await _repository.GetStudentsByFilters(
            "poll-1", ["Academic"], [20], null, null, 1, 10, startDate, endDate)).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].StudentId);
        Assert.Equal(2, result[1].StudentId);
    }


    [Fact]
    public async Task GetStudentsByFilters_AppliesPaginationAsync()
    {
        // Arrange
        var startDate = new DateTime(2026, 1, 1);
        var endDate = new DateTime(2026, 1, 31);

        var data = new List<ErasEvaluationDetailsViewEntity>
        {
            new()
            {
                PollUuid = "poll-1",
                CohortId = 20,
                ComponentName = "Academic",
                FinishedAt = new DateTime(2026, 1, 15),
                StudentId = 1,
                StudentName = "Alice",
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "st@mail.com",
                AnswerText = "I lN",
                VariableName = "ofhr",
                Status = "Done"
            },
            new()
            {
                PollUuid = "poll-1",
                CohortId = 20,
                ComponentName = "Academic",
                FinishedAt = new DateTime(2026, 1, 15),
                StudentId = 2,
                StudentName = "Bob",
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "st@mail.com",
                AnswerText = "I lN",
                VariableName = "ofhr",
                Status = "Done"
            },
            new()
            {
                PollUuid = "poll-1",
                CohortId = 20,
                ComponentName = "Academic",
                FinishedAt = new DateTime(2026, 1, 15),
                StudentId = 3,
                StudentName = "Charlie",
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "st@mail.com",
                AnswerText = "I lN",
                VariableName = "ofhr",
                Status = "Done"
            }
        }.AsQueryable().BuildMockDbSet();

        _mockContext
            .Setup(C => C.Set<ErasEvaluationDetailsViewEntity>())
            .Returns(data.Object);

        _repository = new ErasEvaluationDetailsViewRepository(_mockContext.Object);

        // Act
        var result = (await _repository.GetStudentsByFilters(
            "poll-1", ["Academic"], [20], null, null, 2, 1, startDate, endDate)).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal(2, result[0].StudentId);
    }


    [Fact]
    public async Task CountStudentsByFilters_ReturnsDistinctStudentCountAsync()
    {
        // Arrange
        var startDate = new DateTime(2026, 1, 1);
        var endDate = new DateTime(2026, 1, 31);

        var data = new List<ErasEvaluationDetailsViewEntity>
        {
            new()
            {
                PollUuid = "poll-1",
                CohortId = 20,
                ComponentName = "Academic",
                FinishedAt = new DateTime(2026, 1, 15),
                StudentId = 1,
                RiskLevel = 2,
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "st@mail.com",
                AnswerText = "I lN",
                VariableName = "ofhr",
                Status = "Done",
                StudentName = "total2"
            },
            new()
            {
                PollUuid = "poll-1",
                CohortId = 20,
                ComponentName = "Academic",
                FinishedAt = new DateTime(2026, 1, 16),
                StudentId = 1,
                RiskLevel = 3,
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "st@mail.com",
                AnswerText = "I lN",
                VariableName = "ofhr",
                Status = "Done",
                StudentName = "total2"
            },
            new()
            {
                PollUuid = "poll-1",
                CohortId = 20,
                ComponentName = "Academic",
                FinishedAt = new DateTime(2026, 1, 17),
                StudentId = 2,
                RiskLevel = 4,
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "st@mail.com",
                AnswerText = "back",
                VariableName = "ofhr",
                Status = "Done",
                StudentName = "total2"
            }
        }.AsQueryable().BuildMockDbSet();

        _mockContext
            .Setup(C => C.Set<ErasEvaluationDetailsViewEntity>())
            .Returns(data.Object);

        _repository = new ErasEvaluationDetailsViewRepository(_mockContext.Object);

        // Act
        var result = await _repository.CountStudentsByFilters(
            "poll-1", ["Academic"], [20], null, null, startDate, endDate);

        // Assert
        Assert.Equal(2, result);
    }


    [Fact]
    public async Task CountStudentsByFilters_WhenRiskFilterProvided_ReturnsOnlyMatchingStudentsAsync()
    {
        // Arrange
        var startDate = new DateTime(2026, 1, 1);
        var endDate = new DateTime(2026, 1, 31);

        var data = new List<ErasEvaluationDetailsViewEntity>
        {
            new()
            {
                PollUuid = "poll-1",
                CohortId = 20,
                ComponentName = "Academic",
                FinishedAt = new DateTime(2026, 1, 15),
                StudentId = 1,
                RiskLevel = 1,
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "st@mail.com",
                AnswerText = "I lN",
                Status = "Done",
                StudentName = "total2",
                VariableName = "ofhr",
            },
            new()
            {
                PollUuid = "poll-1",
                CohortId = 20,
                ComponentName = "Academic",
                FinishedAt = new DateTime(2026, 1, 15),
                StudentId = 2,
                RiskLevel = 5,
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "st@mail.com",
                AnswerText = "ctablN",
                VariableName = "ofhr",
                Status = "Done",
                StudentName = "total2"
            }
        }.AsQueryable().BuildMockDbSet();

        _mockContext
            .Setup(C => C.Set<ErasEvaluationDetailsViewEntity>())
            .Returns(data.Object);

        _repository = new ErasEvaluationDetailsViewRepository(_mockContext.Object);

        // Act
        var result = await _repository.CountStudentsByFilters("poll-1", ["Academic"], [20], null, [1], startDate, endDate);

        // Assert
        Assert.Equal(1, result);
    }


    [Fact]
    public async Task CountStudentsByFilters_WhenEvaluationIdProvided_OnlyCountsEvaluationAsync()
    {
        // Arrange
        var startDate = new DateTime(2026, 1, 1);
        var endDate = new DateTime(2026, 1, 31);

        var data = new List<ErasEvaluationDetailsViewEntity>
    {
        new()
        {
            PollUuid = "poll-1",
            CohortId = 20,
            ComponentName = "Academic",
            FinishedAt = new DateTime(2026, 1, 15),
            EvaluationId = 10,
            StudentId = 1,
            EvaluationName = "Eval1",
            PollName = "Poll",
            StudentEmail = "st@mail.com",
            AnswerText = "Icomp lN",
            VariableName = "ofhr",
            Status = "Done",
            StudentName = "stu1"
        },
        new()
        {
            PollUuid = "poll-1",
            CohortId = 20,
            ComponentName = "Academic",
            FinishedAt = new DateTime(2026, 1, 15),
            EvaluationId = 20,
            StudentId = 2,
            EvaluationName = "Eval1",
            PollName = "Poll",
            StudentEmail = "st@mail.com",
            AnswerText = "ntrinN",
            VariableName = "ofhr",
            Status = "Done",
            StudentName = "stu1"
        }
    }.AsQueryable().BuildMockDbSet();

        _mockContext
            .Setup(C => C.Set<ErasEvaluationDetailsViewEntity>())
            .Returns(data.Object);

        _repository = new ErasEvaluationDetailsViewRepository(_mockContext.Object);

        // Act
        var result = await _repository.CountStudentsByFilters(
            "poll-1", ["Academic"], [20], null, null, startDate, endDate, 10);

        // Assert
        Assert.Equal(1, result);
    }


    [Fact]
    public async Task CountRecentAlerts_ReturnsNumberOfGroupedStudentComponentsAsync()
    {
        // Arrange
        var data = new List<ErasEvaluationDetailsViewEntity>
    {
        new()
        {
            StudentId = 1,
            StudentName = "Student 1",
            ComponentName = "Academic",
            Status = "Completed",
            RiskLevel = 2,
            FinishedAt = new DateTime(2026, 1, 1),
            EvaluationName = "Eval1",
            PollName = "Poll",
            StudentEmail = "st@mail.com",
            PollUuid = "1",
            AnswerText = "ceacepma",
            VariableName = "vk"
        },
        new()
        {
            StudentId = 1,
            StudentName = "Student 1",
            ComponentName = "Academic",
            Status = "Completed",
            RiskLevel = 4,
            FinishedAt = new DateTime(2026, 1, 2),
            EvaluationName = "Eval1",
            PollName = "Poll",
            StudentEmail = "st@mail.com",
            PollUuid = "1",
            AnswerText = "comp",
            VariableName = "motiv"
        },
        new()
        {
            StudentId = 1,
            StudentName = "Student 1",
            ComponentName = "Attendance",
            Status = "Completed",
            RiskLevel = 3,
            FinishedAt = new DateTime(2026, 1, 3),
            EvaluationName = "Eval1",
            PollName = "Poll",
            StudentEmail = "st@mail.com",
            PollUuid = "1",
            AnswerText = "evalu",
            VariableName = "presebr"
        }
    }.AsQueryable().BuildMockDbSet();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _mockContext = new Mock<AppDbContext>(options);

        _mockContext
            .Setup(C => C.ErasEvaluationDetailsView)
            .Returns(data.Object);

        _repository = new ErasEvaluationDetailsViewRepository(_mockContext.Object);

        // Act
        var result = await _repository.CountRecentAlerts();

        // Assert
        Assert.Equal(2, result);
    }


    [Fact]
    public async Task GetRecentAlertsStudentAsync_GroupsByStudentAndComponentAsync()
    {
        // Arrange
        var data = new List<ErasEvaluationDetailsViewEntity>
    {
        new()
        {
            StudentId = 1,
            StudentName = "Student 1",
            ComponentName = "Academic",
            Status = "Completed",
            RiskLevel = 2,
            FinishedAt = new DateTime(2026, 1, 1),
            EvaluationName = "Eval1",
            PollName = "Poll",
            StudentEmail = "st@mail.com",
            PollUuid = "1",
            AnswerText = "imyaaf",
            VariableName = "ofhr"
        },
        new()
        {
            StudentId = 1,
            StudentName = "Student 1",
            ComponentName = "Academic",
            Status = "Completed",
            RiskLevel = 4,
            FinishedAt = new DateTime(2026, 1, 2),
            EvaluationName = "Eval1",
            PollName = "Poll",
            StudentEmail = "st@mail.com",
            PollUuid = "1",
            AnswerText = "obc",
            VariableName = "desesp"
            }
        }.AsQueryable().BuildMockDbSet();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _mockContext = new Mock<AppDbContext>(options);

        _mockContext
            .Setup(C => C.ErasEvaluationDetailsView)
            .Returns(data.Object);

        _repository = new ErasEvaluationDetailsViewRepository(_mockContext.Object);

        // Act
        var result = (await _repository.GetRecentAlertsStudentAsync(1, 10)).ToList();

        // Assert
        var item = Assert.Single(result);

        Assert.Equal("1", item.StudentId);
        Assert.Equal("Student 1", item.StudentName);
        Assert.Equal("Academic", item.Category);
    }


    [Fact]
    public async Task GetRecentAlertsStudentAsync_ExcludesInvalidStatusesAsync()
    {
        // Arrange
        var data = new List<ErasEvaluationDetailsViewEntity>
        {
            new()
            {
                StudentId = 1,
                StudentName = "Student 1",
                ComponentName = "Academic",
                Status = "Pending",
                RiskLevel = 5,
                FinishedAt = new DateTime(2026, 1, 1),
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "st@mail.com",
                PollUuid = "1",
                AnswerText = "I lN",
                VariableName = "ofhr"
            },
            new()
            {
                StudentId = 2,
                StudentName = "Student 2",
                ComponentName = "Academic",
                Status = "Cancelled",
                RiskLevel = 5,
                FinishedAt = new DateTime(2026, 1, 2),
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "st@mail.com",
                PollUuid = "1",
                AnswerText = "Iiaq",
                VariableName = "ofhr"
            }
        }.AsQueryable().BuildMockDbSet();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _mockContext = new Mock<AppDbContext>(options);

        _mockContext
            .Setup(C => C.ErasEvaluationDetailsView)
            .Returns(data.Object);

        _repository = new ErasEvaluationDetailsViewRepository(_mockContext.Object);

        // Act
        var result = await _repository.GetRecentAlertsStudentAsync(1, 10);

        // Assert
        Assert.Empty(result);
    }


    [Fact]
    public async Task GetRecentAlertsStudentAsync_OrdersByRiskThenDateDescendingAsync()
    {
        // Arrange
        var data = new List<ErasEvaluationDetailsViewEntity>
        {
            new()
            {
                StudentId = 1,
                StudentName = "Low Risk",
                ComponentName = "Academic",
                Status = "Completed",
                RiskLevel = 1,
                FinishedAt = new DateTime(2026, 1, 10),
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "st@mail.com",
                PollUuid = "1",
                AnswerText = "mracftf",
                VariableName = "oc"
            },
            new()
            {
                StudentId = 2,
                StudentName = "High Risk",
                ComponentName = "Academic",
                Status = "Completed",
                RiskLevel = 5,
                FinishedAt = new DateTime(2026, 1, 1),
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "st@mail.com",
                PollUuid = "1",
                AnswerText = "apmvN",
                VariableName = "depl"
            },
            new()
            {
                StudentId = 3,
                StudentName = "High Risk Recent",
                ComponentName = "Academic",
                Status = "Completed",
                RiskLevel = 5,
                FinishedAt = new DateTime(2026, 1, 20),
                EvaluationName = "Eval1",
                PollName = "Poll",
                StudentEmail = "st@mail.com",
                PollUuid = "1",
                AnswerText = "ttbmpN",
                VariableName = "irntsbmf"
            }
        }.AsQueryable().BuildMockDbSet();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _mockContext = new Mock<AppDbContext>(options);

        _mockContext
            .Setup(C => C.ErasEvaluationDetailsView)
            .Returns(data.Object);

        _repository = new ErasEvaluationDetailsViewRepository(_mockContext.Object);

        // Act
        var result = (await _repository.GetRecentAlertsStudentAsync(1, 10)).ToList();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal("3", result[0].StudentId);
        Assert.Equal("2", result[1].StudentId);
        Assert.Equal("1", result[2].StudentId);
    }
}
