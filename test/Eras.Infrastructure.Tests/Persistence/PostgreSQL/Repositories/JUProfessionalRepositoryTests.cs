using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;
using Eras.Infrastructure.Tests.Persistence.PostgreSQL.Utils;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories;


public class ProfessionalRepositoryTests : RepositoryTestBase
{

    private static ProfessionalRepository CreateRepository(AppDbContext Context)
    {
        return new ProfessionalRepository(Context);
    }

    private static async Task SeedAsync(AppDbContext Context)
    {
        Context.Professionals.AddRange(
            new JUProfessionalEntity
            {
                Id = 1
            },
            new JUProfessionalEntity
            {
                Id = 2
            },
            new JUProfessionalEntity
            {
                Id = 3
            }
        );

        await Context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetAllAsync_WhenProfessionalsExist_ReturnsAllProfessionalsAsync()
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
    public async Task GetAllAsync_WhenNoProfessionalsExist_ReturnsEmptyListAsync()
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
    public async Task GetByIdAsync_WhenProfessionalExists_ReturnsProfessionalAsync()
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
    }

    [Fact]
    public async Task GetByIdAsync_WhenProfessionalDoesNotExist_ReturnsNullAsync()
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
    public async Task UpdateAsync_WhenProfessionalExists_UpdatesEntityAsync()
    {
        // Arrange
        await using var context = CreateContext();
        await SeedAsync(context);

        var repository = CreateRepository(context);

        var entity = await repository.GetByIdAsync(1);

        Assert.NotNull(entity);
        entity.Name = "Updated Professional";

        // Act
        var result = await repository.UpdateAsync(entity);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);

        var updatedEntity = await context
            .Set<JUProfessionalEntity>()
            .AsNoTracking()
            .SingleAsync(X => X.Id == 1);

        Assert.Equal(1, updatedEntity.Id);
        Assert.Equal("Updated Professional", updatedEntity.Name);
    }

    [Fact]
    public async Task UpdateAsync_WhenProfessionalDoesNotExist_ReturnsInputWithoutCreatingEntityAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var repository = CreateRepository(context);

        var entity = new JUProfessional
        {
            Id = 999
        };

        // Act
        var result = await repository.UpdateAsync(entity);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(999, result.Id);

        var databaseEntity = await context
            .Set<JUProfessionalEntity>()
            .FirstOrDefaultAsync(X => X.Id == 999);

        Assert.Null(databaseEntity);
    }
}
