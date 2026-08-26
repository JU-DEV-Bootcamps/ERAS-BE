using System.Security.Claims;

using Eras.Api.Controllers;
using Eras.Application.Contracts.Services;
using Eras.Application.DTOs.AttachmentManagement;
using Eras.Error.Bussiness;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Api.Tests.Controllers;

public class AttachmentsControllerTests
{
    private readonly Mock<IAttachmentService> _mockAttachmentService;
    private readonly Mock<ILogger<AttachmentsController>> _mockLogger;
    private readonly AttachmentsController _controller;

    public AttachmentsControllerTests()
    {
        _mockAttachmentService = new Mock<IAttachmentService>();
        _mockLogger = new Mock<ILogger<AttachmentsController>>();
        _controller = new AttachmentsController(_mockAttachmentService.Object, _mockLogger.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(
                        [new Claim(ClaimTypes.NameIdentifier, "user-1")]))
                }
            }
        };
    }

    private static AttachmentDto BuildDto(int id = 1) => new()
    {
        Id = id,
        EntityType = "interventions",
        EntityId = 1,
        ContentHash = new string('a', 64),
        CreatedBy = "user-1"
    };

    private static Mock<IFormFile> BuildFormFile(string fileName, string content)
    {
        var mock = new Mock<IFormFile>();
        var bytes = System.Text.Encoding.UTF8.GetBytes(content);
        mock.Setup(f => f.FileName).Returns(fileName);
        mock.Setup(f => f.OpenReadStream()).Returns(() => new MemoryStream(bytes));
        return mock;
    }

    [Fact]
    public async Task Upload_Should_ReturnCreated_When_FilesProvidedAsync()
    {
        // Arrange
        var formFiles = new FormFileCollection { BuildFormFile("report.pdf", "content").Object };

        _mockAttachmentService
            .Setup(x => x.UploadAttachmentsAsync(
                "interventions", 1,
                It.Is<IReadOnlyCollection<(Stream Stream, string FileName)>>(f => f.Count == 1 && f.First().FileName == "report.pdf"),
                "user-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AttachmentDto> { BuildDto() });

        // Act
        var result = await _controller.UploadAsync("interventions", 1, formFiles, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedResult>(result.Result);
        Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
    }

    [Fact]
    public async Task Upload_Should_PassAllFilesInOneBatchCall_When_MultipleFilesProvidedAsync()
    {
        // Arrange — the fix under test: multiple files go through UploadAttachmentsAsync once,
        // not UploadAttachmentAsync looped per file.
        var formFiles = new FormFileCollection
        {
            BuildFormFile("a.pdf", "content-a").Object,
            BuildFormFile("b.pdf", "content-b").Object
        };

        _mockAttachmentService
            .Setup(x => x.UploadAttachmentsAsync(
                "interventions", 1, It.IsAny<IReadOnlyCollection<(Stream Stream, string FileName)>>(),
                "user-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AttachmentDto> { BuildDto(1), BuildDto(2) });

        // Act
        var result = await _controller.UploadAsync("interventions", 1, formFiles, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedResult>(result.Result);
        var attachments = Assert.IsAssignableFrom<IReadOnlyCollection<AttachmentDto>>(createdResult.Value);
        Assert.Equal(2, attachments.Count);
        _mockAttachmentService.Verify(
            x => x.UploadAttachmentsAsync(
                "interventions", 1, It.Is<IReadOnlyCollection<(Stream Stream, string FileName)>>(f => f.Count == 2),
                "user-1", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Upload_Should_ReturnBadRequest_When_NoFilesProvidedAsync()
    {
        // Act
        var result = await _controller.UploadAsync("interventions", 1, new FormFileCollection(), CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
        _mockAttachmentService.Verify(
            x => x.UploadAttachmentsAsync(
                It.IsAny<string>(), It.IsAny<int>(), It.IsAny<IReadOnlyCollection<(Stream Stream, string FileName)>>(),
                It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task List_Should_ReturnOkWithAttachmentsAsync()
    {
        // Arrange
        _mockAttachmentService
            .Setup(x => x.ListAttachmentsAsync("interventions", 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AttachmentDto> { BuildDto() });

        // Act
        var result = await _controller.ListAsync("interventions", 1, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var attachments = Assert.IsAssignableFrom<IReadOnlyCollection<AttachmentDto>>(okResult.Value);
        Assert.Single(attachments);
    }

    [Fact]
    public async Task Download_Should_ReturnRedirect_When_DirectUrlAvailableAsync()
    {
        // Arrange
        _mockAttachmentService
            .Setup(x => x.GetAttachmentUrlAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync("https://example.com/signed-url");

        // Act
        var result = await _controller.DownloadAsync(1, CancellationToken.None);

        // Assert
        var redirect = Assert.IsType<RedirectResult>(result);
        Assert.Equal("https://example.com/signed-url", redirect.Url);
        _mockAttachmentService.Verify(x => x.DownloadAttachmentAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Download_Should_StreamFile_When_NoDirectUrlAvailableAsync()
    {
        // Arrange
        _mockAttachmentService
            .Setup(x => x.GetAttachmentUrlAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);
        _mockAttachmentService
            .Setup(x => x.DownloadAttachmentAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new MemoryStream([1, 2, 3]), "application/pdf", "report.pdf"));

        // Act
        var result = await _controller.DownloadAsync(1, CancellationToken.None);

        // Assert
        var fileResult = Assert.IsType<FileStreamResult>(result);
        Assert.Equal("application/pdf", fileResult.ContentType);
    }

    [Fact]
    public async Task Delete_Should_ReturnNoContentAsync()
    {
        // Arrange
        _mockAttachmentService
            .Setup(x => x.DeleteAttachmentAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteAsync(1, CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_Should_PropagateNotFoundException_When_AttachmentMissingAsync()
    {
        // Arrange
        _mockAttachmentService
            .Setup(x => x.DeleteAttachmentAsync(999, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Attachment '999' not found."));

        // Act & Assert — relies on the global ErrorFilter to translate IErasException to its
        // StatusCode; the controller itself doesn't catch it (unlike AssessmentsController's
        // manual try/catch style elsewhere).
        await Assert.ThrowsAsync<NotFoundException>(() => _controller.DeleteAsync(999, CancellationToken.None));
    }
}
