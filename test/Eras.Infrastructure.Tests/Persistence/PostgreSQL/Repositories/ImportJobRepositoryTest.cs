using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories;

public class ImportJobRepositoryTest
{
    private static AppDbContext BuildContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetLatestImportJobIdsByEvaluationIdsAsync_ShouldReturnLatestJobPerEvaluationAsync()
    {
        using var context = BuildContext();

        var baseTime = DateTime.UtcNow;

        context.ImportJobs.AddRange(
            new ImportJobEntity
            {
                Id = 1,
                EvaluationId = 10,
                CreatedAtUtc = baseTime.AddMinutes(-30)
            },
            new ImportJobEntity
            {
                Id = 2,
                EvaluationId = 10,
                CreatedAtUtc = baseTime.AddMinutes(-10)
            },
            new ImportJobEntity
            {
                Id = 3,
                EvaluationId = 10,
                CreatedAtUtc = baseTime.AddMinutes(-20)
            },
            new ImportJobEntity
            {
                Id = 4,
                EvaluationId = 20,
                CreatedAtUtc = baseTime.AddMinutes(-30)
            },
            new ImportJobEntity
            {
                Id = 5,
                EvaluationId = 20,
                CreatedAtUtc = baseTime.AddMinutes(-5)
            },
            new ImportJobEntity
            {
                Id = 6,
                EvaluationId = 30,
                CreatedAtUtc = baseTime.AddMinutes(-1)
            }
        );

        await context.SaveChangesAsync();

        var repository = new ImportJobRepository(context);

        var result = await repository.GetLatestImportJobIdsByEvaluationIdsAsync(
            new[] { 10, 20, 30 });

        Assert.Equal(3, result.Count);

        Assert.Equal(2, result[10]);
        Assert.Equal(5, result[20]);
        Assert.Equal(6, result[30]);
    }

    [Fact]
    public async Task GetLatestImportJobIdsByEvaluationIdsAsync_ShouldIgnoreOtherEvaluationsAsync()
    {
        using var context = BuildContext();

        var baseTime = DateTime.UtcNow;

        context.ImportJobs.AddRange(
            new ImportJobEntity
            {
                Id = 1,
                EvaluationId = 10,
                CreatedAtUtc = baseTime
            },
            new ImportJobEntity
            {
                Id = 2,
                EvaluationId = 20,
                CreatedAtUtc = baseTime.AddMinutes(1)
            });

        await context.SaveChangesAsync();

        var repository = new ImportJobRepository(context);

        var result = await repository.GetLatestImportJobIdsByEvaluationIdsAsync(
            new[] { 10 });

        Assert.Single(result);
        Assert.Equal(1, result[10]);
        Assert.DoesNotContain(20, result.Keys);
    }

    [Fact]
    public async Task GetLatestImportJobIdsByEvaluationIdsAsync_ShouldReturnEmpty_WhenNoEvaluationMatchesAsync()
    {
        using var context = BuildContext();

        context.ImportJobs.Add(new ImportJobEntity
        {
            Id = 1,
            EvaluationId = 10,
            CreatedAtUtc = DateTime.UtcNow
        });

        await context.SaveChangesAsync();

        var repository = new ImportJobRepository(context);

        var result = await repository.GetLatestImportJobIdsByEvaluationIdsAsync(
            new[] { 999 });

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetLatestImportJobIdsByEvaluationIdsAsync_ShouldReturnEmpty_WhenInputIsEmptyAsync()
    {
        using var context = BuildContext();

        context.ImportJobs.Add(new ImportJobEntity
        {
            Id = 1,
            EvaluationId = 10,
            CreatedAtUtc = DateTime.UtcNow
        });

        await context.SaveChangesAsync();

        var repository = new ImportJobRepository(context);

        var result = await repository.GetLatestImportJobIdsByEvaluationIdsAsync(
            Array.Empty<int>());

        Assert.Empty(result);
    }
}