using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;
using Eras.Infrastructure.Tests.Persistence.PostgreSQL.Utils;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories;

public class ServiceProvidersRepositoryTest : RepositoryTestBase
{

    [Fact]
    public async Task GetByNameAsync_ShouldReturnServiceProvider_WhenExistsAsync()
    {
        // Arrange
        await using var context = CreateContext();
        var dto = new ServiceProviders
        {
            Id = 1,
            ServiceProviderName = "Test Provider",
            ServiceProviderLogo = "logo"
        };
        context.ServiceProviders.Add(dto.ToPersistence());

        await context.SaveChangesAsync();

        var repository = new ServiceProvidersRepository(context);

        // Act
        var result = await repository.GetByNameAsync("Test Provider");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Provider", result.ServiceProviderName);
    }

    [Fact]
    public async Task GetByNameAsync_ShouldReturnNull_WhenServiceProviderDoesNotExistAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var service = new ServiceProvidersEntity
        {
            Id = 1,
            ServiceProviderName = "Test Provider",
            ServiceProviderLogo = "logo"
        };

        context.ServiceProviders.Add(service);
        service.ToDomain();
        await context.SaveChangesAsync();

        var repository = new ServiceProvidersRepository(context);

        // Act
        var result = await repository.GetByNameAsync("Unknown Provider");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByNameAsync_ShouldReturnFirstMatchingServiceProvider_WhenMultipleExistAsync()
    {
        // Arrange
        await using var context = CreateContext();

        context.ServiceProviders.AddRange(
            new ServiceProvidersEntity
            {
                Id = 1,
                ServiceProviderName = "Provider A",
                ServiceProviderLogo = "logo"
            },
            new ServiceProvidersEntity
            {
                Id = 2,
                ServiceProviderName = "Provider B",
                ServiceProviderLogo = "logo"
            },
            new ServiceProvidersEntity
            {
                Id = 3,
                ServiceProviderName = "Provider A",
                ServiceProviderLogo = "logo"
            });

        await context.SaveChangesAsync();

        var repository = new ServiceProvidersRepository(context);

        // Act
        var result = await repository.GetByNameAsync("Provider A");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Provider A", result.ServiceProviderName);
    }

    [Fact]
    public async Task GetByNameAsync_ShouldReturnNull_WhenDatabaseIsEmptyAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var repository = new ServiceProvidersRepository(context);

        // Act
        var result = await repository.GetByNameAsync("Test Provider");

        // Assert
        Assert.Null(result);
    }
}
