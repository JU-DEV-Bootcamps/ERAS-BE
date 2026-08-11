using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories;

public class ImportJobItemRepositoryTests
{
    private readonly AppDbContext _context;
    private readonly ImportJobItemRepository _repository;

    public ImportJobItemRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new ImportJobItemRepository(_context);

        SeedData();
    }

    private void SeedData()
    {
        var items = new List<ImportJobItemEntity>
        {
            new ImportJobItemEntity
            {
                Id = 1,
                ImportJobId = 100,
                Status = ImportJobStatus.Extracted,
                ErrorMessage = null,
                RetryCount = 0,
                UpdatedAtUtc = DateTime.UtcNow.AddMinutes(-5)
            },
            new ImportJobItemEntity
            {
                Id = 2,
                ImportJobId = 100,
                Status = ImportJobStatus.Queued,
                ErrorMessage = null,
                RetryCount = 0,
                UpdatedAtUtc = DateTime.UtcNow.AddMinutes(-4)
            },
            new ImportJobItemEntity
            {
                Id = 3,
                ImportJobId = 100,
                Status = ImportJobStatus.Running,
                ErrorMessage = null,
                RetryCount = 0,
                UpdatedAtUtc = DateTime.UtcNow.AddMinutes(-3)
            },
            new ImportJobItemEntity
            {
                Id = 4,
                ImportJobId = 100,
                Status = ImportJobStatus.Completed,
                ErrorMessage = null,
                RetryCount = 0,
                UpdatedAtUtc = DateTime.UtcNow.AddMinutes(-2)
            },
            new ImportJobItemEntity
            {
                Id = 5,
                ImportJobId = 100,
                Status = ImportJobStatus.Failed,
                ErrorMessage = "Import failed",
                RetryCount = 1,
                UpdatedAtUtc = DateTime.UtcNow.AddMinutes(-1)
            },

            new ImportJobItemEntity
            {
                Id = 6,
                ImportJobId = 200,
                Status = ImportJobStatus.Extracted,
                ErrorMessage = null,
                RetryCount = 0,
                UpdatedAtUtc = DateTime.UtcNow
            },

            new ImportJobItemEntity
            {
                Id = 7,
                ImportJobId = 100,
                Status = ImportJobStatus.Failed,
                ErrorMessage = "Another failure",
                RetryCount = 3,
                UpdatedAtUtc = DateTime.UtcNow.AddMinutes(-10)
            }
        };

        _context.Set<ImportJobItemEntity>().AddRange(items);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetByJobIdAsyncShouldReturnItemsForJobAsync()
    {
        // Act
        var result = await _repository.GetByJobIdAsync(100);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(6, result.Count);

        Assert.All(result, Item => Assert.Equal(100, Item.ImportJobId));
    }

    [Fact]
    public async Task GetByJobIdAsyncShouldOrderItemsByIdAsync()
    {
        // Act
        var result = await _repository.GetByJobIdAsync(100);

        // Assert
        Assert.Equal(new[] { 1, 2, 3, 4, 5, 7 }, result.Select(Item => Item.Id));
    }

    [Fact]
    public async Task GetByJobIdAsyncShouldReturnEmptyWhenJobDoesNotExistAsync()
    {
        // Act
        var result = await _repository.GetByJobIdAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByJobIdAndStatusAsyncShouldReturnItemsWithRequestedStatusAsync()
    {
        // Act
        var result = await _repository.GetByJobIdAndStatusAsync(100, ImportJobStatus.Failed);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(new[] { 5, 7 }, result.Select(Item => Item.Id));

        Assert.All(result, Item => Assert.Equal(ImportJobStatus.Failed, Item.Status));
    }

    [Fact]
    public async Task GetByJobIdAndStatusAsyncShouldReturnEmptyWhenStatusDoesNotMatchAsync()
    {
        // Act
        var result = await _repository.GetByJobIdAndStatusAsync(100, ImportJobStatus.Skipped);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByJobIdAndStatusAsyncShouldNotReturnItemsFromAnotherJobAsync()
    {
        // Act
        var result = await _repository.GetByJobIdAndStatusAsync(200, ImportJobStatus.Extracted);

        // Assert
        var item = Assert.Single(result);
        Assert.Equal(6, item.Id);
        Assert.Equal(200, item.ImportJobId);
        Assert.Equal(ImportJobStatus.Extracted, item.Status);
    }

    [Fact]
    public async Task GetByIdsAsyncShouldReturnMatchingItemsAsync()
    {
        // Act
        var result = await _repository.GetByIdsAsync(100, new List<int> { 1, 3, 5 });

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal(new[] { 1, 3, 5 }, result.Select(Item => Item.Id).OrderBy(Id => Id));
    }

    [Fact]
    public async Task GetByIdsAsyncShouldOnlyReturnItemsFromRequestedJobAsync()
    {
        // Act
        var result = await _repository.GetByIdsAsync(100, new List<int> { 1, 6 });

        // Assert
        var item = Assert.Single(result);
        Assert.Equal(1, item.Id);
        Assert.Equal(100, item.ImportJobId);
    }

    [Fact]
    public async Task GetByIdsAsyncShouldReturnEmptyWhenIdsAreEmptyAsync()
    {
        // Act
        var result = await _repository.GetByIdsAsync(100, new List<int>());

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdsAsyncShouldReturnEmptyWhenNoIdsMatchAsync()
    {
        // Act
        var result = await _repository.GetByIdsAsync(100, new List<int> { 999, 1000 });

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetImportPhaseCountsAsyncShouldReturnCorrectCountsAsync()
    {
        // Act
        var result = await _repository.GetImportPhaseCountsAsync(100);

        // Assert
        Assert.Equal(2, result.Pending);
        Assert.Equal(1, result.Completed);
        Assert.Equal(2, result.Failed);
    }

    [Fact]
    public async Task GetImportPhaseCountsAsyncShouldReturnZerosWhenJobDoesNotExistAsync()
    {
        // Act
        var result = await _repository.GetImportPhaseCountsAsync(999);

        // Assert
        Assert.Equal(0, result.Pending);
        Assert.Equal(0, result.Completed);
        Assert.Equal(0, result.Failed);
    }

    [Fact]
    public async Task GetImportPhaseCountsAsyncShouldNotCountItemsFromAnotherJobAsync()
    {
        // Act
        var result = await _repository.GetImportPhaseCountsAsync(200);

        // Assert
        Assert.Equal(0, result.Pending);
        Assert.Equal(0, result.Completed);
        Assert.Equal(0, result.Failed);
    }
}
