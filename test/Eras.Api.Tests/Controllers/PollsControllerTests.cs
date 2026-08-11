using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Eras.Api.Controllers;
using Eras.Application.Dtos;
using Eras.Application.DTOs.Poll;
using Eras.Application.Features.Polls.Queries.GetAllByPollAndCohort;
using Eras.Application.Features.Polls.Queries.GetAllPollsQuery;
using Eras.Application.Features.Polls.Queries.GetPollsByCohort;
using Eras.Application.Features.Polls.Queries.GetPollsByStudent;
using Eras.Application.Features.Variables.Queries.GetVariablesByPollUuidAndComponent;
using Eras.Application.Models.Response.Controllers.PollsController;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Eras.Api.Tests.Controllers;

public class PollsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<PollsController>> _loggerMock;
    private readonly PollsController _controller;

    public PollsControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<PollsController>>();
        _controller = new PollsController(
            _mediatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetPollsByCohortAsync_WhenBothStudentAndCohortProvided_ReturnsBadRequestAsync()
    {
        // Act
        var result = await _controller.GetPollsByCohortAsync(CohortId: 1, StudentId: 2);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Only filter by StudentId or CohortId", badRequest.Value);

        _mediatorMock.Verify(M => M.Send(It.IsAny<IRequest<object>>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetPollsByCohortAsync_WhenNoFilters_ReturnsAllPollsAsync()
    {
        // Arrange
        var expected = new List<GetPollsQueryResponse>();

        _mediatorMock
            .Setup(M => M.Send(It.IsAny<GetAllPollsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.GetPollsByCohortAsync(0, 0);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expected, ok.Value);
        _mediatorMock.Verify(M =>
            M.Send(It.IsAny<GetAllPollsQuery>(),It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetPollsByCohortAsync_WhenStudentProvided_ReturnsStudentPollsAsync()
    {
        // Arrange
        var expected = new List<GetPollsQueryResponse>();

        _mediatorMock
            .Setup(M => M.Send(It.Is<GetPollsByStudentQuery>(Q => Q.StudentId == 5), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.GetPollsByCohortAsync(0, 5);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expected, ok.Value);
        _mediatorMock.Verify(M =>
            M.Send(It.Is<GetPollsByStudentQuery>(Q => Q.StudentId == 5), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetPollsByCohortAsync_WhenOnlyCohortProvided_ReturnsCohortPollsAsync()
    {
        // Arrange
        var expected = new List<GetPollsQueryResponse>();
        _mediatorMock
            .Setup(M => M.Send(It.Is<GetPollsByCohortListQuery>(Q => Q.CohortId == 10), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.GetPollsByCohortAsync(10, 0);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expected, ok.Value);

        _mediatorMock.Verify(M =>
            M.Send(It.Is<GetPollsByCohortListQuery>(Q => Q.CohortId == 10), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetAllPollVariableByCohortAndPollAsync_ReturnsOkAsync()
    {
        // Arrange
        var expected = new List<PollVariableDto>();

        _mediatorMock
            .Setup(M => M.Send(It.Is<GetAllByPollAndCohortQuery>(Q => Q.cohortId == 7 && Q.pollId == 3), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.GetAllPollVariableByCohortAndPollAsync(3, 7);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expected, ok.Value);
    }

    [Fact]
    public async Task GetVariablesByComponentsAsync_ReturnsVariablesAsync()
    {
        // Arrange
        var components = new List<string>{ "A", "B"};

        var expected = new List<Variable>
        {
            new Variable(),
            new Variable()
        };

        _mediatorMock
            .Setup(M => M.Send(
                It.Is<GetVariablesByPollUuidAndComponentQuery>(Q =>
                    Q.pollUuid == "poll-uuid" &&
                    Q.LastVersion &&
                    Q.component.SequenceEqual(components)),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.GetVariablesByComponentsAsync("poll-uuid", components, true);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expected, ok.Value);
        _mediatorMock.Verify(M =>
            M.Send(It.IsAny<GetVariablesByPollUuidAndComponentQuery>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}