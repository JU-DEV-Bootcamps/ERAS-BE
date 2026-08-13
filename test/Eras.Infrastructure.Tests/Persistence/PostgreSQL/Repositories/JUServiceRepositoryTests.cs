using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;
using Eras.Infrastructure.Tests.Persistence.PostgreSQL.Utils;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories;


public class JUServiceRepositoryTests : RepositoryTestBase
{

    private static JUServiceRepository CreateRepository(AppDbContext Context)
    {
        return new JUServiceRepository(Context);
    }

    private static async Task SeedAsync(AppDbContext Context)
    {
        Context.JUServices.AddRange(
            new JUServiceEntity
            {
                Id = 1,
                Name = "Workshop"
            },
            new JUServiceEntity
            {
                Id = 2,
                Name = "Game"
            }
        );

        await Context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetByIdAsync_WhenServiceExists_ReturnsServiceAsync()
    {
        // Arrange
        await using var context = CreateContext();
        await SeedAsync(context);

        var repository = CreateRepository(context);

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenServiceDoesNotExist_ReturnsNullAsync()
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
    public async Task UpdateAsync_WhenServiceExists_UpdatesEntityAsync()
    {
        // Arrange
        await using var context = CreateContext();
        await SeedAsync(context);

        var repository = CreateRepository(context);

        var entity = await repository.GetByIdAsync(1);

        Assert.NotNull(entity);
        entity.Name = "Updated Service";

        // Act
        var result = await repository.UpdateAsync(entity);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);

        var updatedEntity = await context
            .Set<JUServiceEntity>()
            .AsNoTracking()
            .SingleAsync(X => X.Id == 1);

        Assert.Equal(1, updatedEntity.Id);
        Assert.Equal("Updated Service", updatedEntity.Name);
    }

    [Fact]
    public async Task UpdateAsync_WhenServiceDoesNotExist_ReturnsInputWithoutCreatingEntityAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var repository = CreateRepository(context);

        var entity = new JUService
        {
            Id = 999
        };

        // Act
        var result = await repository.UpdateAsync(entity);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(999, result.Id);

        var databaseEntity = await context
            .Set<JUServiceEntity>()
            .FirstOrDefaultAsync(X => X.Id == 999);

        Assert.Null(databaseEntity);
    }
}
