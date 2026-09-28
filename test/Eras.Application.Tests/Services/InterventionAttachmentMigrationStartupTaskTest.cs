using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Services;
using Eras.Application.Services;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Services;

/// <summary>
/// This orchestration (completion-marker gating, when to mark done) previously lived inline in
/// Program.cs, where it wasn't unit-testable at all. Extracting it into its own class was
/// specifically so this policy could be verified directly, independent of
/// InterventionAttachmentMigrationService's own migration-algorithm tests.
/// </summary>
public class InterventionAttachmentMigrationStartupTaskTest
{
    private readonly Mock<IInterventionAttachmentMigrationService> _mockMigrationService;
    private readonly Mock<IDataMigrationCompletionRepository> _mockCompletionRepository;
    private readonly InterventionAttachmentMigrationStartupTask _task;

    public InterventionAttachmentMigrationStartupTaskTest()
    {
        _mockMigrationService = new Mock<IInterventionAttachmentMigrationService>();
        _mockCompletionRepository = new Mock<IDataMigrationCompletionRepository>();
        _task = new InterventionAttachmentMigrationStartupTask(
            _mockMigrationService.Object,
            _mockCompletionRepository.Object,
            Mock.Of<ILogger<InterventionAttachmentMigrationStartupTask>>());
    }

    private static InterventionAttachmentMigrationResult BuildValidResult() => new()
    {
        InterventionsProcessed = 3,
        AttachmentsCreated = 5
    };

    [Fact]
    public async Task RunAsync_Should_SkipMigration_When_AlreadyCompletedAsync()
    {
        // Arrange
        _mockCompletionRepository
            .Setup(x => x.IsCompletedAsync(InterventionAttachmentMigrationService.MigrationName))
            .ReturnsAsync(true);

        // Act
        await _task.RunAsync();

        // Assert
        _mockMigrationService.Verify(x => x.MigrateAsync(It.IsAny<CancellationToken>()), Times.Never);
        _mockCompletionRepository.Verify(x => x.MarkCompletedAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task RunAsync_Should_RunMigration_When_NotYetCompletedAsync()
    {
        // Arrange
        _mockCompletionRepository
            .Setup(x => x.IsCompletedAsync(InterventionAttachmentMigrationService.MigrationName))
            .ReturnsAsync(false);
        _mockMigrationService
            .Setup(x => x.MigrateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(BuildValidResult());

        // Act
        await _task.RunAsync();

        // Assert
        _mockMigrationService.Verify(x => x.MigrateAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RunAsync_Should_MarkCompleted_When_ResultIsValidAsync()
    {
        // Arrange
        _mockCompletionRepository
            .Setup(x => x.IsCompletedAsync(InterventionAttachmentMigrationService.MigrationName))
            .ReturnsAsync(false);
        _mockMigrationService
            .Setup(x => x.MigrateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(BuildValidResult());

        // Act
        await _task.RunAsync();

        // Assert
        _mockCompletionRepository.Verify(
            x => x.MarkCompletedAsync(InterventionAttachmentMigrationService.MigrationName), Times.Once);
    }

    [Fact]
    public async Task RunAsync_Should_NotMarkCompleted_When_ResultHasMismatchedArraysAsync()
    {
        // Arrange — a corrupt-data intervention was skipped, so the run is not fully valid
        _mockCompletionRepository
            .Setup(x => x.IsCompletedAsync(InterventionAttachmentMigrationService.MigrationName))
            .ReturnsAsync(false);
        _mockMigrationService
            .Setup(x => x.MigrateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InterventionAttachmentMigrationResult
            {
                InterventionsSkippedDueToMismatchedArrays = [7]
            });

        // Act
        await _task.RunAsync();

        // Assert
        _mockCompletionRepository.Verify(x => x.MarkCompletedAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task RunAsync_Should_NotMarkCompleted_When_ResultHasValidationFailuresAsync()
    {
        // Arrange — projected count didn't match the expected count for some intervention
        _mockCompletionRepository
            .Setup(x => x.IsCompletedAsync(InterventionAttachmentMigrationService.MigrationName))
            .ReturnsAsync(false);
        _mockMigrationService
            .Setup(x => x.MigrateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InterventionAttachmentMigrationResult
            {
                ValidationFailures = [new InterventionAttachmentMigrationValidationFailure(9, 2, 1)]
            });

        // Act
        await _task.RunAsync();

        // Assert
        _mockCompletionRepository.Verify(x => x.MarkCompletedAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task RunAsync_Should_PropagateTheGivenCancellationTokenAsync()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        _mockCompletionRepository
            .Setup(x => x.IsCompletedAsync(InterventionAttachmentMigrationService.MigrationName))
            .ReturnsAsync(false);
        _mockMigrationService
            .Setup(x => x.MigrateAsync(cts.Token))
            .ReturnsAsync(BuildValidResult());

        // Act
        await _task.RunAsync(cts.Token);

        // Assert
        _mockMigrationService.Verify(x => x.MigrateAsync(cts.Token), Times.Once);
    }
}
