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
    public async Task GetRecentAlertsStudentAsync_Should_Return_First_Page()
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
            .Setup(c => c.ErasEvaluationDetailsView)
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
    public async Task GetRecentAlertsStudentAsync_Should_Return_Second_Page()
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
            .Setup(c => c.ErasEvaluationDetailsView)
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
    public async Task GetRecentAlertsStudentAsync_Should_Return_Empty_When_Evaluations_AreNot_Completed_InProgresss()
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
            .Setup(c => c.ErasEvaluationDetailsView)
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
}
