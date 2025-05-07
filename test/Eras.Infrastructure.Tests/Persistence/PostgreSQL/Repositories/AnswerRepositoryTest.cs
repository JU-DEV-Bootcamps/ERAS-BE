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

        Assert.NotNull(result);
        Assert.Single(result);
    }
}
