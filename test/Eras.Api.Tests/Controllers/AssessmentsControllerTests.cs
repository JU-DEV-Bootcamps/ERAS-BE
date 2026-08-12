using Eras.Api.Controllers.AssessmentManagement;
using Eras.Application.Contracts.Infrastructure;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Features.RemissionManagement;
using Eras.Application.Models;
using Eras.Domain.Entities.AssessmentManagement;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moq;

namespace Eras.Api.Tests.Controllers;

public class AssessmentsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IFileStorageService> _storageMock;
    private readonly AssessmentsController _controller;

    public AssessmentsControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _storageMock = new Mock<IFileStorageService>();
        IOptions<FileStorageSettings> options = Options.Create(new FileStorageSettings() { AllowedExtensions = [], BasePath = "" });

        _controller = new AssessmentsController(
            _mediatorMock.Object,
            options,
            _storageMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkAsync()
    {
        var dto = new AssessmentDto
        {
            Id = 1,
            CreatedBy = "",
            Status = AssessmentStatus.Remitted,
            Service = "",
            StudentIds = [1, 2],
        };
        var response = new List<AssessmentDto> { dto };
        _mediatorMock
            .Setup(X => X.Send(It.IsAny<GetAllRemissionsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _controller.GetAll(CancellationToken.None);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetById_ReturnsOkAsync()
    {
        var dto = new AssessmentDto { 
            Id = 1, 
            CreatedBy = "",
            Status = AssessmentStatus.Remitted,
            Service = "",
            StudentIds = [1, 2],
        };

        _mediatorMock
            .Setup(X => X.Send(It.IsAny<GetRemissionByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        var result = await _controller.GetById(1, CancellationToken.None);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenNullAsync()
    {
        _mediatorMock
            .Setup(X => X.Send(It.IsAny<GetRemissionByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AssessmentDto?)null);

        var result = await _controller.GetById(1, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetByStudentId_ReturnsOkAsync()
    {
        var dto = new List<AssessmentDto>{
            new AssessmentDto {
                Id = 1,
                CreatedBy = "",
                Status = AssessmentStatus.Remitted,
                Service = "",
                StudentIds = [1, 2],
            }
        };
        _mediatorMock
            .Setup(X => X.Send(It.IsAny<GetRemissionsByStudentIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        var result = await _controller.GetByStudentId(1, CancellationToken.None);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetByStudentId_ReturnsNotFound_WhenNullAsync()
    {
        _mediatorMock
            .Setup(X => X.Send(It.IsAny<GetRemissionsByStudentIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(It.IsAny<List<AssessmentDto>>());

        var result = await _controller.GetById(1, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetByStatus_ReturnsOkAsync()
    {
        _mediatorMock
            .Setup(X => X.Send(It.IsAny<GetRemissionsByStatusQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AssessmentDto>());

        var result = await _controller.GetByStatus("Remitted", CancellationToken.None);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetByStatus_ReturnsNotFound_WhenNullAsync()
    {
        var result = await _controller.GetByStatus("StatusIncorrect", CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Create_ReturnsOkAsync()
    {
        var dto = new AssessmentDto {
            Id = 1,
            CreatedBy = "",
            Status = AssessmentStatus.Remitted,
            Service = "",
            StudentIds = [1, 2],
        };
        _mediatorMock
            .Setup(X => X.Send(It.IsAny<CreateRemissionCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        var result = await _controller.Create(dto, CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(AssessmentsController.GetById), created.ActionName);
    }

    [Fact]
    public async Task Update_ReturnsOkAsync()
    {
        var dto = new AssessmentDto
        {
            Id = 1,
            CreatedBy = "",
            Status = AssessmentStatus.Remitted,
            Service = "",
            StudentIds = [1, 2],
        };
        _mediatorMock
            .Setup(X => X.Send(It.IsAny<UpdateRemissionCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        var result = await _controller.Update(5, dto, CancellationToken.None);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenNullResponseAsync()
    {
        var dto = new AssessmentDto
        {
            Id = default,
            CreatedBy = "",
            Status = AssessmentStatus.Remitted,
            Service = "",
            StudentIds = [1, 2],
        };
        _mediatorMock
            .Setup(X => X.Send(It.IsAny<UpdateRemissionCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(It.IsAny<AssessmentDto>());

        var result = await _controller.Update(1, dto, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Update_ReturnsConflictResponseAsync()
    {
        var dto = new AssessmentDto
        {
            Id = 1,
            CreatedBy = "",
            Status = AssessmentStatus.Remitted,
            Service = "",
            StudentIds = [1, 2],
        };
        _mediatorMock
            .Setup(X => X.Send(It.IsAny<UpdateRemissionCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException("busy"));
        var result = await _controller.Update(1, dto, CancellationToken.None);

        Assert.IsType<ConflictObjectResult>(result.Result);
    }

    [Fact]
    public async Task Delete_ReturnsNoContentResultAsync()
    {
        _mediatorMock
            .Setup(X => X.Send(It.IsAny<DeleteAssessmentCommand>(), It.IsAny<CancellationToken>()));

        var result = await _controller.Delete(1, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_FailsAndReturnsNotFoundAsync()
    {
        _mediatorMock
            .Setup(X => X.Send(It.IsAny<DeleteAssessmentCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException());
        var result = await _controller.Delete(1, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetInterventions_ReturnsOkAsync()
    {
        _mediatorMock
            .Setup(X => X.Send(It.IsAny<GetInterventionsByAssessmentQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(It.IsAny<List<InterventionDto>>);

        var result = await _controller.GetInterventions(1, CancellationToken.None);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task AddIntervention_ReturnsCreatedAsync()
    {
        var dto = new AddInterventionDto
        {
            AssessmentId = 1,
            Intervention = new IndividualInterventionDto() { DateUtc = DateTime.Now, StudentIds = [] },
        };
        _mediatorMock
            .Setup(X => X.Send(It.IsAny<AddInterventionCommand>(), It.IsAny<CancellationToken>()));

        var result = await _controller.AddIntervention(dto, CancellationToken.None);

        Assert.IsType<CreatedResult>(result.Result);
    }

    [Fact]
    public async Task UpsertInterventions_ReturnsConflictResponseAsync()
    { 
        _mediatorMock
            .Setup(X => X.Send(It.IsAny<UpsertInterventionsCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException("busy"));
        var result = await _controller.UpsertInterventions(1, It.IsAny<List<InterventionDto>>(), CancellationToken.None);

        Assert.IsType<ConflictObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpsertInterventions_ReturnsOkResponseAsync()
    {
        _mediatorMock
            .Setup(X => X.Send(It.IsAny<UpsertInterventionsCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(It.IsAny<List<InterventionDto>>());
        var result = await _controller.UpsertInterventions(1, It.IsAny<List<InterventionDto>>(), CancellationToken.None);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task DeleteIntervention_ReturnsNoContentResultAsync()
    {
        _mediatorMock
            .Setup(X => X.Send(It.IsAny<DeleteInterventionCommand>(), It.IsAny<CancellationToken>()));

        var result = await _controller.DeleteIntervention(1, 1, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteIntervention_FailsAndReturnsNotFoundAsync()
    {
        _mediatorMock
            .Setup(X => X.Send(It.IsAny<DeleteInterventionCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException());
        var result = await _controller.DeleteIntervention(1, 1, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UploadAttachments_FailsAndReturnsNotFoundAsync()
    {
        var files = new FormFileCollection();
        var result = await _controller.UploadAttachments(1, files, CancellationToken.None);
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task UploadAttachments_ReturnsOkAsync()
    {
        var stream = new MemoryStream(new byte[] { 1, 2, 3 });
        IFormFile file = new FormFile(stream, 0, stream.Length, "file", "test.pdf");
        var files = new FormFileCollection{file};

        _mediatorMock
            .Setup(X => X.Send(It.IsAny<UploadInterventionAttachmentsCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string> { "test.pdf" });

        var result = await _controller.UploadAttachments(1, files, CancellationToken.None);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task UploadAttachments_FailsAndReturnsConflictAsync()
    {
        var stream = new MemoryStream(new byte[] { 1, 2, 3 });
        IFormFile file = new FormFile(stream, 0, stream.Length, "file", "test.pdf");
        var files = new FormFileCollection { file };
        _mediatorMock
            .Setup(X => X.Send(It.IsAny<UploadInterventionAttachmentsCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("exists"));
        var result = await _controller.UploadAttachments(1, files, CancellationToken.None);

        Assert.IsType<ConflictObjectResult>(result.Result);
    }

    [Fact]
    public async Task DownloadAttachment_WithUnrecognizedExtensionAsync()
    {
        _storageMock
            .Setup(X => X.ReadAsync(It.IsAny<string>()))
            .ReturnsAsync(new MemoryStream());
        var result = await _controller.DownloadAttachment(1, "file.xyz", CancellationToken.None);

        var file = Assert.IsType<FileStreamResult>(result);

        Assert.Equal("application/octet-stream", file.ContentType);
    }

    [Fact]
    public async Task DownloadAttachment_ReturnsOkAsync()
    {
        _storageMock
            .Setup(X => X.ReadAsync(It.IsAny<string>()))
            .ReturnsAsync(new MemoryStream());
        var result = await _controller.DownloadAttachment(1, "test.pdf", CancellationToken.None);
        var file = Assert.IsType<FileStreamResult>(result);

        Assert.Equal("application/pdf", file.ContentType); var stream = new MemoryStream(new byte[] { 1, 2, 3 });
    }

    [Fact]
    public async Task DownloadAttachment_FailsAndReturnsConflictAsync()
    {
        _storageMock
            .Setup(X => X.ReadAsync(It.IsAny<string>()))
            .ThrowsAsync(new FileNotFoundException());

        var result = await _controller.DownloadAttachment(1, "missing.pdf", CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteAttachment_ReturnsOkAsync()
    {
        _mediatorMock
            .Setup(X => X.Send(It.IsAny<DeleteInterventionAttachmentCommand>(), It.IsAny<CancellationToken>()));
        var result = await _controller.DeleteAttachment(1, "test.pdf", CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteAttachment_FailsAndReturnsNotFoundAsync()
    {
        _mediatorMock
            .Setup(X => X.Send(It.IsAny<DeleteInterventionAttachmentCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException());

        var result = await _controller.DeleteAttachment(1, "missing.pdf", CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
    }
}
