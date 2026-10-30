using Eras.Api.Controllers;
using Eras.Application.DTOs.StudentInterventions;
using Eras.Application.Features.StudentInterventions;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using Moq;

using Xunit;

namespace Eras.Api.Tests.Controllers;

public class StudentInterventionsControllerTests
{
    private readonly Mock<IMediator> _mediator = new();
    private readonly StudentInterventionsController _controller;

    public StudentInterventionsControllerTests()
    {
        _controller = new StudentInterventionsController(_mediator.Object);
    }

    private static StudentInterventionDto Dto(int Id = 10) => new()
    {
        Id = Id,
        StudentId = 1,
        Kind = StudentInterventionKind.RiskIdentified,
        Type = StudentInterventionType.Academic,
        Description = "Failed two midterms",
        EntryDate = new DateOnly(2026, 10, 20),
    };

    private static SaveStudentInterventionDto Body() => new()
    {
        Kind = StudentInterventionKind.RiskIdentified,
        Type = StudentInterventionType.Academic,
        Description = "Failed two midterms",
        EntryDate = new DateOnly(2026, 10, 20),
    };

    [Fact]
    public async Task GetByStudentAsync_ReturnsOkWithTheEntriesAsync()
    {
        _mediator
            .Setup(Mediator => Mediator.Send(It.Is<GetStudentInterventionsQuery>(Query => Query.StudentId == 1), It.IsAny<CancellationToken>()))
            .ReturnsAsync([Dto()]);

        IActionResult result = await _controller.GetByStudentAsync(1, CancellationToken.None);

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result);
        Assert.Single(Assert.IsAssignableFrom<IReadOnlyList<StudentInterventionDto>>(ok.Value));
    }

    [Fact]
    public async Task CreateAsync_ReturnsCreatedWithTheEntryAsync()
    {
        _mediator
            .Setup(Mediator => Mediator.Send(It.IsAny<CreateStudentInterventionCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Dto());

        IActionResult result = await _controller.CreateAsync(1, Body(), CancellationToken.None);

        CreatedAtActionResult created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(201, created.StatusCode);
        Assert.Equal(10, Assert.IsType<StudentInterventionDto>(created.Value).Id);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsOkWithTheEntryAsync()
    {
        _mediator
            .Setup(Mediator => Mediator.Send(It.Is<UpdateStudentInterventionCommand>(Command => Command.EntryId == 10), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Dto());

        IActionResult result = await _controller.UpdateAsync(1, 10, Body(), CancellationToken.None);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsNoContentAsync()
    {
        IActionResult result = await _controller.DeleteAsync(1, 10, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
        _mediator.Verify(
            Mediator => Mediator.Send(It.Is<DeleteStudentInterventionCommand>(Command => Command.StudentId == 1 && Command.EntryId == 10), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
