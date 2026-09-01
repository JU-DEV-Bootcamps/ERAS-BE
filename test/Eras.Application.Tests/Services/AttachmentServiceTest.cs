using System.Security.Cryptography;
using System.Text;

using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Services;
using Eras.Application.DTOs.AttachmentManagement;
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
    private readonly Mock<IUserIdentityProvider> _mockUserIdentityProvider;
    private readonly Mock<ILogger<AttachmentService>> _mockLogger;
    private readonly AttachmentService _service;

    private const string EntityType = InterventionConstants.AttachmentEntityType;

    public AttachmentServiceTest()
    {
        _mockRepository = new Mock<IAttachmentRepository>();
        _mockFileStorage = new Mock<IFileStorageService>();
        _mockSettings = new Mock<IOptions<FileStorageSettings>>();
        _mockUserIdentityProvider = new Mock<IUserIdentityProvider>();
        _mockUserIdentityProvider.Setup(X => X.UserId).Returns("user-1");
        _mockLogger = new Mock<ILogger<AttachmentService>>();

        _mockSettings
            .Setup(X => X.Value)
            .Returns(new FileStorageSettings
            {
                BasePath = "",
                AllowedExtensions = [".pdf", ".png", ".jpg", ".txt"],
                MaxAttachmentsPerEntityType = new Dictionary<string, int> { [EntityType] = 5 }
            });

        _service = new AttachmentService(
            _mockRepository.Object,
            _mockFileStorage.Object,
            _mockSettings.Object,
            _mockUserIdentityProvider.Object,
            _mockLogger.Object);
    }

    private AttachmentService CreateServiceWithSettings(FileStorageSettings Settings)
    {
        var mockSettings = new Mock<IOptions<FileStorageSettings>>();
        mockSettings.Setup(X => X.Value).Returns(Settings);
        return new AttachmentService(
            _mockRepository.Object, _mockFileStorage.Object, mockSettings.Object, _mockUserIdentityProvider.Object, _mockLogger.Object);
    }

    private static MemoryStream ContentStream(string Content) => new(Encoding.UTF8.GetBytes(Content));

    private static string ComputeHash(string Content) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(Content)));

    // A minimal, real PDF byte signature (%PDF) — enough to pass magic-byte validation without
    // needing a genuine, fully-formed PDF document.
    private static MemoryStream RealPdfStream() => new([0x25, 0x50, 0x44, 0x46, 0x2D, 0x31, 0x2E, 0x34]);

    // The Windows/DOS executable signature (MZ) — used to simulate a renamed executable.
    private static MemoryStream FakeExecutableStream() => new([0x4D, 0x5A, 0x90, 0x00, 0x03, 0x00, 0x00, 0x00]);

    [Fact]
    public async Task UploadAttachmentAsync_Should_SaveFileAndPersistMetadata_OnSuccessAsync()
    {
        // Arrange
        _mockRepository
            .Setup(X => X.GetByContentHashAsync(EntityType, 1, It.IsAny<string>()))
            .ReturnsAsync((Attachment?)null);
        _mockRepository.Setup(X => X.CountByEntityAsync(EntityType, 1)).ReturnsAsync(0);
        _mockFileStorage
            .Setup(X => X.SaveAsync(It.IsAny<Stream>(), "report.txt", "interventions/1"))
            .ReturnsAsync("interventions/1/generated.txt");
        _mockRepository
            .Setup(X => X.AddAsync(It.IsAny<Attachment>()))
            .ReturnsAsync((Attachment Obj) => Obj);
        _mockFileStorage.Setup(X => X.GetUrlAsync(It.IsAny<string>())).ReturnsAsync((string?)null);

        // Act
        AttachmentDto result = await _service.UploadAttachmentAsync(
            EntityType, 1, ContentStream("hello"), "report.txt", CancellationToken.None);

        // Assert
        Assert.Equal(EntityType, result.EntityType);
        Assert.Equal(1, result.EntityId);
        Assert.Equal("report.txt", result.OriginalFileName);
        Assert.Equal("user-1", result.CreatedBy);
        Assert.Null(result.DownloadUrl);

        _mockFileStorage.Verify(X => X.SaveAsync(It.IsAny<Stream>(), "report.txt", "interventions/1"), Times.Once);
        _mockRepository.Verify(X => X.AddAsync(It.IsAny<Attachment>()), Times.Once);
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
            StorageKey = "interventions/1/existing.txt",
            ContentHash = hash,
            CreatedBy = "user-1"
        };

        _mockRepository
            .Setup(X => X.GetByContentHashAsync(EntityType, 1, hash))
            .ReturnsAsync(existing);
        _mockFileStorage.Setup(X => X.GetUrlAsync(existing.StorageKey)).ReturnsAsync((string?)null);

        // Act
        AttachmentDto result = await _service.UploadAttachmentAsync(
            EntityType, 1, ContentStream("duplicate-content"), "dup.txt", CancellationToken.None);

        // Assert
        Assert.Equal(99, result.Id);

        _mockFileStorage.Verify(X => X.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _mockRepository.Verify(X => X.AddAsync(It.IsAny<Attachment>()), Times.Never);
    }

    [Fact]
    public async Task UploadAttachmentAsync_Should_ThrowAndNotSave_When_AtMaxAttachmentCountAsync()
    {
        // Arrange
        _mockRepository
            .Setup(X => X.GetByContentHashAsync(EntityType, 1, It.IsAny<string>()))
            .ReturnsAsync((Attachment?)null);
        _mockRepository.Setup(X => X.CountByEntityAsync(EntityType, 1)).ReturnsAsync(5);

        // Act
        var exception = await Assert.ThrowsAsync<BussinessException>(
            () => _service.UploadAttachmentAsync(EntityType, 1, ContentStream("x"), "x.txt", CancellationToken.None));

        // Assert
        Assert.Equal(409, exception.StatusCode);
        _mockFileStorage.Verify(X => X.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task UploadAttachmentAsync_Should_DeleteOrphanedFile_When_MetadataWriteFailsAsync()
    {
        // Arrange
        _mockRepository
            .Setup(X => X.GetByContentHashAsync(EntityType, 1, It.IsAny<string>()))
            .ReturnsAsync((Attachment?)null);
        _mockRepository.Setup(X => X.CountByEntityAsync(EntityType, 1)).ReturnsAsync(0);
        _mockFileStorage
            .Setup(X => X.SaveAsync(It.IsAny<Stream>(), "report.txt", "interventions/1"))
            .ReturnsAsync("interventions/1/generated.txt");
        _mockRepository
            .Setup(X => X.AddAsync(It.IsAny<Attachment>()))
            .ThrowsAsync(new InvalidOperationException("db down"));

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.UploadAttachmentAsync(EntityType, 1, ContentStream("hello"), "report.txt", CancellationToken.None));

        // Assert: the orphaned physical file is cleaned up
        _mockFileStorage.Verify(X => X.DeleteAsync("interventions/1/generated.txt"), Times.Once);
    }

    [Fact]
    public async Task UploadAttachmentAsync_Should_ThrowBadRequest_When_EntityTypeNotRegisteredAsync()
    {
        // Act
        var exception = await Assert.ThrowsAsync<BussinessException>(
            () => _service.UploadAttachmentAsync("unregistered-entity", 1, ContentStream("x"), "x.pdf", CancellationToken.None));

        // Assert
        Assert.Equal(400, exception.StatusCode);
        _mockRepository.Verify(X => X.GetByContentHashAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task UploadAttachmentAsync_Should_ThrowBadRequest_When_ExtensionNotAllowedAsync()
    {
        // Act
        var exception = await Assert.ThrowsAsync<BussinessException>(
            () => _service.UploadAttachmentAsync(EntityType, 1, ContentStream("x"), "malware.exe", CancellationToken.None));

        // Assert
        Assert.Equal(400, exception.StatusCode);
        _mockFileStorage.Verify(X => X.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task UploadAttachmentAsync_Should_RejectRenamedExecutable_DisguisedAsAllowedExtensionAsync()
    {
        // Arrange — a Windows executable (MZ header) renamed to a .jpg, the AC's exact scenario.

        // Act
        var exception = await Assert.ThrowsAsync<BussinessException>(
            () => _service.UploadAttachmentAsync(
                EntityType, 1, FakeExecutableStream(), "photo.jpg", CancellationToken.None));

        // Assert
        Assert.Equal(400, exception.StatusCode);
        _mockFileStorage.Verify(X => X.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _mockRepository.Verify(X => X.AddAsync(It.IsAny<Attachment>()), Times.Never);
    }

    [Fact]
    public async Task UploadAttachmentAsync_Should_RejectRenamedExecutable_DisguisedAsTextFileAsync()
    {
        // Arrange — no fixed magic-byte signature exists for .txt, but a known-dangerous
        // signature (MZ) must still be rejected regardless of claimed extension.

        // Act
        var exception = await Assert.ThrowsAsync<BussinessException>(
            () => _service.UploadAttachmentAsync(
                EntityType, 1, FakeExecutableStream(), "notes.txt", CancellationToken.None));

        // Assert
        Assert.Equal(400, exception.StatusCode);
        _mockFileStorage.Verify(X => X.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task UploadAttachmentAsync_Should_AcceptContent_MatchingItsExtensionsSignatureAsync()
    {
        // Arrange
        _mockRepository.Setup(X => X.GetByContentHashAsync(EntityType, 1, It.IsAny<string>())).ReturnsAsync((Attachment?)null);
        _mockRepository.Setup(X => X.CountByEntityAsync(EntityType, 1)).ReturnsAsync(0);
        _mockFileStorage
            .Setup(X => X.SaveAsync(It.IsAny<Stream>(), "real.pdf", "interventions/1"))
            .ReturnsAsync("interventions/1/real-generated.pdf");
        _mockRepository.Setup(X => X.AddAsync(It.IsAny<Attachment>())).ReturnsAsync((Attachment Obj) => Obj);
        _mockFileStorage.Setup(X => X.GetUrlAsync(It.IsAny<string>())).ReturnsAsync((string?)null);

        // Act
        var result = await _service.UploadAttachmentAsync(
            EntityType, 1, RealPdfStream(), "real.pdf", CancellationToken.None);

        // Assert
        Assert.Equal("real.pdf", result.OriginalFileName);
        _mockFileStorage.Verify(X => X.SaveAsync(It.IsAny<Stream>(), "real.pdf", "interventions/1"), Times.Once);
    }

    [Fact]
    public async Task UploadAttachmentAsync_Should_RejectMismatchedContent_ForAKnownSignatureExtensionAsync()
    {
        // Arrange — plain text content claiming to be a PDF: no dangerous signature, but it still
        // doesn't match .pdf's known signature, so it must be rejected rather than trusted.

        // Act
        BussinessException exception = await Assert.ThrowsAsync<BussinessException>(
            () => _service.UploadAttachmentAsync(
                EntityType, 1, ContentStream("not actually a pdf"), "fake.pdf", CancellationToken.None));

        // Assert
        Assert.Equal(400, exception.StatusCode);
        _mockFileStorage.Verify(X => X.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task UploadAttachmentAsync_Should_RejectFile_ExceedingMaxFileSizeBytesAsync()
    {
        // Arrange
        AttachmentService service = CreateServiceWithSettings(new FileStorageSettings
        {
            BasePath = "",
            AllowedExtensions = [".txt"],
            MaxFileSizeBytes = 10,
            MaxAttachmentsPerEntityType = new Dictionary<string, int> { [EntityType] = 5 }
        });
        Stream oversized = ContentStream(new string('a', 11)); // 11 bytes > 10-byte limit

        // Act
        BussinessException exception = await Assert.ThrowsAsync<BussinessException>(
            () => service.UploadAttachmentAsync(EntityType, 1, oversized, "big.txt", CancellationToken.None));

        // Assert
        Assert.Equal(400, exception.StatusCode);
        Assert.Contains("exceeds the maximum allowed size", exception.FriendlyMessage);
        _mockFileStorage.Verify(X => X.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task UploadAttachmentAsync_Should_Accept_FileAtExactlyTheSizeLimitAsync()
    {
        // Arrange
        AttachmentService service = CreateServiceWithSettings(new FileStorageSettings
        {
            BasePath = "",
            AllowedExtensions = [".txt"],
            MaxFileSizeBytes = 10,
            MaxAttachmentsPerEntityType = new Dictionary<string, int> { [EntityType] = 5 }
        });
        Stream exactlyAtLimit = ContentStream(new string('a', 10)); // exactly 10 bytes

        _mockRepository.Setup(X => X.GetByContentHashAsync(EntityType, 1, It.IsAny<string>())).ReturnsAsync((Attachment?)null);
        _mockRepository.Setup(X => X.CountByEntityAsync(EntityType, 1)).ReturnsAsync(0);
        _mockFileStorage
            .Setup(X => X.SaveAsync(It.IsAny<Stream>(), "ok.txt", "interventions/1"))
            .ReturnsAsync("interventions/1/ok-generated.txt");
        _mockRepository.Setup(X => X.AddAsync(It.IsAny<Attachment>())).ReturnsAsync((Attachment Obj) => Obj);
        _mockFileStorage.Setup(X => X.GetUrlAsync(It.IsAny<string>())).ReturnsAsync((string?)null);

        // Act
        AttachmentDto result = await service.UploadAttachmentAsync(EntityType, 1, exactlyAtLimit, "ok.txt", CancellationToken.None);

        // Assert
        Assert.Equal(10, result.SizeBytes);
    }

    [Fact]
    public async Task UploadAttachmentsAsync_Should_SaveAllFiles_OnSuccessAsync()
    {
        // Arrange
        _mockRepository
            .Setup(X => X.GetByContentHashAsync(EntityType, 1, It.IsAny<string>()))
            .ReturnsAsync((Attachment?)null);
        _mockRepository.Setup(X => X.CountByEntityAsync(EntityType, 1)).ReturnsAsync(0);
        _mockFileStorage
            .Setup(X => X.SaveAsync(It.IsAny<Stream>(), "a.txt", "interventions/1"))
            .ReturnsAsync("interventions/1/a-generated.txt");
        _mockFileStorage
            .Setup(X => X.SaveAsync(It.IsAny<Stream>(), "b.txt", "interventions/1"))
            .ReturnsAsync("interventions/1/b-generated.txt");
        _mockRepository.Setup(X => X.AddAsync(It.IsAny<Attachment>())).ReturnsAsync((Attachment Obj) => Obj);
        _mockFileStorage.Setup(X => X.GetUrlAsync(It.IsAny<string>())).ReturnsAsync((string?)null);

        var files = new List<(Stream Stream, string FileName)>
        {
            (ContentStream("content-a"), "a.txt"),
            (ContentStream("content-b"), "b.txt")
        };

        // Act
        IReadOnlyCollection<AttachmentDto> result = await _service.UploadAttachmentsAsync(EntityType, 1, files, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        _mockRepository.Verify(X => X.AddAsync(It.IsAny<Attachment>()), Times.Exactly(2));
    }

    [Fact]
    public async Task UploadAttachmentsAsync_Should_RollBackEarlierFiles_When_ALaterFileFailsAsync()
    {
        // Arrange
        _mockRepository
            .Setup(X => X.GetByContentHashAsync(EntityType, 1, It.IsAny<string>()))
            .ReturnsAsync((Attachment?)null);
        _mockRepository.SetupSequence(X => X.CountByEntityAsync(EntityType, 1))
            .ReturnsAsync(0)  // "a.pdf": under the limit, allowed
            .ReturnsAsync(5); // "b.pdf": at the limit, rejected

        _mockFileStorage
            .Setup(X => X.SaveAsync(It.IsAny<Stream>(), "a.txt", "interventions/1"))
            .ReturnsAsync("interventions/1/a-generated.txt");

        var createdAttachment = new Attachment
        {
            Id = 42,
            EntityType = EntityType,
            EntityId = 1,
            StorageKey = "interventions/1/a-generated.txt",
            ContentHash = "hash-a",
            CreatedBy = "user-1"
        };
        _mockRepository.Setup(X => X.AddAsync(It.IsAny<Attachment>())).ReturnsAsync(createdAttachment);
        _mockRepository.Setup(X => X.GetByIdAsync(42)).ReturnsAsync(createdAttachment);
        _mockFileStorage.Setup(X => X.GetUrlAsync(It.IsAny<string>())).ReturnsAsync((string?)null);

        var files = new List<(Stream Stream, string FileName)>
        {
            (ContentStream("content-a"), "a.txt"),
            (ContentStream("content-b"), "b.txt")
        };

        // Act
        var exception = await Assert.ThrowsAsync<BussinessException>(
            () => _service.UploadAttachmentsAsync(EntityType, 1, files, CancellationToken.None));

        // Assert
        Assert.Equal(409, exception.StatusCode);

        // "a.txt" was saved, then rolled back once "b.txt" failed the batch
        _mockRepository.Verify(X => X.AddAsync(It.IsAny<Attachment>()), Times.Once);
        _mockRepository.Verify(X => X.DeleteAsync(createdAttachment), Times.Once);
        _mockFileStorage.Verify(X => X.DeleteAsync("interventions/1/a-generated.txt"), Times.Once);

        // "b.txt" never reached physical storage — it failed the max-count check first
        _mockFileStorage.Verify(X => X.SaveAsync(It.IsAny<Stream>(), "b.txt", It.IsAny<string>()), Times.Never);
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
        BussinessException exception = await Assert.ThrowsAsync<BussinessException>(
            () => _service.UploadAttachmentsAsync(EntityType, 1, files, CancellationToken.None));

        // Assert
        Assert.Equal(400, exception.StatusCode);
        _mockFileStorage.Verify(X => X.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _mockRepository.Verify(X => X.AddAsync(It.IsAny<Attachment>()), Times.Never);
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
        _mockRepository.Setup(X => X.GetByEntityAsync(EntityType, 1)).ReturnsAsync(attachments);
        _mockFileStorage.Setup(X => X.GetUrlAsync(It.IsAny<string>())).ReturnsAsync((string?)null);

        // Act
        IReadOnlyCollection<AttachmentDto> result = await _service.ListAttachmentsAsync(EntityType, 1, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task DownloadAttachmentAsync_Should_ThrowNotFound_When_AttachmentMissingAsync()
    {
        // Arrange
        _mockRepository.Setup(X => X.GetByIdAsync(999)).ReturnsAsync((Attachment?)null);

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
        _mockRepository.Setup(X => X.GetByIdAsync(1)).ReturnsAsync(attachment);

        // Act
        await _service.DeleteAttachmentAsync(1, CancellationToken.None);

        // Assert
        _mockRepository.Verify(X => X.DeleteAsync(attachment), Times.Once);
        _mockFileStorage.Verify(X => X.DeleteAsync("interventions/1/x.pdf"), Times.Once);
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
        _mockRepository.Setup(X => X.GetByIdAsync(1)).ReturnsAsync(attachment);
        _mockFileStorage.Setup(X => X.DeleteAsync(attachment.StorageKey)).ThrowsAsync(new IOException("disk error"));

        // Act & Assert (should not throw despite physical delete failure)
        await _service.DeleteAttachmentAsync(1, CancellationToken.None);
        _mockRepository.Verify(X => X.DeleteAsync(attachment), Times.Once);
    }

    [Fact]
    public async Task DeleteAttachmentAsync_Should_ThrowNotFound_When_AttachmentMissingAsync()
    {
        // Arrange
        _mockRepository.Setup(X => X.GetByIdAsync(999)).ReturnsAsync((Attachment?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.DeleteAttachmentAsync(999, CancellationToken.None));
    }
}
