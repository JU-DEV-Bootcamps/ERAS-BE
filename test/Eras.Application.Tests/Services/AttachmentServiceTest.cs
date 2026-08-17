using System.Security.Cryptography;
using System.Text;

using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Models;
using Eras.Application.Services;
using Eras.Domain.Entities;
using Eras.Domain.Entities.AssessmentManagement;
using Eras.Error.Bussiness;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moq;

namespace Eras.Application.Tests.Services;

public class AttachmentServiceTest
{
    private readonly Mock<IAttachmentRepository> _mockRepository;
    private readonly Mock<IFileStorageService> _mockFileStorage;
    private readonly Mock<IOptions<FileStorageSettings>> _mockSettings;
    private readonly Mock<ILogger<AttachmentService>> _mockLogger;
    private readonly AttachmentService _service;

    private const string EntityType = InterventionConstants.AttachmentEntityType;

    public AttachmentServiceTest()
    {
        _mockRepository = new Mock<IAttachmentRepository>();
        _mockFileStorage = new Mock<IFileStorageService>();
        _mockSettings = new Mock<IOptions<FileStorageSettings>>();
        _mockLogger = new Mock<ILogger<AttachmentService>>();

        _mockSettings
            .Setup(x => x.Value)
            .Returns(new FileStorageSettings
            {
                BasePath = "",
                AllowedExtensions = [".pdf", ".png"],
                MaxAttachmentsPerEntityType = new Dictionary<string, int> { [EntityType] = 5 }
            });

        _service = new AttachmentService(
            _mockRepository.Object,
            _mockFileStorage.Object,
            _mockSettings.Object,
            _mockLogger.Object);
    }

    private static Stream ContentStream(string content) => new MemoryStream(Encoding.UTF8.GetBytes(content));

