using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Mappers;
using Eras.Domain.Entities;
using Eras.Error.Critical;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;
using Eras.Infrastructure.Tests.Persistence.PostgreSQL.Utils;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

using MockQueryable.Moq;

using Moq;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories;

public class TestPersistEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

public class TestDomainEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}
public class BaseRepositoryTest : RepositoryTestBase
{
    private readonly Mock<AppDbContext> _mockContext;
    private readonly BaseRepository<TestDomainEntity, TestPersistEntity> _repository;

    public BaseRepositoryTest()
    {
        _mockContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
        _repository = new BaseRepository<TestDomainEntity, TestPersistEntity>(
            _mockContext.Object,
            P => new TestDomainEntity { Id = P.Id, Name = P.Name },
            D => new TestPersistEntity { Id = D.Id, Name = D.Name });
    }

    private (Mock<DatabaseFacade> Database, Mock<IDbContextTransaction> Transaction)
        SetupTransaction(bool AmbientTransaction)
    {
        var database = new Mock<DatabaseFacade>(_mockContext.Object);

        var transaction = new Mock<IDbContextTransaction>();

        _mockContext
            .SetupGet(C => C.Database)
            .Returns(database.Object);

        if (AmbientTransaction)
        {
            database
                .Setup(D => D.CurrentTransaction)
                .Returns(transaction.Object);
        }
        else
        {
            database
                .Setup(D => D.CurrentTransaction)
                .Returns((IDbContextTransaction?)null);

            database
                .Setup(D => D.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(transaction.Object);
        }

        return (database, transaction);
    }

    private void SetupEntities(params TestPersistEntity[] Entities)
    {
        var mockSet = Entities.AsQueryable().BuildMockDbSet();
        _mockContext.Setup(C => C.Set<TestPersistEntity>()).Returns(mockSet.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsMappedDomainEntitiesAsync()
    {
        SetupEntities(
            new TestPersistEntity { Id = 1, Name = "A" },
            new TestPersistEntity { Id = 2, Name = "B" });

        var result = await _repository.GetAllAsync();

        Assert.Equal(2, result.Count());
        Assert.Contains(result, R => R.Id == 1 && R.Name == "A");
    }

    [Fact]
    public async Task GetPagedAsync_SkipsAndTakesAsync()
    {
        SetupEntities(Enumerable.Range(1, 5)
            .Select(I => new TestPersistEntity { Id = I, Name = $"E{I}" })
            .ToArray());

        var result = await _repository.GetPagedAsync(2, 2);

        Assert.Equal(new[] { 3, 4 }, result.Select(R => R.Id));
    }

    [Fact]
    public async Task CountAsync_ReturnsTotalCountAsync()
    {
        SetupEntities(new TestPersistEntity { Id = 1 }, new TestPersistEntity { Id = 2 });

        Assert.Equal(2, await _repository.CountAsync());
    }

    [Fact]
    public async Task CountAsync_WithPredicate_TranslatesExpressionAgainstPersistTypeAsync()
    {
        SetupEntities(
            new TestPersistEntity { Id = 1, Name = "keep" },
            new TestPersistEntity { Id = 2, Name = "drop" });

        var count = await _repository.CountAsync(D => D.Name == "keep");

        Assert.Equal(1, count);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFoundAsync()
    {
        var mockSet = new Mock<DbSet<TestPersistEntity>>();
        mockSet.Setup(S => S.FindAsync(1)).ReturnsAsync((TestPersistEntity?)null);
        _mockContext.Setup(C => C.Set<TestPersistEntity>()).Returns(mockSet.Object);

        var result = await _repository.GetByIdAsync(1);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsMappedEntity_WhenFoundAsync()
    {
        var entity = new TestPersistEntity { Id = 1, Name = "A" };
        var mockSet = new Mock<DbSet<TestPersistEntity>>();
        mockSet.Setup(S => S.FindAsync(1)).ReturnsAsync(entity);
        _mockContext.Setup(C => C.Set<TestPersistEntity>()).Returns(mockSet.Object);

        var result = await _repository.GetByIdAsync(1);

        Assert.Equal("A", result!.Name);
    }

    [Fact]
    public async Task UpdateAsync_CallsUpdate_SaveChangesAsync()
    {
        var mockSet = new Mock<DbSet<TestPersistEntity>>();
        _mockContext.Setup(C => C.Set<TestPersistEntity>()).Returns(mockSet.Object);
        _mockContext.Setup(C => C.SaveChangesAsync(default)).ReturnsAsync(1);

        var result = await _repository.UpdateAsync(new TestDomainEntity { Id = 1, Name = "X" });

        mockSet.Verify(S => S.Update(It.Is<TestPersistEntity>(E => E.Id == 1)), Times.Once);
        Assert.Equal("X", result.Name);
    }

    [Fact]
    public async Task AddAsync_AddsEntity_SavesAndReturnsMappedEntityAsync()
    {
        var mockSet = new Mock<DbSet<TestPersistEntity>>();

        var persistedEntity = new TestPersistEntity
        {
            Id = 1,
            Name = "A"
        };

        mockSet
            .Setup(S => S.AddAsync(
                It.Is<TestPersistEntity>(E => E.Name == "A"),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new EntityEntry<TestPersistEntity>(
                    null!));

        var context = new Mock<AppDbContext>(
            new DbContextOptions<AppDbContext>());

        var entity = new TestPersistEntity { Id = 1, Name = "A" };

        mockSet
            .Setup(S => S.AddAsync(
                It.IsAny<TestPersistEntity>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((TestPersistEntity E, CancellationToken _) =>
            {
                return null!;
            });
    }
    
    [Fact]
    public async Task AddBatchAsync_WithoutAmbientTransaction_BeginsCommitsAndDisposesTransactionAsync()
    {
        var mockSet = new Mock<DbSet<TestPersistEntity>>();

        _mockContext
            .Setup(C => C.Set<TestPersistEntity>())
            .Returns(mockSet.Object);

        _mockContext
            .Setup(C => C.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        var (database, transaction) = SetupTransaction(AmbientTransaction: false);

        await _repository.AddBatchAsync(
            new[]
            {
            new TestDomainEntity { Id = 1, Name = "A" },
            new TestDomainEntity { Id = 2, Name = "B" }
            });

        database.Verify(
            D => D.BeginTransactionAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        mockSet.Verify(
            S => S.AddRange(It.Is<IEnumerable<TestPersistEntity>>(
                E => E.Count() == 2)),
            Times.Once);

        transaction.Verify(T => T.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        transaction.Verify(T => T.DisposeAsync(), Times.Once);
    }

    [Fact]
    public async Task AddBatchAsync_WithAmbientTransaction_DoesNotBeginCommitOrDisposeTransactionAsync()
    {
        var mockSet = new Mock<DbSet<TestPersistEntity>>();

        _mockContext
            .Setup(C => C.Set<TestPersistEntity>())
            .Returns(mockSet.Object);

        _mockContext
            .Setup(C => C.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        var (database, transaction) = SetupTransaction(AmbientTransaction: true);

        await _repository.AddBatchAsync(
            new[]
            {
            new TestDomainEntity { Id = 1, Name = "A" },
            new TestDomainEntity { Id = 2, Name = "B" }
            });

        database.Verify(
            D => D.BeginTransactionAsync(It.IsAny<CancellationToken>()),
            Times.Never);

        transaction.Verify(T => T.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        transaction.Verify(T => T.DisposeAsync(), Times.Never);
    }

    [Fact]
    public async Task AddTrackedBatchAsync_ReturnsMappedEntitiesAsync()
    {
        var mockSet = new Mock<DbSet<TestPersistEntity>>();

        _mockContext
            .Setup(C => C.Set<TestPersistEntity>())
            .Returns(mockSet.Object);

        _mockContext
            .Setup(C => C.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        var (database, transaction) = SetupTransaction(false);

        var result = await _repository.AddTrackedBatchAsync(
            new[]
            {
            new TestDomainEntity { Id = 1, Name = "A" },
            new TestDomainEntity { Id = 2, Name = "B" }
            });

        Assert.Equal(2, result.Count());
        Assert.Equal(
            new[] { 1, 2 },
            result.Select(E => E.Id));

        Assert.Equal(
            new[] { "A", "B" },
            result.Select(E => E.Name));

        transaction.Verify(
            T => T.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        transaction.Verify(
            T => T.DisposeAsync(),
            Times.Once);
    }

    [Fact]
    public async Task AddTrackedBatchAsync_WithAmbientTransaction_DoesNotCreateTransactionAsync()
    {
        var mockSet = new Mock<DbSet<TestPersistEntity>>();

        _mockContext
            .Setup(C => C.Set<TestPersistEntity>())
            .Returns(mockSet.Object);

        _mockContext
            .Setup(C => C.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        var (database, transaction) = SetupTransaction(true);

        var result = await _repository.AddTrackedBatchAsync(
            new[]
            {
            new TestDomainEntity { Id = 1, Name = "A" }
            });

        Assert.Single(result);

        database.Verify(
            D => D.BeginTransactionAsync(It.IsAny<CancellationToken>()),
            Times.Never);

        transaction.Verify(
            T => T.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Never);

        transaction.Verify(
            T => T.DisposeAsync(),
            Times.Never);
    }

    //[Fact]
    //public async Task AddTrackedBatchAsync_WhenSaveFails_RollsBackAndThrowsAsync()
    //{
    //    var mockSet = new Mock<DbSet<TestPersistEntity>>();

    //    _mockContext
    //        .Setup(C => C.Set<TestPersistEntity>())
    //        .Returns(mockSet.Object);

    //    _mockContext
    //        .Setup(C => C.SaveChangesAsync(It.IsAny<CancellationToken>()))
    //        .ThrowsAsync(new InvalidOperationException("DB error"));

    //    var (_, transaction) = SetupTransaction(false);

    //    var exception = await Assert.ThrowsAsync<DatabaseCustomException>(
    //        () => _repository.AddTrackedBatchAsync(
    //            new[]
    //            {
    //            new TestDomainEntity { Id = 1, Name = "A" }
    //            }));

    //    Assert.IsType<InvalidOperationException>(exception.InnerException);

    //    transaction.Verify(
    //        T => T.RollbackAsync(It.IsAny<CancellationToken>()),
    //        Times.Once);

    //    transaction.Verify(
    //        T => T.DisposeAsync(),
    //        Times.Once);
    //}

    [Fact]
    public async Task DeleteAsync_RemovesEntityAndSavesChangesAsync()
    {
        var mockSet = new Mock<DbSet<TestPersistEntity>>();

        _mockContext
            .Setup(C => C.Set<TestPersistEntity>())
            .Returns(mockSet.Object);

        _mockContext
            .Setup(C => C.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _repository.DeleteAsync(
            new TestDomainEntity
            {
                Id = 1,
                Name = "A"
            });

        mockSet.Verify(
            S => S.Remove(It.Is<TestPersistEntity>(
                E => E.Id == 1 && E.Name == "A")),
            Times.Once);

        _mockContext.Verify(
            C => C.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenUpdateFails_ThrowsDatabaseCustomExceptionAsync()
    {
        var mockSet = new Mock<DbSet<TestPersistEntity>>();

        _mockContext
            .Setup(C => C.Set<TestPersistEntity>())
            .Returns(mockSet.Object);

        mockSet
            .Setup(S => S.Update(It.IsAny<TestPersistEntity>()))
            .Throws(new InvalidOperationException("DB error"));

        var exception = await Assert.ThrowsAsync<DatabaseCustomException>(
            () => _repository.UpdateAsync(
                new TestDomainEntity
                {
                    Id = 1,
                    Name = "A"
                }));

        Assert.IsType<InvalidOperationException>(exception.InnerException);
    }
}
