using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories;

public class AttachmentRepositoryTest
{
    private static AttachmentRepository CreateRepository(out AppDbContext context)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        context = new AppDbContext(options);
        return new AttachmentRepository(context);
    }

    private static Attachment BuildAttachment(
        string entityType,
        int entityId,
        string storageKey,
        string contentHash,
        string? originalFileName = null,
        string? mimeType = null,
        long? sizeBytes = null)
    {
        return new Attachment
        {
            EntityType = entityType,
            EntityId = entityId,
            OriginalFileName = originalFileName,
            StorageKey = storageKey,
            ContentHash = contentHash,
            MimeType = mimeType,
            SizeBytes = sizeBytes,
            CreatedBy = "user-uuid-1"
        };
    }

    [Fact]
    public async Task AddAsync_Should_PersistAttachment_WithAllFieldsAsync()
    {
        // Arrange
        var repository = CreateRepository(out _);
        var attachment = BuildAttachment(
            "Intervention", 42, "Intervention/42/11111111-1111-1111-1111-111111111111.pdf", new string('a', 64),
            originalFileName: "report.pdf", mimeType: "application/pdf", sizeBytes: 12345);

        // Act
        Attachment persisted = await repository.AddAsync(attachment);

        // Assert
        Assert.NotEqual(0, persisted.Id);
        Assert.Equal("Intervention", persisted.EntityType);
        Assert.Equal(42, persisted.EntityId);
        Assert.Equal("report.pdf", persisted.OriginalFileName);
        Assert.Equal(attachment.StorageKey, persisted.StorageKey);
        Assert.Equal(AttachmentStorageProvider.LocalFileSystem, persisted.StorageProvider);
        Assert.Equal("application/pdf", persisted.MimeType);
        Assert.Equal(12345, persisted.SizeBytes);
        Assert.Equal(new string('a', 64), persisted.ContentHash);
    }

    [Fact]
    public async Task AddAsync_Should_DefaultStorageProvider_ToLocalFileSystemAsync()
    {
        // Arrange
        var repository = CreateRepository(out _);
        var attachment = BuildAttachment("Intervention", 1, "Intervention/1/file.bin", new string('b', 64));

        // Act
        Attachment persisted = await repository.AddAsync(attachment);

        // Assert
        Assert.Equal(AttachmentStorageProvider.LocalFileSystem, persisted.StorageProvider);
    }

    [Fact]
    public async Task AddAsync_Should_PersistLegacyOnlyFieldsAsNull_WhenNotProvidedAsync()
    {
        // Documents the permanent limitation from User Story 1.1: rows backfilled from legacy
        // Intervention data never have original file name / mime type / size available.

        // Arrange
        var repository = CreateRepository(out _);
        var attachment = BuildAttachment("Intervention", 7, "Intervention/7/legacy-file.bin", new string('c', 64));

        // Act
        Attachment persisted = await repository.AddAsync(attachment);

        // Assert
        Assert.Null(persisted.OriginalFileName);
        Assert.Null(persisted.MimeType);
        Assert.Null(persisted.SizeBytes);
    }

    [Fact]
    public async Task GetByEntityAsync_Should_ReturnOnlyMatchingAttachmentsAsync()
    {
        // Arrange
        var repository = CreateRepository(out _);
        await repository.AddAsync(BuildAttachment("Intervention", 1, "Intervention/1/a.bin", new string('d', 64)));
        await repository.AddAsync(BuildAttachment("Intervention", 2, "Intervention/2/b.bin", new string('e', 64)));
        await repository.AddAsync(BuildAttachment("Assessment", 1, "Assessment/1/c.bin", new string('f', 64)));

        // Act
        IEnumerable<Attachment> results = await repository.GetByEntityAsync("Intervention", 1);

        // Assert
        Attachment result = Assert.Single(results);
        Assert.Equal("Intervention/1/a.bin", result.StorageKey);
    }

    [Fact]
    public async Task GetByEntityAsync_Should_ReturnEmpty_When_NoAttachmentsExistAsync()
    {
        // Arrange
        var repository = CreateRepository(out _);

        // Act
        IEnumerable<Attachment> results = await repository.GetByEntityAsync("Intervention", 999);

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public async Task GetByContentHashAsync_Should_ReturnMatch_When_HashAlreadyExistsForEntityAsync()
    {
        // Arrange
        var repository = CreateRepository(out _);
        var hash = new string('g', 64);
        await repository.AddAsync(BuildAttachment("Intervention", 1, "Intervention/1/a.bin", hash));

        // Act
        Attachment? result = await repository.GetByContentHashAsync("Intervention", 1, hash);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Intervention/1/a.bin", result!.StorageKey);
    }

    [Fact]
    public async Task GetByContentHashAsync_Should_ReturnNull_When_HashBelongsToDifferentEntityAsync()
    {
        // Arrange
        var repository = CreateRepository(out _);
        var hash = new string('h', 64);
        await repository.AddAsync(BuildAttachment("Intervention", 1, "Intervention/1/a.bin", hash));

        // Act
        Attachment? result = await repository.GetByContentHashAsync("Intervention", 2, hash);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CountByEntityAsync_Should_ReturnNumberOfAttachmentsForEntityAsync()
    {
        // Arrange
        var repository = CreateRepository(out _);
        await repository.AddAsync(BuildAttachment("Intervention", 1, "Intervention/1/a.bin", new string('i', 64)));
        await repository.AddAsync(BuildAttachment("Intervention", 1, "Intervention/1/b.bin", new string('j', 64)));
        await repository.AddAsync(BuildAttachment("Intervention", 2, "Intervention/2/c.bin", new string('k', 64)));

        // Act
        int count = await repository.CountByEntityAsync("Intervention", 1);

        // Assert
        Assert.Equal(2, count);
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnPersistedAttachmentAsync()
    {
        // Arrange
        var repository = CreateRepository(out _);
        Attachment persisted = await repository.AddAsync(
            BuildAttachment("Intervention", 1, "Intervention/1/a.bin", new string('l', 64)));

        // Act
        Attachment? result = await repository.GetByIdAsync(persisted.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(persisted.Id, result!.Id);
    }

    [Fact]
    public async Task DeleteAsync_Should_RemoveAttachmentAsync()
    {
        // Arrange
        var repository = CreateRepository(out _);
        Attachment persisted = await repository.AddAsync(
            BuildAttachment("Intervention", 1, "Intervention/1/a.bin", new string('m', 64)));

        // Act
        await repository.DeleteAsync(persisted);

        // Assert
        Assert.Null(await repository.GetByIdAsync(persisted.Id));
    }

    [Fact]
    public async Task GetStaleByEntityTypeAsync_Should_ReturnOnlyMatchingTypeOlderThanCutoffAsync()
    {
        // Arrange
        var repository = CreateRepository(out _);
        DateTime cutoff = DateTime.UtcNow.AddHours(-24);

        var staleTemp = new Attachment
        {
            EntityType = "Temp", EntityId = 1, StorageKey = "Temp/1/old.bin", ContentHash = new string('n', 64),
            CreatedBy = "user-uuid-1", CreatedAt = DateTime.UtcNow.AddHours(-30)
        };
        var freshTemp = new Attachment
        {
            EntityType = "Temp", EntityId = 2, StorageKey = "Temp/2/new.bin", ContentHash = new string('o', 64),
            CreatedBy = "user-uuid-1", CreatedAt = DateTime.UtcNow.AddHours(-1)
        };
        var staleOtherType = new Attachment
        {
            EntityType = "Intervention", EntityId = 3, StorageKey = "Intervention/3/old.bin", ContentHash = new string('p', 64),
            CreatedBy = "user-uuid-1", CreatedAt = DateTime.UtcNow.AddHours(-30)
        };

        await repository.AddAsync(staleTemp);
        await repository.AddAsync(freshTemp);
        await repository.AddAsync(staleOtherType);

        // Act
        IReadOnlyCollection<Attachment> result = await repository.GetStaleByEntityTypeAsync("Temp", cutoff, CancellationToken.None);

        // Assert
        Attachment onlyResult = Assert.Single(result);
        Assert.Equal(staleTemp.StorageKey, onlyResult.StorageKey);
    }

    [Fact]
    public async Task GetStaleByEntityTypeAsync_Should_ReturnEmpty_When_NothingIsStaleAsync()
    {
        // Arrange
        var repository = CreateRepository(out _);
        await repository.AddAsync(new Attachment
        {
            EntityType = "Temp", EntityId = 1, StorageKey = "Temp/1/new.bin", ContentHash = new string('r', 64),
            CreatedBy = "user-uuid-1", CreatedAt = DateTime.UtcNow
        });

        // Act
        IReadOnlyCollection<Attachment> result = await repository.GetStaleByEntityTypeAsync(
            "Temp", DateTime.UtcNow.AddHours(-24), CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }
}
