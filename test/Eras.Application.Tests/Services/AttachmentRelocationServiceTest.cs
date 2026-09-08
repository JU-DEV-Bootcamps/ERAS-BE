using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Models;
using Eras.Domain.Entities;

using Eras.Application.Services;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moq;

namespace Eras.Application.Tests.Services;

public sealed class AttachmentRelocationServiceTest
{
    private readonly Mock<IAttachmentRepository> _repository = new();
    private readonly Mock<IFileStorageService> _fileStorage = new();
    private readonly IOptions<FileStorageSettings> _settings = Options.Create(new FileStorageSettings
    {
        BasePath = "unused",
        AllowedExtensions = Array.Empty<string>(),
        StorageRelocationBatchSize = 100
    });

    private AttachmentRelocationService CreateSut() =>
        new(_repository.Object, _fileStorage.Object, _settings, Mock.Of<ILogger<AttachmentRelocationService>>());

    private static Attachment MakePending(int Id, string EntityType, int EntityId, string StorageKey, DateTime PendingAt) =>
        new()
        {
            Id = Id,
            EntityType = EntityType,
            EntityId = EntityId,
            StorageKey = StorageKey,
            ContentHash = "hash",
            CreatedBy = "tester",
            StorageRelocationPendingAt = PendingAt
        };

    [Fact]
    public async Task RunAsync_Should_MoveFileAndUpdateRow_WhenSourceExistsAsync()
    {
        var attachment = MakePending(1, "interventions", 42, "Temp/7/abc.pdf", DateTime.UtcNow);
        _repository.Setup(R => R.GetPendingRelocationAsync(It.IsAny<int>()))
            .ReturnsAsync(new[] { attachment });
        _fileStorage.Setup(F => F.ExistsAsync("Temp/7/abc.pdf")).ReturnsAsync(true);

        await CreateSut().RunAsync();

        _fileStorage.Verify(F => F.MoveAsync("Temp/7/abc.pdf", "interventions/42/abc.pdf"), Times.Once);
        _repository.Verify(R => R.MarkRelocatedAsync(1, "interventions/42/abc.pdf"), Times.Once);
    }

    [Fact]
    public async Task RunAsync_Should_SkipMoveButStillMarkRelocated_WhenSourceAlreadyMissingAsync()
    {
        var attachment = MakePending(2, "interventions", 42, "Temp/7/abc.pdf", DateTime.UtcNow);
        _repository.Setup(R => R.GetPendingRelocationAsync(It.IsAny<int>()))
            .ReturnsAsync(new[] { attachment });
        _fileStorage.Setup(F => F.ExistsAsync("Temp/7/abc.pdf")).ReturnsAsync(false);

        await CreateSut().RunAsync();

        _fileStorage.Verify(F => F.MoveAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _repository.Verify(R => R.MarkRelocatedAsync(2, "interventions/42/abc.pdf"), Times.Once);
    }

    [Fact]
    public async Task RunAsync_Should_LeaveRowPending_WhenMoveThrows_AndNotAffectOtherRowsAsync()
    {
        var failing = MakePending(3, "interventions", 42, "Temp/7/fail.pdf", DateTime.UtcNow.AddMinutes(-2));
        var succeeding = MakePending(4, "interventions", 42, "Temp/7/ok.pdf", DateTime.UtcNow.AddMinutes(-1));
        _repository.Setup(R => R.GetPendingRelocationAsync(It.IsAny<int>()))
            .ReturnsAsync(new[] { failing, succeeding });
        _fileStorage.Setup(F => F.ExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
        _fileStorage.Setup(F => F.MoveAsync("Temp/7/fail.pdf", It.IsAny<string>()))
            .ThrowsAsync(new IOException("disk error"));

        await CreateSut().RunAsync();

        _repository.Verify(R => R.MarkRelocatedAsync(3, It.IsAny<string>()), Times.Never);
        _repository.Verify(R => R.MarkRelocatedAsync(4, "interventions/42/ok.pdf"), Times.Once);
    }

    [Fact]
    public async Task RunAsync_Should_DoNothing_WhenNoRowsPendingAsync()
    {
        _repository.Setup(R => R.GetPendingRelocationAsync(It.IsAny<int>()))
            .ReturnsAsync(Array.Empty<Attachment>());

        await CreateSut().RunAsync();

        _fileStorage.Verify(F => F.MoveAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _repository.Verify(R => R.MarkRelocatedAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
    }
}
