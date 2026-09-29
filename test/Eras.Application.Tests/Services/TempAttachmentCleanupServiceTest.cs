using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Services;
using Eras.Application.Models;
using Eras.Application.Services;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moq;

namespace Eras.Application.Tests.Services;

public class TempAttachmentCleanupServiceTest
{
    private readonly Mock<IAttachmentRepository> _mockAttachmentRepository;
    private readonly Mock<IAttachmentDraftSessionRepository> _mockDraftSessionRepository;
    private readonly Mock<IAttachmentService> _mockAttachmentService;
    private readonly TempAttachmentCleanupService _service;

    private const int TtlHours = 24;

    public TempAttachmentCleanupServiceTest()
    {
        _mockAttachmentRepository = new Mock<IAttachmentRepository>();
        _mockDraftSessionRepository = new Mock<IAttachmentDraftSessionRepository>();
        _mockAttachmentService = new Mock<IAttachmentService>();

        var mockSettings = new Mock<IOptions<FileStorageSettings>>();
        mockSettings.Setup(X => X.Value).Returns(new FileStorageSettings
        {
            BasePath = "",
            AllowedExtensions = [".pdf"],
            TempAttachmentTtlHours = TtlHours
        });

        _service = new TempAttachmentCleanupService(
            _mockAttachmentRepository.Object,
            _mockDraftSessionRepository.Object,
            _mockAttachmentService.Object,
            mockSettings.Object,
            Mock.Of<ILogger<TempAttachmentCleanupService>>());
    }

    private static Attachment BuildStaleAttachment(int id, int entityId) => new()
    {
        Id = id,
        EntityType = AttachmentDraftSession.AttachmentEntityType,
        EntityId = entityId,
        StorageKey = $"Temp/{entityId}/file.pdf",
        ContentHash = new string('a', 64),
        CreatedBy = "user-1",
        CreatedAt = DateTime.UtcNow.AddHours(-(TtlHours + 1))
    };

    [Fact]
    public async Task RunAsync_Should_DeleteEveryStaleAttachment_ViaAttachmentServiceAsync()
    {
        // Arrange
        var stale = new[] { BuildStaleAttachment(1, 10), BuildStaleAttachment(2, 11) };
        _mockAttachmentRepository
            .Setup(X => X.GetStaleByEntityTypeAsync(AttachmentDraftSession.AttachmentEntityType, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(stale);
        _mockDraftSessionRepository
            .Setup(X => X.GetOrphanedAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        await _service.RunAsync(CancellationToken.None);

        // Assert — the service's own safe-delete order (metadata then file) is reused as-is.
        _mockAttachmentService.Verify(X => X.DeleteAttachmentAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        _mockAttachmentService.Verify(X => X.DeleteAttachmentAsync(2, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RunAsync_Should_ComputeTheCutoff_FromTempAttachmentTtlHoursAsync()
    {
        // Arrange
        _mockAttachmentRepository
            .Setup(X => X.GetStaleByEntityTypeAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _mockDraftSessionRepository
            .Setup(X => X.GetOrphanedAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        DateTime expectedCutoff = DateTime.UtcNow.AddHours(-TtlHours);

        // Act
        await _service.RunAsync(CancellationToken.None);

        // Assert — allow a small tolerance for the time elapsed during the call itself.
        _mockAttachmentRepository.Verify(X => X.GetStaleByEntityTypeAsync(
            AttachmentDraftSession.AttachmentEntityType,
            It.Is<DateTime>(D => Math.Abs((D - expectedCutoff).TotalSeconds) < 5),
            It.IsAny<CancellationToken>()), Times.Once);
        _mockDraftSessionRepository.Verify(X => X.GetOrphanedAsync(
            It.Is<DateTime>(D => Math.Abs((D - expectedCutoff).TotalSeconds) < 5),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RunAsync_Should_ContinueSweeping_When_OneAttachmentDeleteFailsAsync()
    {
        // Arrange
        var stale = new[] { BuildStaleAttachment(1, 10), BuildStaleAttachment(2, 11) };
        _mockAttachmentRepository
            .Setup(X => X.GetStaleByEntityTypeAsync(AttachmentDraftSession.AttachmentEntityType, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(stale);
        _mockAttachmentService
            .Setup(X => X.DeleteAttachmentAsync(1, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new IOException("disk error"));
        _mockDraftSessionRepository
            .Setup(X => X.GetOrphanedAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act — must not throw despite attachment 1 failing
        await _service.RunAsync(CancellationToken.None);

        // Assert — attachment 2 is still attempted
        _mockAttachmentService.Verify(X => X.DeleteAttachmentAsync(2, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RunAsync_Should_DeleteEveryOrphanedDraftSessionAsync()
    {
        // Arrange
        _mockAttachmentRepository
            .Setup(X => X.GetStaleByEntityTypeAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        var orphaned = new[]
        {
            new AttachmentDraftSession { Id = 5, CreatedBy = "user-1" },
            new AttachmentDraftSession { Id = 6, CreatedBy = "user-1" }
        };
        _mockDraftSessionRepository
            .Setup(X => X.GetOrphanedAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(orphaned);

        // Act
        await _service.RunAsync(CancellationToken.None);

        // Assert
        _mockDraftSessionRepository.Verify(X => X.DeleteByIdAsync(5), Times.Once);
        _mockDraftSessionRepository.Verify(X => X.DeleteByIdAsync(6), Times.Once);
    }

    [Fact]
    public async Task RunAsync_Should_ContinueSweeping_When_OneSessionDeleteFailsAsync()
    {
        // Arrange
        _mockAttachmentRepository
            .Setup(X => X.GetStaleByEntityTypeAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        var orphaned = new[]
        {
            new AttachmentDraftSession { Id = 5, CreatedBy = "user-1" },
            new AttachmentDraftSession { Id = 6, CreatedBy = "user-1" }
        };
        _mockDraftSessionRepository
            .Setup(X => X.GetOrphanedAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(orphaned);
        _mockDraftSessionRepository.Setup(X => X.DeleteByIdAsync(5)).ThrowsAsync(new InvalidOperationException("db down"));

        // Act — must not throw despite session 5 failing
        await _service.RunAsync(CancellationToken.None);

        // Assert — session 6 is still attempted
        _mockDraftSessionRepository.Verify(X => X.DeleteByIdAsync(6), Times.Once);
    }

    [Fact]
    public async Task RunAsync_Should_DoNothing_When_NothingIsStaleOrOrphanedAsync()
    {
        // Arrange
        _mockAttachmentRepository
            .Setup(X => X.GetStaleByEntityTypeAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _mockDraftSessionRepository
            .Setup(X => X.GetOrphanedAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act & Assert — no throw
        await _service.RunAsync(CancellationToken.None);
        _mockAttachmentService.Verify(X => X.DeleteAttachmentAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockDraftSessionRepository.Verify(X => X.DeleteByIdAsync(It.IsAny<int>()), Times.Never);
    }
}