    private static string ComputeHash(string content) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(content)));

    [Fact]
    public async Task UploadAttachmentAsync_Should_SaveFileAndPersistMetadata_OnSuccessAsync()
    {
        // Arrange
        _mockRepository
            .Setup(x => x.GetByContentHashAsync(EntityType, 1, It.IsAny<string>()))
            .ReturnsAsync((Attachment?)null);
        _mockRepository.Setup(x => x.CountByEntityAsync(EntityType, 1)).ReturnsAsync(0);
        _mockFileStorage
            .Setup(x => x.SaveAsync(It.IsAny<Stream>(), "report.pdf", "interventions/1"))
            .ReturnsAsync("interventions/1/generated.pdf");
        _mockRepository
            .Setup(x => x.AddAsync(It.IsAny<Attachment>()))
            .ReturnsAsync((Attachment a) => a);
        _mockFileStorage.Setup(x => x.GetUrlAsync(It.IsAny<string>())).ReturnsAsync((string?)null);

        // Act
        var result = await _service.UploadAttachmentAsync(
            EntityType, 1, ContentStream("hello"), "report.pdf", "user-1", CancellationToken.None);

        // Assert
        Assert.Equal(EntityType, result.EntityType);
        Assert.Equal(1, result.EntityId);
        Assert.Equal("report.pdf", result.OriginalFileName);
        Assert.Equal("user-1", result.CreatedBy);
        Assert.Null(result.DownloadUrl);

        _mockFileStorage.Verify(x => x.SaveAsync(It.IsAny<Stream>(), "report.pdf", "interventions/1"), Times.Once);
        _mockRepository.Verify(x => x.AddAsync(It.IsAny<Attachment>()), Times.Once);
    }

    [Fact]
    public async Task UploadAttachmentAsync_Should_SkipSaving_When_ContentHashAlreadyExistsAsync()
    {
        // Arrange
        var hash = ComputeHash("duplicate-content");
        var existing = new Attachment
        {
            Id = 99,
            EntityType = EntityType,
            EntityId = 1,
            StorageKey = "interventions/1/existing.pdf",
            ContentHash = hash,
            CreatedBy = "user-1"
        };

        _mockRepository
            .Setup(x => x.GetByContentHashAsync(EntityType, 1, hash))
            .ReturnsAsync(existing);
        _mockFileStorage.Setup(x => x.GetUrlAsync(existing.StorageKey)).ReturnsAsync((string?)null);

        // Act
        var result = await _service.UploadAttachmentAsync(
            EntityType, 1, ContentStream("duplicate-content"), "dup.pdf", "user-1", CancellationToken.None);

        // Assert
        Assert.Equal(99, result.Id);

        _mockFileStorage.Verify(x => x.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _mockRepository.Verify(x => x.AddAsync(It.IsAny<Attachment>()), Times.Never);
    }

    [Fact]
    public async Task UploadAttachmentAsync_Should_ThrowAndNotSave_When_AtMaxAttachmentCountAsync()
    {
        // Arrange
        _mockRepository
            .Setup(x => x.GetByContentHashAsync(EntityType, 1, It.IsAny<string>()))
            .ReturnsAsync((Attachment?)null);
        _mockRepository.Setup(x => x.CountByEntityAsync(EntityType, 1)).ReturnsAsync(5);

        // Act
        var exception = await Assert.ThrowsAsync<BussinessException>(
            () => _service.UploadAttachmentAsync(EntityType, 1, ContentStream("x"), "x.pdf", "user-1", CancellationToken.None));

        // Assert
        Assert.Equal(409, exception.StatusCode);
        _mockFileStorage.Verify(x => x.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task UploadAttachmentAsync_Should_DeleteOrphanedFile_When_MetadataWriteFailsAsync()
    {
        // Arrange
        _mockRepository
            .Setup(x => x.GetByContentHashAsync(EntityType, 1, It.IsAny<string>()))
            .ReturnsAsync((Attachment?)null);
        _mockRepository.Setup(x => x.CountByEntityAsync(EntityType, 1)).ReturnsAsync(0);
        _mockFileStorage
            .Setup(x => x.SaveAsync(It.IsAny<Stream>(), "report.pdf", "interventions/1"))
            .ReturnsAsync("interventions/1/generated.pdf");
        _mockRepository
            .Setup(x => x.AddAsync(It.IsAny<Attachment>()))
            .ThrowsAsync(new InvalidOperationException("db down"));

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.UploadAttachmentAsync(EntityType, 1, ContentStream("hello"), "report.pdf", "user-1", CancellationToken.None));

        // Assert: the orphaned physical file is cleaned up
        _mockFileStorage.Verify(x => x.DeleteAsync("interventions/1/generated.pdf"), Times.Once);
    }

    [Fact]
    public async Task UploadAttachmentAsync_Should_ThrowBadRequest_When_EntityTypeNotRegisteredAsync()
    {
        // Act
        var exception = await Assert.ThrowsAsync<BussinessException>(
            () => _service.UploadAttachmentAsync("unregistered-entity", 1, ContentStream("x"), "x.pdf", "user-1", CancellationToken.None));

        // Assert
        Assert.Equal(400, exception.StatusCode);
        _mockRepository.Verify(x => x.GetByContentHashAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task UploadAttachmentAsync_Should_ThrowBadRequest_When_ExtensionNotAllowedAsync()
    {
        // Act
        var exception = await Assert.ThrowsAsync<BussinessException>(
            () => _service.UploadAttachmentAsync(EntityType, 1, ContentStream("x"), "malware.exe", "user-1", CancellationToken.None));

        // Assert
        Assert.Equal(400, exception.StatusCode);
        _mockFileStorage.Verify(x => x.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task UploadAttachmentsAsync_Should_SaveAllFiles_OnSuccessAsync()
    {
        // Arrange
        _mockRepository
            .Setup(x => x.GetByContentHashAsync(EntityType, 1, It.IsAny<string>()))
            .ReturnsAsync((Attachment?)null);
        _mockRepository.Setup(x => x.CountByEntityAsync(EntityType, 1)).ReturnsAsync(0);
        _mockFileStorage
            .Setup(x => x.SaveAsync(It.IsAny<Stream>(), "a.pdf", "interventions/1"))
            .ReturnsAsync("interventions/1/a-generated.pdf");
        _mockFileStorage
            .Setup(x => x.SaveAsync(It.IsAny<Stream>(), "b.pdf", "interventions/1"))
            .ReturnsAsync("interventions/1/b-generated.pdf");
        _mockRepository.Setup(x => x.AddAsync(It.IsAny<Attachment>())).ReturnsAsync((Attachment a) => a);
        _mockFileStorage.Setup(x => x.GetUrlAsync(It.IsAny<string>())).ReturnsAsync((string?)null);

        var files = new List<(Stream Stream, string FileName)>
        {
            (ContentStream("content-a"), "a.pdf"),
            (ContentStream("content-b"), "b.pdf")
        };

        // Act
        var result = await _service.UploadAttachmentsAsync(EntityType, 1, files, "user-1", CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        _mockRepository.Verify(x => x.AddAsync(It.IsAny<Attachment>()), Times.Exactly(2));
    }

    [Fact]
    public async Task UploadAttachmentsAsync_Should_RollBackEarlierFiles_When_ALaterFileFailsAsync()
    {
        // Arrange
        _mockRepository
            .Setup(x => x.GetByContentHashAsync(EntityType, 1, It.IsAny<string>()))
            .ReturnsAsync((Attachment?)null);
        _mockRepository.SetupSequence(x => x.CountByEntityAsync(EntityType, 1))
            .ReturnsAsync(0)  // "a.pdf": under the limit, allowed
            .ReturnsAsync(5); // "b.pdf": at the limit, rejected

        _mockFileStorage
            .Setup(x => x.SaveAsync(It.IsAny<Stream>(), "a.pdf", "interventions/1"))
            .ReturnsAsync("interventions/1/a-generated.pdf");

        var createdAttachment = new Attachment
        {
            Id = 42,
            EntityType = EntityType,
            EntityId = 1,
            StorageKey = "interventions/1/a-generated.pdf",
            ContentHash = "hash-a",
            CreatedBy = "user-1"
        };
        _mockRepository.Setup(x => x.AddAsync(It.IsAny<Attachment>())).ReturnsAsync(createdAttachment);
        _mockRepository.Setup(x => x.GetByIdAsync(42)).ReturnsAsync(createdAttachment);
        _mockFileStorage.Setup(x => x.GetUrlAsync(It.IsAny<string>())).ReturnsAsync((string?)null);

        var files = new List<(Stream Stream, string FileName)>
        {
            (ContentStream("content-a"), "a.pdf"),
            (ContentStream("content-b"), "b.pdf")
        };

        // Act
        var exception = await Assert.ThrowsAsync<BussinessException>(
            () => _service.UploadAttachmentsAsync(EntityType, 1, files, "user-1", CancellationToken.None));

        // Assert
        Assert.Equal(409, exception.StatusCode);

        // "a.pdf" was saved, then rolled back once "b.pdf" failed the batch
        _mockRepository.Verify(x => x.AddAsync(It.IsAny<Attachment>()), Times.Once);
        _mockRepository.Verify(x => x.DeleteAsync(createdAttachment), Times.Once);
        _mockFileStorage.Verify(x => x.DeleteAsync("interventions/1/a-generated.pdf"), Times.Once);

        // "b.pdf" never reached physical storage — it failed the max-count check first
        _mockFileStorage.Verify(x => x.SaveAsync(It.IsAny<Stream>(), "b.pdf", It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task UploadAttachmentsAsync_Should_ValidateAllExtensions_BeforeSavingAnyFileAsync()
    {
        // Arrange
        var files = new List<(Stream Stream, string FileName)>
        {
            (ContentStream("content-a"), "a.pdf"),
            (ContentStream("content-b"), "malware.exe")
        };

        // Act
        var exception = await Assert.ThrowsAsync<BussinessException>(
            () => _service.UploadAttachmentsAsync(EntityType, 1, files, "user-1", CancellationToken.None));

        // Assert
        Assert.Equal(400, exception.StatusCode);
        _mockFileStorage.Verify(x => x.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _mockRepository.Verify(x => x.AddAsync(It.IsAny<Attachment>()), Times.Never);
    }

    [Fact]
    public async Task ListAttachmentsAsync_Should_ReturnMappedDtosAsync()
    {
        // Arrange
        var attachments = new List<Attachment>
        {
            new() { Id = 1, EntityType = EntityType, EntityId = 1, StorageKey = "a", ContentHash = "h1", CreatedBy = "u" },
            new() { Id = 2, EntityType = EntityType, EntityId = 1, StorageKey = "b", ContentHash = "h2", CreatedBy = "u" }
        };
        _mockRepository.Setup(x => x.GetByEntityAsync(EntityType, 1)).ReturnsAsync(attachments);
        _mockFileStorage.Setup(x => x.GetUrlAsync(It.IsAny<string>())).ReturnsAsync((string?)null);

        // Act
        var result = await _service.ListAttachmentsAsync(EntityType, 1, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task DownloadAttachmentAsync_Should_ThrowNotFound_When_AttachmentMissingAsync()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Attachment?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.DownloadAttachmentAsync(999, CancellationToken.None));
    }

    [Fact]
    public async Task DeleteAttachmentAsync_Should_DeleteMetadataThenPhysicalFileAsync()
    {
        // Arrange
        var attachment = new Attachment
        {
            Id = 1, EntityType = EntityType, EntityId = 1, StorageKey = "interventions/1/x.pdf",
            ContentHash = "h", CreatedBy = "u"
        };
        _mockRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(attachment);

        // Act
        await _service.DeleteAttachmentAsync(1, CancellationToken.None);

        // Assert
        _mockRepository.Verify(x => x.DeleteAsync(attachment), Times.Once);
        _mockFileStorage.Verify(x => x.DeleteAsync("interventions/1/x.pdf"), Times.Once);
    }

    [Fact]
    public async Task DeleteAttachmentAsync_Should_NotThrow_When_PhysicalFileDeleteFailsAsync()
    {
        // Arrange
        var attachment = new Attachment
        {
            Id = 1, EntityType = EntityType, EntityId = 1, StorageKey = "interventions/1/x.pdf",
            ContentHash = "h", CreatedBy = "u"
        };
        _mockRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(attachment);
        _mockFileStorage.Setup(x => x.DeleteAsync(attachment.StorageKey)).ThrowsAsync(new IOException("disk error"));

        // Act & Assert (should not throw despite physical delete failure)
        await _service.DeleteAttachmentAsync(1, CancellationToken.None);
        _mockRepository.Verify(x => x.DeleteAsync(attachment), Times.Once);
    }

    [Fact]
    public async Task DeleteAttachmentAsync_Should_ThrowNotFound_When_AttachmentMissingAsync()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Attachment?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.DeleteAttachmentAsync(999, CancellationToken.None));
    }
}
