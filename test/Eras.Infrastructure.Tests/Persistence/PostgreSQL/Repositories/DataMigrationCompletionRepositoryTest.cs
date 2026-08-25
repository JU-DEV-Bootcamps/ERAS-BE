using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories;

public class DataMigrationCompletionRepositoryTest
{
    private static DataMigrationCompletionRepository CreateRepository(out AppDbContext context)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        context = new AppDbContext(options);
        return new DataMigrationCompletionRepository(context);
    }

    [Fact]
    public async Task IsCompletedAsync_Should_ReturnFalse_When_NeverMarkedCompletedAsync()
    {
        // Arrange
        var repository = CreateRepository(out _);

        // Act
        bool result = await repository.IsCompletedAsync("some-migration");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task MarkCompletedAsync_Then_IsCompletedAsync_Should_ReturnTrueAsync()
    {
        // Arrange
        var repository = CreateRepository(out _);

        // Act
        await repository.MarkCompletedAsync("some-migration");

        // Assert
        Assert.True(await repository.IsCompletedAsync("some-migration"));
    }

    [Fact]
    public async Task IsCompletedAsync_Should_BeScopedToTheGivenName_NotAnyMigrationAsync()
    {
        // Arrange
        var repository = CreateRepository(out _);
        await repository.MarkCompletedAsync("migration-a");

        // Act & Assert — a different name is still reported as not completed
        Assert.False(await repository.IsCompletedAsync("migration-b"));
    }

    [Fact]
    public async Task MarkCompletedAsync_Should_BeIdempotent_When_CalledTwiceForTheSameNameAsync()
    {
        // Arrange
        var repository = CreateRepository(out _);
        await repository.MarkCompletedAsync("some-migration");

        // Act — calling it again must not throw (e.g. from a unique-index violation) or duplicate
        await repository.MarkCompletedAsync("some-migration");

        // Assert
        Assert.True(await repository.IsCompletedAsync("some-migration"));
    }
}
