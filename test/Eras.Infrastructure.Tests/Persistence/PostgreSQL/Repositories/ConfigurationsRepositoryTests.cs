using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;
using Eras.Infrastructure.Tests.Persistence.PostgreSQL.Utils;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories;

public class ConfigurationsRepositoryTests : RepositoryTestBase
{
    [Fact]
    public async Task GetByIdAsyncNoTracking_WhenConfigurationExists_ReturnsConfigurationAsync()
    {
        await using var context = CreateContext();
        var dto = new Configurations
        {
            Id = 1,
            UserId = "user-1",
            ServiceProviderId = 10,
            ConfigurationName = "Configuration 1",
            BaseURL = "https://example.com",
            EncryptedKey = "encrypted-key",
            IsDeleted = false
        };
        context.Configurations.Add(dto.ToPersistence());
        await context.SaveChangesAsync();
        var repository = new ConfigurationsRepository(context);
        var result = await repository.GetByIdAsyncNoTracking(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("user-1", result.UserId);
        Assert.Equal(10, result.ServiceProviderId);
        Assert.Equal("Configuration 1", result.ConfigurationName);
        Assert.Equal("https://example.com", result.BaseURL);
        Assert.Equal("encrypted-key", result.EncryptedKey);
        Assert.False(result.IsDeleted);
    }

    [Fact]
    public async Task GetByIdAsyncNoTracking_WhenConfigurationDoesNotExist_ThrowsNullReferenceExceptionAsync()
    {
        await using var context = CreateContext();
        var repository = new ConfigurationsRepository(context);
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => repository.GetByIdAsyncNoTracking(999));
    }

    [Fact]
    public async Task GetByNameAsync_WhenConfigurationExists_ReturnsConfigurationAsync()
    {
        await using var context = CreateContext();

        var config = new ConfigurationsEntity
        {
            Id = 1,
            UserId = "user-1",
            ServiceProviderId = 10,
            ConfigurationName = "Configuration 1",
            BaseURL = "https://example.com",
            EncryptedKey = "encrypted-key",
            IsDeleted = false
        };
        context.Configurations.Add(config);
        var dto = config.ToDomain();
        await context.SaveChangesAsync();
        var repository = new ConfigurationsRepository(context);
        var result = await repository.GetByNameAsync("Configuration 1");

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(dto.EncryptedKey, result.EncryptedKey);
        Assert.Equal(dto.ConfigurationName, result.ConfigurationName);
    }

    [Fact]
    public async Task GetByNameAsync_WhenConfigurationDoesNotExist_ReturnsNullAsync()
    {
        await using var context = CreateContext();
        var repository = new ConfigurationsRepository(context);
        var result = await repository.GetByNameAsync("Does Not Exist");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserConfigurationsAsync_ReturnsUserNonDeletedConfigurationsAsync()
    {
        await using var context = CreateContext();

        context.Configurations.AddRange(
            new ConfigurationsEntity
            {
                Id = 1,
                UserId = "user-1",
                ServiceProviderId = 10,
                ConfigurationName = "Configuration 1",
                BaseURL = "https://one.example.com",
                EncryptedKey = "key-1",
                IsDeleted = false
            },
            new ConfigurationsEntity
            {
                Id = 2,
                UserId = "user-1",
                ServiceProviderId = 20,
                ConfigurationName = "Configuration 2",
                BaseURL = "https://two.example.com",
                EncryptedKey = "key-2",
                IsDeleted = false
            });

        await context.SaveChangesAsync();
        var repository = new ConfigurationsRepository(context);
        var result = await repository.GetUserConfigurationsAsync("user-1");

        Assert.Equal(2, result.Count);

        Assert.Contains(result, x => x.Id == 1);
        Assert.Contains(result, x => x.Id == 2);
    }

    [Fact]
    public async Task GetUserConfigurationsAsync_DoesNotReturnDeletedConfigurationsAsync()
    {
        await using var context = CreateContext();

        context.Configurations.AddRange(
            new ConfigurationsEntity
            {
                Id = 1,
                UserId = "user-1",
                ServiceProviderId = 10,
                ConfigurationName = "Active",
                IsDeleted = false,
                BaseURL = "",
                EncryptedKey = ""
            },
            new ConfigurationsEntity
            {
                Id = 2,
                UserId = "user-1",
                ServiceProviderId = 20,
                ConfigurationName = "Deleted",
                IsDeleted = true,
                BaseURL = "",
                EncryptedKey = ""
            });

        await context.SaveChangesAsync();

        var repository = new ConfigurationsRepository(context);
        var result = await repository.GetUserConfigurationsAsync("user-1");

        Assert.Single(result);
        Assert.Equal(1, result[0].Id);
        Assert.Equal("Active", result[0].ConfigurationName);
    }

    [Fact]
    public async Task GetUserConfigurationsAsync_DoesNotReturnOtherUsersConfigurationsAsync()
    {
        await using var context = CreateContext();

        context.Configurations.AddRange(
            new ConfigurationsEntity
            {
                Id = 1,
                UserId = "user-1",
                ConfigurationName = "User 1 Configuration",
                IsDeleted = false,
                BaseURL = "",
                EncryptedKey = ""
            },
            new ConfigurationsEntity
            {
                Id = 2,
                UserId = "user-2",
                ConfigurationName = "User 2 Configuration",
                IsDeleted = false,
                BaseURL = "",
                EncryptedKey = ""
            });

        await context.SaveChangesAsync();
        var repository = new ConfigurationsRepository(context);
        var result = await repository.GetUserConfigurationsAsync("user-1");

        Assert.Single(result);
        Assert.Equal(1, result[0].Id);
        Assert.Equal("user-1", result[0].UserId);
    }

    [Fact]
    public async Task GetUserConfigurationsAsync_WhenNoConfigurationsExist_ReturnsEmptyListAsync()
    {
        await using var context = CreateContext();
        var repository = new ConfigurationsRepository(context);
        var result = await repository.GetUserConfigurationsAsync("user-1");

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task UpdateDeleteStatus_WhenConfigurationExists_SetsIsDeletedToTrueAsync()
    {
        await using var context = CreateContext();
        context.Configurations.Add(new ConfigurationsEntity
        {
            Id = 1,
            UserId = "user-1",
            ConfigurationName = "Configuration 1",
            IsDeleted = false,
            BaseURL = "",
            EncryptedKey = ""
        });
        await context.SaveChangesAsync();
        var repository = new ConfigurationsRepository(context);
        var result = await repository.UpdateDeleteStatus(1);

        Assert.NotNull(result);
        Assert.True(result.IsDeleted);
        Assert.Equal(1, result.Id);

        var entity = await context.Configurations.FirstAsync(x => x.Id == 1);

        Assert.True(entity.IsDeleted);
    }

    [Fact]
    public async Task UpdateDeleteStatus_WhenConfigurationDoesNotExist_ThrowsNullReferenceExceptionAsync()
    {
        await using var context = CreateContext();
        var repository = new ConfigurationsRepository(context);
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => repository.UpdateDeleteStatus(999));
    }
}
