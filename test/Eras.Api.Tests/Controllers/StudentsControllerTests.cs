using Eras.Api.Controllers;
using Eras.Application.DTOs;
using Eras.Application.DTOs.Student;
using Eras.Application.Features.Answers.Queries;
using Eras.Application.Features.Cohorts.Queries.GetCohortStudentsRiskByPoll;
using Eras.Application.Features.Cohorts.Queries.GetCohortTopRiskStudents;
using Eras.Application.Features.Cohorts.Queries.GetCohortTopRiskStudentsByComponent;
using Eras.Application.Features.Students.Commands.CreateStudent;
using Eras.Application.Features.Students.Queries.GetAll;
using Eras.Application.Features.Students.Queries.GetAllAverageRiskByCohorAndPoll;
using Eras.Application.Features.Students.Queries.GetAllByPollAndDate;
using Eras.Application.Features.Students.Queries.GetStudentDetails;
using Eras.Application.Models.Response.Calculations;
using Eras.Application.Models.Response.Common;
using Eras.Application.Models.Response.Controllers.StudentsController;
using Eras.Application.Utils;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Eras.Api.Tests.Controllers;

public class StudentsControllerTests
{
    private readonly Mock<IMediator> _mediator = new();
    private readonly Mock<ILogger<StudentsController>> _logger = new();
    private readonly StudentsController _controller;

    public StudentsControllerTests()
    {
        _mediator = new Mock<IMediator>();
        _logger = new Mock<ILogger<StudentsController>>();
        _controller = new StudentsController(
            _mediator.Object,
            _logger.Object);
    }

    [Fact]
    public async Task ImportStudentsAsync_ReturnsBadRequest_WhenStudentsIsNullAsync()
    {
        var result = await _controller.ImportStudentsAsync(null);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("No Students body found", badRequest.Value);
    }

    [Fact]
    public async Task ImportStudentsAsync_ReturnsOk_WhenImportSucceedsAsync()
    {
        var dto = new StudentImportDto()
        {
            Name = "S",
            Email = "s@mail.com",
            SISId = "120,"
        };
        var response = new CreateCommandResponse<Student[]>(null, "Success", true);

        _mediator
            .Setup(x => x.Send(It.IsAny<CreateStudentsCommand>(), default))
            .ReturnsAsync(response);

        var result = await _controller.ImportStudentsAsync([dto]);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task ImportStudentsAsync_Returns400_WhenImportFailsAsync()
    {
        var students = new StudentImportDto()
        {
            Name = "S",
            Email = "s@mail.com",
            SISId = "120,"
        };
        var response = new CreateCommandResponse<Student[]>(null, "Error", false);

        _mediator
            .Setup(x => x.Send(It.IsAny<CreateStudentsCommand>(), default))
            .ReturnsAsync(response);

        var result = await _controller.ImportStudentsAsync([students]);

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(400, objectResult.StatusCode);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOkAsync()
    {
        var pagination = new Pagination();
        var response = new PagedResult<GetAllStudentsQueryResponse>(Items:
            [new GetAllStudentsQueryResponse()],
            Count: 1
        );
        _mediator
            .Setup(x => x.Send(It.IsAny<GetAllStudentsQuery>(), default))
            .ReturnsAsync(response);

        var result = await _controller.GetAllAsync(pagination);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetStudentDetailsByIdAsync_ReturnsOk_WhenStudentExistsAsync()
    {
        var student = new Student()
        {
            Id = 1,
        };
        var response = new CreateCommandResponse<Student>(student, "Success", true);
        _mediator
            .Setup(x => x.Send(It.IsAny<GetStudentDetailsQuery>(), default))
            .ReturnsAsync(response);

        var result = await _controller.GetStudentDetailsByIdAsync(1);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetStudentDetailsByIdAsync_ReturnsNotFound_WhenStudentDoesNotExistAsync()
    {
        var response = new CreateCommandResponse<Student>(null, "Error", false);
        _mediator
            .Setup(x => x.Send(It.IsAny<GetStudentDetailsQuery>(), default))
            .ReturnsAsync(response);

        var result = await _controller.GetStudentDetailsByIdAsync(1);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetPreviewPollsAsync_ReturnsOkAsync()
    {
        var pagination = new Pagination();
        var response = new PagedResult<Student>(Items: [new Student()], Count: 1);
        _mediator
            .Setup(x => x.Send(It.IsAny<GetAllStudentsByPollUuidAndDaysQuery>(), default))
            .ReturnsAsync(response);

        var result = await _controller.GetPreviewPollsAsync(pagination, Guid.NewGuid().ToString(), 30);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetAllAvgRiskByCohortAndPollAsync_ReturnsOkAsync()
    {
        var pagination = new Pagination();
        var response = new PagedResult<StudentAverageRiskDto>(Items: [new StudentAverageRiskDto()], Count: 1);
        _mediator
            .Setup(x => x.Send(It.IsAny<GetAllAverageRiskByCohortAndPollQuery>(), default))
            .ReturnsAsync(response);

        var result = await _controller.GetAllAvgRiskByCohortAndPollAsync(
            "1,2",
            Guid.NewGuid().ToString(),
            pagination,
            true,
            1);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetPollRiskSumStudentsAsync_ReturnsOkAsync()
    {
        _mediator
            .Setup(x => x.Send(It.IsAny<GetCohortStudentsRiskByPollQuery>(), default))
            .ReturnsAsync(new List<GetCohortStudentsRiskByPollResponse>());

        var result = await _controller.GetPollRiskSumStudentsAsync(
            Guid.NewGuid().ToString(),
            1);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetPollTopStudentsAsync_ReturnsOkAsync()
    {
        var response = new GetQueryResponse<PagedResult<GetCohortTopRiskStudentsByComponentResponse>>(
            null, "Success", true);

        _mediator
            .Setup(x => x.Send(It.IsAny<GetCohortTopRiskStudentsQuery>(), default))
            .ReturnsAsync(response);

        var result = await _controller.GetPollTopStudentsAsync(
            Guid.NewGuid().ToString(),
            1,
            true,
            new Pagination());

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetComponentTopStudentsAsync_ReturnsOkAsync()
    {
        var response = new GetQueryResponse<PagedResult<GetCohortTopRiskStudentsByComponentResponse>>(
            null, "Success", true);
        _mediator
            .Setup(x => x.Send(It.IsAny<GetCohortTopRiskStudentsByComponentQuery>(), default))
            .ReturnsAsync(response);

        var result = await _controller.GetComponentTopStudentsAsync(
            Guid.NewGuid().ToString(),
            "Mental Health",
            1,
            true,
            new Pagination());

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetStudentAnswersByPollAsync_ReturnsOkAsync()
    {
        var response = new PagedResult<StudentAnswer>(Items: null, Count: 0);
        _mediator
            .Setup(x => x.Send(It.IsAny<GetStudentAnswersByPollQuery>(), default))
            .ReturnsAsync(response);

        var result = await _controller.GetStudentAnswersByPollAsync(1, 2, new Pagination());

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetStudentAnswersByPollAsync_ReturnsNotFound_WhenExceptionOccursAsync()
    {
        _mediator
            .Setup(x => x.Send(It.IsAny<GetStudentAnswersByPollQuery>(), default))
            .ThrowsAsync(new Exception("failure"));

        var result = await _controller.GetStudentAnswersByPollAsync(1, 2, new Pagination());

        Assert.IsType<NotFoundObjectResult>(result);
    }
}
