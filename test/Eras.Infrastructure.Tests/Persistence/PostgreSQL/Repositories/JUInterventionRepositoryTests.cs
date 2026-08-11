using Eras.Domain.Common;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories;

public class InterventionRepositoryTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static InterventionRepository CreateRepository(AppDbContext Context)
    {
        return new InterventionRepository(Context);
    }

    private static async Task SeedAsync(AppDbContext Context)
    {
        Context.JUInterventions.AddRange(
            new JUInterventionEntity
            {
                Id = 1,
                StudentId = 1,
                Student = new StudentEntity()
                {
                    Email = "stu@mail.com",
                    Name = "Test",
                    Uuid = "123"
                },
                Audit = new AuditInfo()
            },
            new JUInterventionEntity
            {
                Id = 2,
                Student = new StudentEntity()
                {
                    Email = "stu2@mail.com",
                    Name = "Test2",
                    Uuid = "1234"
                },
                StudentId = 2,
                Audit = new AuditInfo()
            },
            new JUInterventionEntity
            {
                Id = 3,
                StudentId = 3,
                Student = new StudentEntity()
                {
                    Email = "stu3@mail.com",
                    Name = "Test3",
                    Uuid = "12345"
                },
                Audit = new AuditInfo()
            }
        );

        await Context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetAllAsync_WhenInterventionsExist_ReturnsAllInterventionsAsync()
    {
        // Arrange
        await using var context = CreateContext();
        await SeedAsync(context);

        var repository = CreateRepository(context);

        // Act
        var result = (await repository.GetAllAsync()).ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);

        Assert.Contains(result, X => X.Id == 1);
        Assert.Contains(result, X => X.Id == 2);
        Assert.Contains(result, X => X.Id == 3);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoInterventionsExist_ReturnsEmptyListAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var repository = CreateRepository(context);

        // Act
        var result = (await repository.GetAllAsync()).ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsRequestedPageAsync()
    {
        // Arrange
        await using var context = CreateContext();
        await SeedAsync(context);
        var repository = CreateRepository(context);

        // Act
        var result = (await repository.GetPagedAsync(0, 2)).ToList();

        // Assert
        Assert.Equal(2, result.Count);

        Assert.Equal(1, result[0].Id);
        Assert.Equal(2, result[1].Id);
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsSecondPageAsync()
    {
        // Arrange
        await using var context = CreateContext();
        await SeedAsync(context);

        var repository = CreateRepository(context);

        // Act
        var result = (await repository.GetPagedAsync(2, 2)).ToList();

        // Assert
        var intervention = Assert.Single(result);

        Assert.Equal(3, intervention.Id);
    }

    [Fact]
    public async Task GetPagedAsync_WhenPageIsOutsideRange_ReturnsEmptyAsync()
    {
        // Arrange
        await using var context = CreateContext();
        await SeedAsync(context);

        var repository = CreateRepository(context);

        // Act
        var result = (await repository.GetPagedAsync(10, 10)).ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetPagedAsync_WhenNoInterventionsExist_ReturnsEmptyAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var repository = CreateRepository(context);

        // Act
        var result = (await repository.GetPagedAsync(1, 10)).ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenInterventionExists_ReturnsInterventionAsync()
    {
        // Arrange
        await using var context = CreateContext();
        await SeedAsync(context);

        var repository = CreateRepository(context);

        // Act
        var result = await repository.GetByIdAsync(2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Id);
        Assert.Equal(2, result.StudentId);
    }

    [Fact]
    public async Task GetByIdAsync_WhenInterventionDoesNotExist_ReturnsNullAsync()
    {
        // Arrange
        await using var context = CreateContext();
        await SeedAsync(context);
        var repository = CreateRepository(context);

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_WhenInterventionExists_UpdatesEntityAsync()
    {
        await using var context = CreateContext();
        await SeedAsync(context);
        var repository = CreateRepository(context);
        var entity = await repository.GetByIdAsync(1);

        Assert.NotNull(entity);

        entity.StudentId = 99;

        var result = await repository.UpdateAsync(entity);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(99, result.StudentId);

        var updatedEntity = await context
            .Set<JUInterventionEntity>()
            .AsNoTracking()
            .SingleAsync(X => X.Id == 1);

        Assert.Equal(99, updatedEntity.StudentId);
    }

    [Fact]
    public async Task UpdateAsync_WhenInterventionDoesNotExist_ReturnsInputWithoutSavingAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var repository = CreateRepository(context);

        var entity = new JUIntervention
        {
            Id = 999,
            StudentId = 100
        };

        // Act
        var result = await repository.UpdateAsync(entity);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(999, result.Id);
        Assert.Equal(100, result.StudentId);

        var databaseEntity = await context
            .Set<JUInterventionEntity>()
            .FirstOrDefaultAsync(X => X.Id == 999);

        Assert.Null(databaseEntity);
    }
}
