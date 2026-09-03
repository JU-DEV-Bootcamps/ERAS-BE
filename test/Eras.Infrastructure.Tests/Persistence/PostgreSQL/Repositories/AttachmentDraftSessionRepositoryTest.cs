using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories;

public class AttachmentDraftSessionRepositoryTest
{
    private static AttachmentDraftSessionRepository CreateRepository(string databaseName, out AppDbContext context)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        context = new AppDbContext(options);
        return new AttachmentDraftSessionRepository(context);
    }

    private static AttachmentDraftSessionRepository CreateRepository(out AppDbContext context) =>
        CreateRepository(Guid.NewGuid().ToString(), out context);

    [Fact]
    public async Task AddAsync_Should_PersistDraftSession_AndAssignAnIdAsync()
    {
        // Arrange
        var repository = CreateRepository(out _);
        var session = new AttachmentDraftSession { CreatedBy = "user-1" };

        // Act
        AttachmentDraftSession persisted = await repository.AddAsync(session);

        // Assert
        Assert.NotEqual(0, persisted.Id);
        Assert.Equal("user-1", persisted.CreatedBy);
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnPersistedDraftSessionAsync()
    {
        // Arrange
        var repository = CreateRepository(out _);
        AttachmentDraftSession persisted = await repository.AddAsync(new AttachmentDraftSession { CreatedBy = "user-1" });

        // Act
        AttachmentDraftSession? result = await repository.GetByIdAsync(persisted.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(persisted.Id, result!.Id);
        Assert.Equal("user-1", result.CreatedBy);
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnNull_When_NoSessionExistsAsync()
    {
        // Arrange
        var repository = CreateRepository(out _);

        // Act
        AttachmentDraftSession? result = await repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_Should_RemoveDraftSessionAsync()
    {
        // Arrange — a fresh DbContext for the delete, same as a real request would get its own
        // scoped context; reusing the AddAsync context here would hand DeleteAsync's freshly
        // mapped persistence instance to a change tracker that still has the original AddAsync
        // instance tracked under the same key, which EF rejects as a duplicate.
        string databaseName = Guid.NewGuid().ToString();
        var addRepository = CreateRepository(databaseName, out _);
        AttachmentDraftSession persisted = await addRepository.AddAsync(new AttachmentDraftSession { CreatedBy = "user-1" });
        var deleteRepository = CreateRepository(databaseName, out _);

        // Act
        await deleteRepository.DeleteAsync(persisted);

        // Assert
        var verifyRepository = CreateRepository(databaseName, out _);
        Assert.Null(await verifyRepository.GetByIdAsync(persisted.Id));
    }

    [Fact]
    public async Task DeleteByIdAsync_Should_RemoveDraftSession_EvenAfterAPriorGetByIdInTheSameContextAsync()
    {
        // Arrange — reproduces a real request: GetByIdAsync tracks the session, then it's deleted
        // in that same DbContext. DeleteAsync(entity) would conflict here (see the test above);
        // DeleteByIdAsync reuses the already-tracked instance instead.
        var repository = CreateRepository(out _);
        AttachmentDraftSession persisted = await repository.AddAsync(new AttachmentDraftSession { CreatedBy = "user-1" });
        AttachmentDraftSession? loaded = await repository.GetByIdAsync(persisted.Id);
        Assert.NotNull(loaded);

        // Act
        await repository.DeleteByIdAsync(persisted.Id);

        // Assert
        Assert.Null(await repository.GetByIdAsync(persisted.Id));
    }

    [Fact]
    public async Task DeleteByIdAsync_Should_BeANoOp_When_NoSessionExistsForIdAsync()
    {
        // Arrange
        var repository = CreateRepository(out _);

        // Act & Assert — no throw
        await repository.DeleteByIdAsync(999);
    }
}
