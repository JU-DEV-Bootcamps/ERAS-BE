

using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories.AssessmentManagement;
using Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories;

using Microsoft.EntityFrameworkCore;

using MockQueryable.Moq;

using Moq;

using Xunit;

public class BaseRepositorySingleGenericTest
{
    private readonly Mock<AppDbContext> _mockContext;
    private readonly BaseRepository<TestPersistEntity> _repository;

    public BaseRepositorySingleGenericTest()
    {
        _mockContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
        _repository = new BaseRepository<TestPersistEntity>(_mockContext.Object);
    }

    [Fact]
    public async Task GetByIdAsync_Int_ReturnsEntity_WhenFoundAsync()
    {
        var entity = new TestPersistEntity { Id = 1, Name = "A" };
        var mockSet = new Mock<DbSet<TestPersistEntity>>();
        mockSet.Setup(S => S.FindAsync(1)).ReturnsAsync(entity);
        _mockContext.Setup(C => C.Set<TestPersistEntity>()).Returns(mockSet.Object);

        var result = await _repository.GetByIdAsync(1);

        Assert.Equal("A", result!.Name);
    }

    [Fact]
    public async Task GetByIdAsync_Int_ReturnsNull_WhenNotFoundAsync()
    {
        var mockSet = new Mock<DbSet<TestPersistEntity>>();
        mockSet.Setup(S => S.FindAsync(1)).ReturnsAsync((TestPersistEntity?)null);
        _mockContext.Setup(C => C.Set<TestPersistEntity>()).Returns(mockSet.Object);

        var result = await _repository.GetByIdAsync(1);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_Guid_ReturnsEntity_WhenFoundAsync()
    {
        var id = Guid.NewGuid();
        var entity = new TestPersistEntity { Id = 1, Name = "A" };
        var mockSet = new Mock<DbSet<TestPersistEntity>>();
        mockSet.Setup(S => S.FindAsync(id)).ReturnsAsync(entity);
        _mockContext.Setup(C => C.Set<TestPersistEntity>()).Returns(mockSet.Object);

        var result = await _repository.GetByIdAsync(id);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetByIdAsync_Int_ResolvesToDerivedMethod_NotBaseMethod_WhenCalledThroughDerivedTypeAsync()
    {
        var mockSet = new Mock<DbSet<TestPersistEntity>>();
        mockSet.Setup(S => S.FindAsync(1)).ReturnsAsync(new TestPersistEntity { Id = 1 });
        _mockContext.Setup(C => C.Set<TestPersistEntity>()).Returns(mockSet.Object);

        BaseRepository<TestPersistEntity, TestPersistEntity> asBase = _repository;
        var viaDerived = await _repository.GetByIdAsync(1);
        var viaBase = await asBase.GetByIdAsync(1);

        Assert.NotNull(viaDerived);
        Assert.NotNull(viaBase);
    }
}
