using Eras.Application.Contracts.Persistence;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Moq;
using MockQueryable.Moq;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories;
public class AnswerRepositoryTest
{
    private Mock<DbSet<AnswerEntity>> _mockSet;
    protected Mock<AppDbContext> _mockContext;
    private IAnswerRepository? _repository;

    public AnswerRepositoryTest()
    {
        _mockSet = new Mock<DbSet<AnswerEntity>>();
        _mockContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
    }

    [Fact]
    public void GetByStudentId_Should_Return()
    {
        var dataStudents = new List<StudentEntity>() {
            new StudentEntity(){ Id = 1, Uuid = "1"},
        }.AsQueryable().BuildMockDbSet();
        var dataPollIsntances = new List<PollInstanceEntity>()
        { new PollInstanceEntity { Id = 1, StudentId = 1 } }.AsQueryable().BuildMockDbSet();
        var data = new List<AnswerEntity>()
        { 
            new AnswerEntity()
            {
                Id = 1,
                PollInstanceId = 1,
                AnswerText = "Answer1",
                PollVariableId = 1
            },
            new AnswerEntity()
            {
                Id = 2,
                PollInstanceId = 2,
                AnswerText = "Answer2",
                PollVariableId = 1
            }
        }.AsQueryable().BuildMockDbSet();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "VariableTest")
            .Options;

        _mockContext = new Mock<AppDbContext>(options);
        _mockContext
            .Setup(C => C.Answers)
            .Returns(data.Object);
        _mockContext.Setup(C => C.Students).Returns(dataStudents.Object);
        _mockContext.Setup(C => C.PollInstances).Returns(dataPollIsntances.Object);
        _repository = new AnswerRepository(_mockContext.Object);

        // Act
        var result = _repository.GetByStudentIdAsync("1").Result;
        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetByStudentIdAsync_ShouldReturnEmpty_WhenStudentDoesNotExistAsync()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);
        var repository = new AnswerRepository(context);
        var result = await repository.GetByStudentIdAsync("unknown");

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByStudentIdAsync_ShouldReturnEmpty_WhenStudentHasNoPollsAsync()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        using var context = new AppDbContext(options);
        context.Students.Add(new StudentEntity
        {
            Id = 1,
            Uuid = "student",
            Email = "stu@mail.com",
            Name = "Test",
        });

        context.SaveChanges();

        var repository = new AnswerRepository(context);
        var result = await repository.GetByStudentIdAsync("student");

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByStudentIdAsync_ShouldReturnAnswersFromLatestPollAsync()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);

        context.Students.Add(new StudentEntity
        {
            Id = 1,
            Uuid = "student",
            Email = "stu@mail.com",
            Name = "Test",
        });

        context.PollInstances.AddRange(
            new PollInstanceEntity
            {
                Id = 1,
                StudentId = 1,
                FinishedAt = DateTime.UtcNow.AddDays(-1)
            },
            new PollInstanceEntity
            {
                Id = 2,
                StudentId = 1,
                FinishedAt = DateTime.UtcNow
            });

        context.Answers.AddRange(
            new AnswerEntity
            {
                Id = 1,
                PollInstanceId = 1,
                AnswerText = "Old"
            },
            new AnswerEntity
            {
                Id = 2,
                PollInstanceId = 2,
                AnswerText = "Latest"
            });

        context.SaveChanges();
        var repository = new AnswerRepository(context);
        var result = await repository.GetByStudentIdAsync("student");

        Assert.Single(result);
        Assert.Equal("Latest", result[0].AnswerText);
    }

    [Fact]
    public async Task GetByPollInstanceIdAsync_ShouldReturnAnswersAsync()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);

        context.Answers.AddRange(
            new AnswerEntity
            {
                Id = 1,
                PollInstanceId = 5,
                AnswerText = "A"
            },
            new AnswerEntity
            {
                Id = 2,
                PollInstanceId = 5,
                AnswerText = "B"
            },
            new AnswerEntity
            {
                Id = 3,
                PollInstanceId = 8,
                AnswerText = "C"
            });

        context.SaveChanges();
        var repository = new AnswerRepository(context);
        var result = await repository.GetByPollInstanceIdAsync(5);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetByPollInstanceAnswerAndPollVariableAsync_ShouldFilterCorrectlyAsync()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);
        context.Answers.AddRange(
            new AnswerEntity
            {
                Id = 1,
                PollInstanceId = 10,
                PollVariableId = 2,
                AnswerText = "Yes"
            },
            new AnswerEntity
            {
                Id = 2,
                PollInstanceId = 10,
                PollVariableId = 2,
                AnswerText = "No"
            });

        context.SaveChanges();

        var repository = new AnswerRepository(context);

        var result = await repository.GetByPollInstanceAnswerAndPollVariableAsync(2, 10, "Yes");

        Assert.Single(result);
        Assert.Equal("Yes", result[0].AnswerText);
    }

    [Fact]
    public async Task GetAnswerIdByPollInstanceAndVariableAsync_ShouldReturnIdAsync()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);

        context.Answers.Add(new AnswerEntity
        {
            Id = 100,
            PollInstanceId = 5,
            PollVariableId = 9
        });

        context.SaveChanges();

        var repository = new AnswerRepository(context);

        var id = await repository.GetAnswerIdByPollInstanceAndVariableAsync(9, 5);

        Assert.Equal(100, id);
    }

    [Fact]
    public async Task GetAnswerIdByPollInstanceAndVariableAsync_ShouldReturnNull_WhenMissingAsync()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        using var context = new AppDbContext(options);
        var repository = new AnswerRepository(context);
        var id = await repository.GetAnswerIdByPollInstanceAndVariableAsync(1, 1);

        Assert.Null(id);
    }
}
