namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories;

using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;

using Microsoft.EntityFrameworkCore;

using Xunit;

public class VariableRepositoryTest
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetByNameAsync_ShouldReturnVariable_WhenVariableExistsAsync()
    {
        // Arrange
        await using var context = CreateContext();

        context.Variables.Add(new VariableEntity
        {
            Id = 1,
            Name = "Variable 1",
            Position = 1
        });

        await context.SaveChangesAsync();

        var repository = new VariableRepository(context);

        // Act
        var result = await repository.GetByNameAsync("Variable 1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Variable 1", result.Name);
        Assert.Equal(1, result.Position);
    }

    [Fact]
    public async Task GetByNameAsync_ShouldReturnNull_WhenVariableDoesNotExistAsync()
    {
        // Arrange
        await using var context = CreateContext();

        context.Variables.Add(new VariableEntity
        {
            Id = 1,
            Name = "Variable 1",
            Position = 1
        });

        await context.SaveChangesAsync();

        var repository = new VariableRepository(context);

        // Act
        var result = await repository.GetByNameAsync("Unknown");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByNameAsync_ShouldReturnNull_WhenDatabaseIsEmptyAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var repository = new VariableRepository(context);

        // Act
        var result = await repository.GetByNameAsync("Variable 1");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByNameInPollAndComponentAsync_ShouldReturnVariable_WhenNameExistsAsync()
    {
        // Arrange
        await using var context = CreateContext();

        context.Variables.Add(new VariableEntity
        {
            Id = 1,
            Name = "Variable 1",
            Position = 1
        });

        await context.SaveChangesAsync();

        var repository = new VariableRepository(context);

        // Act
        var result = await repository.GetByNameInPollAndComponentAsync("Variable 1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Variable 1", result.Name);
    }

    [Fact]
    public async Task GetByNameInPollAndComponentAsync_ShouldReturnNull_WhenVariableDoesNotExistAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var repository = new VariableRepository(context);

        // Act
        var result = await repository.GetByNameInPollAndComponentAsync("Unknown");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_ShouldAddAndReturnVariableAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var repository = new VariableRepository(context);

        var variable = new Variable
        {
            Name = "New Variable",
            Position = 1
        };

        // Act
        var result = await repository.AddAsync(variable);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Variable", result.Name);
        Assert.Equal(1, result.Position);

        var persisted = await context.Variables
            .FirstOrDefaultAsync(V => V.Name == "New Variable");

        Assert.NotNull(persisted);
        Assert.Equal("New Variable", persisted.Name);
        Assert.Equal(1, persisted.Position);
    }

    [Fact]
    public async Task AddAsync_ShouldPersistVariableInDatabaseAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var repository = new VariableRepository(context);

        var variable = new Variable
        {
            Name = "Variable",
            Position = 5
        };

        // Act
        var result = await repository.AddAsync(variable);

        // Assert
        var count = await context.Variables.CountAsync();

        Assert.Equal(1, count);
        Assert.Equal(result.Id, (await context.Variables.SingleAsync()).Id);
    }

    [Fact]
    public async Task Add_ShouldThrowNotImplementedExceptionAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var repository = new VariableRepository(context);

        var variable = new Variable
        {
            Name = "Variable",
            Position = 1
        };

        // Act & Assert
        await Assert.ThrowsAsync<NotImplementedException>(
            () => repository.Add(variable));
    }

    [Fact]
    public async Task GetAllAsync_ShouldThrowNotImplementedExceptionAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var repository = new VariableRepository(context);

        // Act & Assert
        await Assert.ThrowsAsync<NotImplementedException>(
            () => repository.GetAllAsync(1));
    }

    [Fact]
    public async Task GetComponentVariableByName_ShouldThrowNotImplementedExceptionAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var repository = new VariableRepository(context);

        // Act & Assert
        await Assert.ThrowsAsync<NotImplementedException>(
            () => repository.GetComponentVariableByName("Variable"));
    }

    [Fact]
    public async Task GetAllByPollUuidAsync_ShouldReturnPreviousVersionVariables_WhenLastVersionIsFalseAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var component = new ComponentEntity
        {
            Id = 1,
            Name = "Component 1"
        };

        var poll = new PollEntity
        {
            Id = 10,
            Uuid = "poll-uuid",
            LastVersion = 2
        };

        var variable = new VariableEntity
        {
            Id = 100,
            Name = "Variable 1",
            Position = 1,
            ComponentId = 1
        };

        var pollVariable = new PollVariableJoin
        {
            Id = 1000,
            PollId = 10,
            VariableId = 100,
            Version = new Domain.Common.VersionInfo()
        };

        context.Components.Add(component);
        context.Polls.Add(poll);
        context.Variables.Add(variable);
        context.PollVariables.Add(pollVariable);

        await context.SaveChangesAsync();

        var repository = new VariableRepository(context);

        // Act
        var result = await repository.GetAllByPollUuidAsync("poll-uuid", [], false);

        // Assert
        Assert.Single(result);
        Assert.Equal("Variable 1", result[0].Name);
    }

    [Fact]
    public async Task GetAllByPollUuidAsync_ShouldExcludePreviousVersion_WhenLastVersionIsTrueAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var component = new ComponentEntity
        {
            Id = 1,
            Name = "Component 1"
        };

        var poll = new PollEntity
        {
            Id = 10,
            Uuid = "poll-uuid",
            LastVersion = 2
        };

        var variable1 = new VariableEntity
        {
            Id = 100,
            Name = "Current Variable",
            Position = 1,
            ComponentId = 1
        };

        var variable2 = new VariableEntity
        {
            Id = 200,
            Name = "Old Variable",
            Position = 2,
            ComponentId = 1
        };

        var currentVersion = new Domain.Common.VersionInfo()
        {
            VersionDate = DateTime.UtcNow,
            VersionNumber = 1
        };

        var oldVersion = new Domain.Common.VersionInfo()
        {
            VersionDate = DateTime.Now,
            VersionNumber = 2
        };

        context.Components.Add(component);
        context.Polls.Add(poll);
        context.Variables.AddRange(variable1, variable2);

        context.PollVariables.AddRange(
            new PollVariableJoin
            {
                Id = 1000,
                PollId = 10,
                VariableId = 100,
                Version = currentVersion
            },
            new PollVariableJoin
            {
                Id = 2000,
                PollId = 10,
                VariableId = 200,
                Version = oldVersion
            });

        await context.SaveChangesAsync();

        var repository = new VariableRepository(context);

        // Act
        var result = await repository.GetAllByPollUuidAsync("poll-uuid", [], true);

        // Assert
        Assert.Single(result);
        Assert.Equal("Old Variable", result[0].Name);
    }

    [Fact]
    public async Task GetAllByPollUuidAsync_ShouldReturnAllComponents_WhenComponentsIsEmptyAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var component1 = new ComponentEntity
        {
            Id = 1,
            Name = "Component 1"
        };

        var component2 = new ComponentEntity
        {
            Id = 2,
            Name = "Component 2"
        };

        var poll = new PollEntity
        {
            Id = 10,
            Uuid = "poll-uuid",
            LastVersion = 1
        };

        var version = new Domain.Common.VersionInfo()
        {
            VersionDate = DateTime.Now,
            VersionNumber = 1
        };

        var variable1 = new VariableEntity
        {
            Id = 100,
            Name = "Variable 1",
            Position = 2,
            ComponentId = 1
        };

        var variable2 = new VariableEntity
        {
            Id = 200,
            Name = "Variable 2",
            Position = 1,
            ComponentId = 2
        };

        context.Components.AddRange(component1, component2);
        context.Polls.Add(poll);
        context.Variables.AddRange(variable1, variable2);

        context.PollVariables.AddRange(
            new PollVariableJoin
            {
                Id = 1000,
                PollId = 10,
                VariableId = 100,
                Version = version
            },
            new PollVariableJoin
            {
                Id = 2000,
                PollId = 10,
                VariableId = 200,
                Version = version
            });

        await context.SaveChangesAsync();

        var repository = new VariableRepository(context);

        // Act
        var result = await repository.GetAllByPollUuidAsync("poll-uuid", [], true);

        // Assert
        Assert.Single(result);

        // Also verifies OrderBy(Position)
        Assert.Equal("Variable 2", result[0].Name);
    }

    [Fact]
    public async Task GetAllByPollUuidAsync_ShouldReturnEmpty_WhenPollUuidDoesNotExistAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var repository = new VariableRepository(context);

        // Act
        var result = await repository.GetAllByPollUuidAsync("unknown-poll", [], true);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllByPollUuidAsync_ShouldReturnEmpty_WhenNoVariablesMatchVersionAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var component = new ComponentEntity
        {
            Id = 1,
            Name = "Component"
        };

        var poll = new PollEntity
        {
            Id = 10,
            Uuid = "poll-uuid",
            LastVersion = 2
        };

        var variable = new VariableEntity
        {
            Id = 100,
            Name = "Variable",
            Position = 1,
            ComponentId = 1
        };

        var oldVersion = new Domain.Common.VersionInfo()
        {
            VersionDate = DateTime.Now,
            VersionNumber = 1
        };

        context.Components.Add(component);
        context.Polls.Add(poll);
        context.Variables.Add(variable);

        context.PollVariables.Add(new PollVariableJoin
        {
            Id = 1000,
            PollId = 10,
            VariableId = 100,
            Version = oldVersion
        });

        await context.SaveChangesAsync();

        var repository = new VariableRepository(context);

        // Act
        var result = await repository.GetAllByPollUuidAsync("poll-uuid", [], true);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByNameAndPollIdAsync_ShouldReturnVariable_WhenNameAndPollMatchAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var variable = new VariableEntity
        {
            Id = 1,
            Name = "Variable 1",
            Position = 1
        };

        var pollVariable = new PollVariableJoin
        {
            Id = 10,
            PollId = 100,
            VariableId = 1
        };

        context.Variables.Add(variable);
        context.PollVariables.Add(pollVariable);

        await context.SaveChangesAsync();

        var repository = new VariableRepository(context);

        // Act
        var result = await repository.GetByNameAndPollIdAsync("Variable 1", 100);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Variable 1", result.Name);
    }

    [Fact]
    public async Task GetByNameAndPollIdAsync_ShouldReturnNull_WhenPollDoesNotMatchAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var variable = new VariableEntity
        {
            Id = 1,
            Name = "Variable 1",
            Position = 1
        };

        context.Variables.Add(variable);

        context.PollVariables.Add(new PollVariableJoin
        {
            Id = 10,
            PollId = 100,
            VariableId = 1
        });

        await context.SaveChangesAsync();

        var repository = new VariableRepository(context);

        // Act
        var result = await repository.GetByNameAndPollIdAsync("Variable 1", 999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByNameAndPollIdAsync_ShouldReturnNull_WhenNameDoesNotMatchAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var variable = new VariableEntity
        {
            Id = 1,
            Name = "Variable 1",
            Position = 1
        };

        context.Variables.Add(variable);

        context.PollVariables.Add(new PollVariableJoin
        {
            Id = 10,
            PollId = 100,
            VariableId = 1
        });

        await context.SaveChangesAsync();

        var repository = new VariableRepository(context);

        // Act
        var result = await repository.GetByNameAndPollIdAsync("Unknown", 100);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllWithNameAndPollIdAsync_ShouldReturnVariablesWithPollIdsAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var variable1 = new VariableEntity
        {
            Id = 1,
            Name = "Variable 1",
            Position = 1
        };

        var variable2 = new VariableEntity
        {
            Id = 2,
            Name = "Variable 2",
            Position = 2
        };

        context.Variables.AddRange(variable1, variable2);

        context.PollVariables.AddRange(
            new PollVariableJoin
            {
                Id = 10,
                PollId = 100,
                VariableId = 1
            },
            new PollVariableJoin
            {
                Id = 20,
                PollId = 200,
                VariableId = 2
            });

        await context.SaveChangesAsync();

        var repository = new VariableRepository(context);

        // Act
        var result = await repository.GetAllWithNameAndPollIdAsync();

        // Assert
        Assert.Equal(2, result.Count);

        var result1 = result.Single(V => V.Id == 1);
        var result2 = result.Single(V => V.Id == 2);

        Assert.Equal("Variable 1", result1.Name);
        Assert.Equal(100, result1.IdPoll);

        Assert.Equal("Variable 2", result2.Name);
        Assert.Equal(200, result2.IdPoll);
    }

    [Fact]
    public async Task GetAllWithNameAndPollIdAsync_ShouldReturnEmpty_WhenNoPollVariablesExistAsync()
    {
        // Arrange
        await using var context = CreateContext();

        context.Variables.Add(new VariableEntity
        {
            Id = 1,
            Name = "Variable 1",
            Position = 1
        });

        await context.SaveChangesAsync();

        var repository = new VariableRepository(context);

        // Act
        var result = await repository.GetAllWithNameAndPollIdAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByPollIdAsync_ShouldReturnVariablesForPollAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var variable1 = new VariableEntity
        {
            Id = 1,
            Name = "Variable 1",
            Position = 2
        };

        var variable2 = new VariableEntity
        {
            Id = 2,
            Name = "Variable 2",
            Position = 1
        };

        var variable3 = new VariableEntity
        {
            Id = 3,
            Name = "Variable 3",
            Position = 3
        };

        context.Variables.AddRange(
            variable1,
            variable2,
            variable3);

        context.PollVariables.AddRange(
            new PollVariableJoin
            {
                Id = 10,
                PollId = 100,
                VariableId = 1
            },
            new PollVariableJoin
            {
                Id = 20,
                PollId = 100,
                VariableId = 2
            },
            new PollVariableJoin
            {
                Id = 30,
                PollId = 200,
                VariableId = 3
            });

        await context.SaveChangesAsync();

        var repository = new VariableRepository(context);

        // Act
        var result = await repository.GetByPollIdAsync(100);

        // Assert
        Assert.Equal(2, result.Count);

        Assert.Contains(result, V =>
            V.Id == 1 &&
            V.Name == "Variable 1" &&
            V.Position == 2 &&
            V.IdPoll == 100);

        Assert.Contains(result, V =>
            V.Id == 2 &&
            V.Name == "Variable 2" &&
            V.Position == 1 &&
            V.IdPoll == 100);

        Assert.DoesNotContain(result, V => V.Id == 3);
    }

    [Fact]
    public async Task GetByPollIdAsync_ShouldReturnEmpty_WhenPollHasNoVariablesAsync()
    {
        // Arrange
        await using var context = CreateContext();

        context.Variables.Add(new VariableEntity
        {
            Id = 1,
            Name = "Variable 1",
            Position = 1
        });

        context.PollVariables.Add(new PollVariableJoin
        {
            Id = 10,
            PollId = 200,
            VariableId = 1
        });

        await context.SaveChangesAsync();

        var repository = new VariableRepository(context);

        // Act
        var result = await repository.GetByPollIdAsync(100);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByNameAndPositionAsync_ShouldReturnVariable_WhenNameAndPositionMatchAsync()
    {
        // Arrange
        await using var context = CreateContext();

        context.Variables.Add(new VariableEntity
        {
            Id = 1,
            Name = "Variable 1",
            Position = 5
        });

        await context.SaveChangesAsync();

        var repository = new VariableRepository(context);

        // Act
        var result = await repository.GetByNameAndPositionAsync("Variable 1", 5);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Variable 1", result.Name);
        Assert.Equal(5, result.Position);
    }

    [Fact]
    public async Task GetByNameAndPositionAsync_ShouldReturnNull_WhenPositionDoesNotMatchAsync()
    {
        // Arrange
        await using var context = CreateContext();

        context.Variables.Add(new VariableEntity
        {
            Id = 1,
            Name = "Variable 1",
            Position = 5
        });

        await context.SaveChangesAsync();

        var repository = new VariableRepository(context);

        // Act
        var result = await repository.GetByNameAndPositionAsync("Variable 1", 10);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByNameAndPositionAsync_ShouldReturnNull_WhenNameDoesNotMatchAsync()
    {
        // Arrange
        await using var context = CreateContext();

        context.Variables.Add(new VariableEntity
        {
            Id = 1,
            Name = "Variable 1",
            Position = 5
        });

        await context.SaveChangesAsync();

        var repository = new VariableRepository(context);

        // Act
        var result = await repository.GetByNameAndPositionAsync("Unknown", 5);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByNameAndPositionAsync_ShouldReturnNull_WhenDatabaseIsEmptyAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var repository = new VariableRepository(context);

        // Act
        var result = await repository.GetByNameAndPositionAsync("Variable", 1);

        // Assert
        Assert.Null(result);
    }
}
