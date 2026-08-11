using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.Mappers;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;
using Microsoft.EntityFrameworkCore;
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
public class BaseRepositoryTest
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
}
