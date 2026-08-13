using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;
using Eras.Infrastructure.Tests.Persistence.PostgreSQL.Utils;

using Microsoft.EntityFrameworkCore;

using MockQueryable.Moq;

using Moq;

using Xunit;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories;

public class PollRepositoryTest : RepositoryTestBase
{
    private readonly Mock<AppDbContext> _mockContext;
    private readonly Mock<DbSet<PollEntity>> _mockSet;
    private readonly PollRepository _repository;

    public PollRepositoryTest()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _mockContext = new Mock<AppDbContext>(options);
        _mockSet = new Mock<DbSet<PollEntity>>();

        _repository = new PollRepository(_mockContext.Object);
    }

    [Fact]
    public async Task GetByNameAsync_ShouldReturnPoll_WhenPollExistsAsync()
    {
        // Arrange
        var data = new List<PollEntity>
        {
            new PollEntity
            {
                Id = 1,
                Name = "Test Poll",
                ParentId = "parent-1",
                Uuid = "uuid-1"
            }
        }
        .AsQueryable()
        .BuildMockDbSet();

        _mockContext
            .Setup(X => X.Polls)
            .Returns(data.Object);

        // Act
        var result = await _repository.GetByNameAsync("Test Poll");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Poll", result.Name);
        Assert.Equal("parent-1", result.ParentId);
        Assert.Equal("uuid-1", result.Uuid);
    }

    [Fact]
    public async Task GetByNameAsync_ShouldReturnNull_WhenPollDoesNotExistAsync()
    {
        // Arrange
        var data = new List<PollEntity>
        {
            new PollEntity
            {
                Id = 1,
                Name = "Test Poll",
                ParentId = "parent-1",
                Uuid = "uuid-1"
            }
        }
        .AsQueryable()
        .BuildMockDbSet();

        _mockContext
            .Setup(X => X.Polls)
            .Returns(data.Object);

        // Act
        var result = await _repository.GetByNameAsync("Unknown Poll");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByParentIdAsync_ShouldReturnPoll_WhenPollExistsAsync()
    {
        // Arrange
        var data = new List<PollEntity>
        {
            new PollEntity
            {
                Id = 1,
                Name = "Test Poll",
                ParentId = "parent-1",
                Uuid = "uuid-1"
            }
        }
        .AsQueryable()
        .BuildMockDbSet();

        _mockContext
            .Setup(X => X.Polls)
            .Returns(data.Object);

        // Act
        var result = await _repository.GetByParentIdAsync("parent-1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Poll", result.Name);
        Assert.Equal("parent-1", result.ParentId);
    }

    [Fact]
    public async Task GetByParentIdAsync_ShouldReturnNull_WhenPollDoesNotExistAsync()
    {
        // Arrange
        var data = new List<PollEntity>
        {
            new PollEntity
            {
                Id = 1,
                Name = "Test Poll",
                ParentId = "parent-1",
                Uuid = "uuid-1"
            }
        }
        .AsQueryable()
        .BuildMockDbSet();

        _mockContext
            .Setup(X => X.Polls)
            .Returns(data.Object);

        // Act
        var result = await _repository.GetByParentIdAsync("unknown-parent");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUuidAsync_ShouldReturnPoll_WhenPollExistsAsync()
    {
        // Arrange
        var data = new List<PollEntity>
        {
            new PollEntity
            {
                Id = 1,
                Name = "Test Poll",
                ParentId = "parent-1",
                Uuid = "uuid-1"
            }
        }
        .AsQueryable()
        .BuildMockDbSet();

        _mockContext
            .Setup(X => X.Polls)
            .Returns(data.Object);

        // Act
        var result = await _repository.GetByUuidAsync("uuid-1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Poll", result.Name);
        Assert.Equal("uuid-1", result.Uuid);
    }

    [Fact]
    public async Task GetByUuidAsync_ShouldReturnNull_WhenPollDoesNotExistAsync()
    {
        // Arrange
        var data = new List<PollEntity>
        {
            new PollEntity
            {
                Id = 1,
                Name = "Test Poll",
                ParentId = "parent-1",
                Uuid = "uuid-1"
            }
        }
        .AsQueryable()
        .BuildMockDbSet();

        _mockContext
            .Setup(X => X.Polls)
            .Returns(data.Object);

        // Act
        var result = await _repository.GetByUuidAsync("unknown-uuid");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateExistingPollAsync()
    {
        // Arrange
        using var context = CreateContext();

        var existingPoll = new PollEntity
        {
            Id = 1,
            Name = "Old Name",
            ParentId = "old-parent",
            Uuid = "old-uuid"
        };

        context.Polls.Add(existingPoll);
        await context.SaveChangesAsync();

        var repository = new PollRepository(context);

        var poll = new Poll
        {
            Id = 1,
            Name = "New Name",
            ParentId = "new-parent",
            Uuid = "new-uuid"
        };

        // Act
        var result = await repository.UpdateAsync(poll);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("New Name", result.Name);

        var updatedEntity = await context.Polls.FindAsync(1);

        Assert.NotNull(updatedEntity);
        Assert.Equal("New Name", updatedEntity.Name);
        Assert.Equal("new-parent", updatedEntity.ParentId);
        Assert.Equal("new-uuid", updatedEntity.Uuid);
    }

    [Fact]
    public async Task UpdateAsync_ShouldNotAddPoll_WhenPollDoesNotExistAsync()
    {
        // Arrange
        using var context = CreateContext();

        var repository = new PollRepository(context);

        var poll = new Poll
        {
            Id = 999,
            Name = "New Poll",
            ParentId = "parent-1",
            Uuid = "uuid-1"
        };

        // Act
        var result = await repository.UpdateAsync(poll);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(999, result.Id);

        var entity = await context.Polls.FindAsync(999);

        Assert.Null(entity);
    }
}
