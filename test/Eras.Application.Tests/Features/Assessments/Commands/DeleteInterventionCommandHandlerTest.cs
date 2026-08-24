using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.Features.RemissionManagement;
using Eras.Application.Features.RemissionManagement.Handlers;
using Eras.Domain.Entities.AssessmentManagement;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Features.Assessments.Commands;

public class DeleteInterventionCommandHandlerTests
{
    private readonly Mock<IAssessmentRepository> _repository = new();
    private readonly Mock<IFileStorageService> _fileStorage = new();
    private readonly Mock<ILogger<DeleteInterventionCommandHandler>> _logger = new();

    private DeleteInterventionCommandHandler CreateHandler() =>
        new(_repository.Object, _fileStorage.Object, _logger.Object);

    [Fact]
    public async Task Handle_WhenInterventionDoesNotExist_ThrowsKeyNotFoundException()
    {
        // Arrange
        var interventionId = 1;
        var assessmentId = 42;

        _repository
            .Setup(x => x.GetInterventionByIdAsync(interventionId))
            .ReturnsAsync((Intervention?)null);

        var handler = CreateHandler();
        var command = new DeleteInterventionCommand(assessmentId, interventionId);

        // Act
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));

        // Assert
        Assert.Equal(
            $"Intervention '{interventionId}' not found for assessment '{assessmentId}'",
            exception.Message);

        _repository.Verify(
            x => x.DeleteInterventionAsync(It.IsAny<int>(), It.IsAny<int>()),
            Times.Never);

        _fileStorage.Verify(
            x => x.DeleteAsync(It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenInterventionIsRemitted_DeletesIntervention()
    {
        // Arrange
        var interventionId = 1;
        var assessmentId = 2;

        var intervention = new Intervention
        {
            DateUtc = DateTime.UtcNow,
            StudentIds = [1],
            Status = InterventionStatus.Remitted,
            Attachments = new List<string>
            {
                "attachment-1.pdf",
                "attachment-2.pdf"
            }
        };

        _repository
            .Setup(x => x.GetInterventionByIdAsync(interventionId))
            .ReturnsAsync(intervention);

        var handler = CreateHandler();
        var command = new DeleteInterventionCommand(assessmentId, interventionId);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _fileStorage.Verify(
            x => x.DeleteAsync(It.IsAny<string>()),
            Times.Never);

        _repository.Verify(
            x => x.DeleteInterventionAsync(assessmentId, interventionId),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenInterventionIsNotRemitted_DoesNotDeleteIntervention_AndDeletesAttachments()
    {
        // Arrange
        var interventionId = 1;
        var assessmentId = 1;

        var attachments = new[]
        {
            "attachment-1.pdf",
            "attachment-2.pdf"
        };

        var intervention = new Intervention
        {
            DateUtc = DateTime.UtcNow,
            StudentIds = [1],
            Status = InterventionStatus.Finalized,
            Attachments = attachments
        };

        _repository
            .Setup(x => x.GetInterventionByIdAsync(interventionId))
            .ReturnsAsync(intervention);

        var handler = CreateHandler();
        var command = new DeleteInterventionCommand(assessmentId, interventionId);

        // Act
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));

        // Assert
        Assert.Equal(
            $"Intervention '{interventionId}' not found for assessment '{assessmentId}'", exception.Message);

        foreach (var attachment in attachments)
        {
            _fileStorage.Verify(x => x.DeleteAsync(attachment), Times.Once);
        }

        _repository.Verify(
            x => x.DeleteInterventionAsync(It.IsAny<int>(), It.IsAny<int>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenMultipleAttachmentsExist_DeletesEachAttachment()
    {
        // Arrange
        var interventionId = 11;
        var assessmentId = 12;

        var intervention = new Intervention
        {
            DateUtc = DateTime.UtcNow,
            StudentIds = [12],
            Status = InterventionStatus.InProgress,
            Attachments = new List<string>
            {
                "one.pdf",
                "two.pdf",
                "three.pdf"
            }
        };

        _repository
            .Setup(x => x.GetInterventionByIdAsync(interventionId))
            .ReturnsAsync(intervention);

        var handler = CreateHandler();
        var command = new DeleteInterventionCommand(assessmentId, interventionId);

        // Act
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));

        // Assert
        _fileStorage.Verify(x => x.DeleteAsync("one.pdf"), Times.Once);
        _fileStorage.Verify(x => x.DeleteAsync("two.pdf"), Times.Once);
        _fileStorage.Verify(x => x.DeleteAsync("three.pdf"), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenAttachmentDeletionThrowsNonFileNotFoundException_PropagatesException()
    {
        // Arrange
        var interventionId = 1;
        var assessmentId = 1;

        var intervention = new Intervention
        {
            DateUtc = DateTime.UtcNow,
            StudentIds = [12],
            Status = InterventionStatus.Finalized,
            Attachments = new List<string> { "attachment.pdf" }
        };

        var expectedException = new IOException("Storage unavailable.");

        _repository
            .Setup(x => x.GetInterventionByIdAsync(interventionId))
            .ReturnsAsync(intervention);

        _fileStorage
            .Setup(x => x.DeleteAsync("attachment.pdf"))
            .ThrowsAsync(expectedException);

        var handler = CreateHandler();
        var command = new DeleteInterventionCommand(assessmentId, interventionId);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<IOException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Same(expectedException, exception);

        _repository.Verify(
            x => x.DeleteInterventionAsync(It.IsAny<int>(), It.IsAny<int>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenRemitted_DeletesRepositoryRecordWithCorrectIds()
    {
        // Arrange
        var interventionId = 1;
        var assessmentId = 1;

        var intervention = new Intervention
        {
            DateUtc = DateTime.UtcNow,
            StudentIds = [200],
            Status = InterventionStatus.Remitted,
            Attachments = new List<string>()
        };

        _repository
            .Setup(x => x.GetInterventionByIdAsync(interventionId))
            .ReturnsAsync(intervention);

        var handler = CreateHandler();
        var command = new DeleteInterventionCommand(assessmentId, interventionId);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _repository.Verify(
            x => x.DeleteInterventionAsync(assessmentId, interventionId), Times.Once);
    }
}
