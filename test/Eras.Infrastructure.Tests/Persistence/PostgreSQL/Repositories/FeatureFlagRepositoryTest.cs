using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities.FeatureFlagManagement;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;

using Microsoft.EntityFrameworkCore;

using MockQueryable.Moq;

using Moq;

using Xunit;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories;

public class FeatureFlagRepositoryTest
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetByNameAsync_WhenFeatureFlagExists_ReturnsFeatureFlagAsync()
    {
        // Arrange
        await using var context = CreateContext();

        context.FeatureFlags.AddRange(
            new FeatureFlag
            {
                Id = 1,
                Name = "FeatureA"
            },
            new FeatureFlag
            {
                Id = 2,
                Name = "FeatureB"
            });
        await context.SaveChangesAsync();
        var repository = new FeatureFlagRepository(context);

        // Act
        var result = await repository.GetByNameAsync("FeatureA");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("FeatureA", result.Name);
    }

    [Fact]
    public async Task GetByNameAsync_WhenFeatureFlagDoesNotExist_ReturnsNullAsync()
    {
        // Arrange
        await using var context = CreateContext();

        context.FeatureFlags.Add(
            new FeatureFlag
            {
                Id = 1,
                Name = "FeatureA"
            });

        await context.SaveChangesAsync();
        var repository = new FeatureFlagRepository(context);

        // Act
        var result = await repository.GetByNameAsync("DoesNotExist");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdNoTrackingAsync_WhenFeatureFlagExists_ReturnsFeatureFlagAsync()
    {
        // Arrange
        await using var context = CreateContext();

        context.FeatureFlags.AddRange(
            new FeatureFlag
            {
                Id = 1,
                Name = "FeatureA"
            },
            new FeatureFlag
            {
                Id = 2,
                Name = "FeatureB"
            });

        await context.SaveChangesAsync();
        var repository = new FeatureFlagRepository(context);

        // Act
        var result = await repository.GetByIdNoTrackingAsync(2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Id);
        Assert.Equal("FeatureB", result.Name);
    }

    [Fact]
    public async Task GetByIdNoTrackingAsync_WhenFeatureFlagDoesNotExist_ReturnsNullAsync()
    {
        // Arrange
        await using var context = CreateContext();

        context.FeatureFlags.Add(
            new FeatureFlag
            {
                Id = 1,
                Name = "FeatureA"
            });

        await context.SaveChangesAsync();
        var repository = new FeatureFlagRepository(context);

        // Act
        var result = await repository.GetByIdNoTrackingAsync(999);

        // Assert
        Assert.Null(result);
    }
}
