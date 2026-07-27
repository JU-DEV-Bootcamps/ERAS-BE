using System.Security.Cryptography;
using System.Text;

using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.Features.RemissionManagement;
using Eras.Application.Features.RemissionManagement.Handlers;
using Eras.Application.Models;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moq;

namespace Eras.Application.Tests.Features.Assessments.Commands;

public class UploadInterventionAttachmentsCommandHandlerTests
{
    private readonly Mock<IAssessmentRepository> _mockRepository;
    private readonly Mock<IFileStorageService> _mockFileStorage;
    private readonly Mock<IOptions<FileStorageSettings>> _mockSettings;
    private readonly Mock<ILogger<UploadInterventionAttachmentsCommandHandler>> _mockLogger;
    private readonly UploadInterventionAttachmentsCommandHandler _handler;

    public UploadInterventionAttachmentsCommandHandlerTests()
    {
        _mockRepository = new Mock<IAssessmentRepository>();
        _mockFileStorage = new Mock<IFileStorageService>();
        _mockSettings = new Mock<IOptions<FileStorageSettings>>();
        _mockLogger = new Mock<ILogger<UploadInterventionAttachmentsCommandHandler>>();

        _mockSettings
            .Setup(x => x.Value)
            .Returns(new FileStorageSettings { AllowedExtensions = [".pdf", ".png", ".jpg"], BasePath = "" });

        _handler = new UploadInterventionAttachmentsCommandHandler(
            _mockRepository.Object,
            _mockFileStorage.Object,
            _mockSettings.Object,
            _mockLogger.Object);
    }

    private static (Stream Stream, string FileName) CreateFile(string fileName, string content)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(content);
        return (new MemoryStream(bytes), fileName);
    }

    private static string ComputeHash(string content)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(content);
        byte[] hashBytes = SHA256.HashData(bytes);
        return Convert.ToHexString(hashBytes);
    }

    [Fact]
    public async Task Handle_Should_Save_Files_And_Return_Paths_When_ValidAsync()
    {
        // Arrange
        var file1 = CreateFile("report.pdf", "content-1");
        var file2 = CreateFile("photo.png", "content-2");

        var command = new UploadInterventionAttachmentsCommand(1, [file1, file2]);

        _mockRepository
            .Setup(x => x.GetAttachmentHashesAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyCollection<string>)new List<string>());

        _mockFileStorage
            .Setup(x => x.SaveAsync(It.IsAny<Stream>(), "report.pdf", $"interventions/1"))
            .ReturnsAsync($"interventions/1/report.pdf");

        _mockFileStorage
            .Setup(x => x.SaveAsync(It.IsAny<Stream>(), "photo.png", $"interventions/1"))
            .ReturnsAsync($"interventions/1/photo.png");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains($"interventions/1/report.pdf", result);
        Assert.Contains($"interventions/1/photo.png", result);

        _mockFileStorage.Verify(
            x => x.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>(), $"interventions/1"),
            Times.Exactly(2));

        _mockRepository.Verify(
            x => x.AddAttachmentsAsync(
                1,
                It.Is<IReadOnlyCollection<string>>(paths => paths.Count == 2),
                It.Is<IReadOnlyCollection<string>>(hashes => hashes.Count == 2)),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Skip_Duplicate_Files_Based_On_HashAsync()
    {
        // Arrange
        var interventionId = 2;
        var duplicateContent = "duplicate-content";
        var newContent = "new-content";

        var duplicateFile = CreateFile("duplicate.pdf", duplicateContent);
        var newFile = CreateFile("new.pdf", newContent);

        var command = new UploadInterventionAttachmentsCommand(interventionId, [duplicateFile, newFile]);

        var existingHashes = new List<string> { ComputeHash(duplicateContent) };

        _mockRepository
            .Setup(x => x.GetAttachmentHashesAsync(interventionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyCollection<string>)existingHashes);

        _mockFileStorage
            .Setup(x => x.SaveAsync(It.IsAny<Stream>(), "new.pdf", $"interventions/{interventionId}"))
            .ReturnsAsync($"interventions/{interventionId}/new.pdf");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Contains($"interventions/{interventionId}/new.pdf", result);

        _mockFileStorage.Verify(
            x => x.SaveAsync(It.IsAny<Stream>(), "duplicate.pdf", It.IsAny<string>()),
            Times.Never);

        _mockFileStorage.Verify(
            x => x.SaveAsync(It.IsAny<Stream>(), "new.pdf", $"interventions/{interventionId}"),
            Times.Once);

        _mockRepository.Verify(
            x => x.AddAttachmentsAsync(
                interventionId,
                It.Is<IReadOnlyCollection<string>>(paths => paths.Count == 1),
                It.Is<IReadOnlyCollection<string>>(hashes => hashes.Count == 1)),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_InvalidOperationException_When_Extension_Not_AllowedAsync()
    {
        // Arrange
        var interventionId = 1;
        var file = CreateFile("notallowed.exe", "content");

        // Act
        var command = new UploadInterventionAttachmentsCommand(interventionId, [file]);

        // Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Extension '.exe' is not allowed.", exception.Message);

        _mockRepository.Verify(
            x => x.GetAttachmentHashesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _mockFileStorage.Verify(
            x => x.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_InvalidOperationException_When_Files_Already_UploadedAsync()
    {
        // Arrange
        var interventionId = 1;
        var content = "already-uploaded-content";
        var file = CreateFile("existing.pdf", content);
        var command = new UploadInterventionAttachmentsCommand(interventionId, [file]);

        var existingHashes = new List<string> { ComputeHash(content) };

        // Act
        _mockRepository
            .Setup(x => x.GetAttachmentHashesAsync(interventionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyCollection<string>)existingHashes);

        // Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));

        Assert.Equal("All files have already been uploaded to this intervention.", exception.Message);

        _mockFileStorage.Verify(
            x => x.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);

        _mockRepository.Verify(
            x => x.AddAttachmentsAsync(
                It.IsAny<int>(),
                It.IsAny<IReadOnlyCollection<string>>(),
                It.IsAny<IReadOnlyCollection<string>>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Return_Empty_When_No_Files_ProvidedAsync()
    {
        // Arrange
        var interventionId = 1;
        var command = new UploadInterventionAttachmentsCommand(interventionId, []);

        _mockRepository
            .Setup(x => x.GetAttachmentHashesAsync(interventionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyCollection<string>)new List<string>());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Empty(result);

        _mockFileStorage.Verify(
            x => x.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);

        _mockRepository.Verify(
            x => x.AddAttachmentsAsync(
                It.IsAny<int>(),
                It.IsAny<IReadOnlyCollection<string>>(),
                It.IsAny<IReadOnlyCollection<string>>()),
            Times.Never);
    }
}
