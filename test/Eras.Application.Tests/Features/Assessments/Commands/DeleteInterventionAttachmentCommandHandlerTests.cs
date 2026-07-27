using System.Net.Mail;

using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.Features.RemissionManagement;
using Eras.Application.Features.RemissionManagement.Handlers;
using Eras.Domain.Entities.AssessmentManagement;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.Assessments.Commands;

public class DeleteInterventionAttachmentCommandHandlerTests
{
    private readonly Mock<IAssessmentRepository> _mockRepository;
    private readonly Mock<IFileStorageService> _mockFileStorage;
    private readonly Mock<ILogger<DeleteInterventionAttachmentCommandHandler>> _mockLogger;
    private readonly DeleteInterventionAttachmentCommandHandler _handler;

    public DeleteInterventionAttachmentCommandHandlerTests()
    {
        _mockRepository = new Mock<IAssessmentRepository>();
        _mockLogger = new Mock<ILogger<DeleteInterventionAttachmentCommandHandler>>();
        _mockFileStorage = new Mock<IFileStorageService>();
        _handler = new DeleteInterventionAttachmentCommandHandler(
            _mockRepository.Object,
            _mockFileStorage.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_Should_Delete_File_When_Is_ValidAsync()
    {
        // Arrange
        var command = new DeleteInterventionAttachmentCommand(1, "report.pdf");
        var intervention = new Intervention
        {
            DateUtc = DateTime.UtcNow,
            StudentIds = [1, 2],
            Mode = InterventionMode.InPlace,
            Attachments = new List<string> { "interventions/1/report.pdf" }.AsReadOnly(),
            AttachmentHashes = new List<string> { "HASH1" }.AsReadOnly()
        };
        var interventionExpected = new Intervention
        {
            DateUtc = DateTime.UtcNow,
            StudentIds = [1, 2],
            Mode = InterventionMode.InPlace,
            Attachments = new List<string>().AsReadOnly(),
            AttachmentHashes = new List<string> ().AsReadOnly()
        };
        _mockRepository
            .Setup(x => x.GetInterventionByIdAsync(1))
            .ReturnsAsync(intervention);

        _mockFileStorage
            .Setup(x => x.DeleteAsync("interventions/1/report.pdf"));

        _mockRepository
            .Setup(x => x.RemoveAttachmentAsync(1, "interventions/1/report.pdf"));

        //Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockRepository.Verify(
            x => x.GetInterventionByIdAsync(1),
            Times.Once);

        _mockFileStorage.Verify(
            x => x.DeleteAsync("interventions/1/report.pdf"),
            Times.Exactly(1));

        _mockRepository.Verify(
            x => x.RemoveAttachmentAsync(1, "interventions/1/report.pdf"),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_Exception_When_Intervention_Is_NullAsync()
    {
        // Arrange
        var command = new DeleteInterventionAttachmentCommand(1, "report.pdf");

        // Act
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(command, CancellationToken.None));

        // Assert
        Assert.Equal("Intervention '1' not found.", exception.Message);

        _mockRepository.Verify(
            x => x.GetInterventionByIdAsync(1),
            Times.Once);

        _mockFileStorage.Verify(
            x => x.DeleteAsync("interventions/1/report.pdf"),
            Times.Never);

        _mockRepository.Verify(
            x => x.RemoveAttachmentAsync(1, "interventions/1/report.pdf"),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_Exception_When_Attachment_Not_ExistsAsync()
    {
        // Arrange
        var command = new DeleteInterventionAttachmentCommand(1, "report.pdf");
        var intervention = new Intervention
        {
            DateUtc = DateTime.UtcNow,
            StudentIds = [1, 2],
            Mode = InterventionMode.InPlace,
            Attachments = new List<string>().AsReadOnly(),
            AttachmentHashes = new List<string>().AsReadOnly()
        };
        
        _mockRepository
            .Setup(x => x.GetInterventionByIdAsync(1))
            .ReturnsAsync(intervention);

        // Act
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(command, CancellationToken.None));

        // Assert
        Assert.Equal("Attachment 'report.pdf' not found in intervention '1'.", exception.Message);
        _mockRepository.Verify(
            x => x.GetInterventionByIdAsync(1),
            Times.Once);

        _mockFileStorage.Verify(
            x => x.DeleteAsync("interventions/1/report.pdf"),
            Times.Never);

        _mockRepository.Verify(
            x => x.RemoveAttachmentAsync(1, "interventions/1/report.pdf"),
            Times.Never);
    }
}
