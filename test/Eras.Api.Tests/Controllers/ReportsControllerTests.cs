using Eras.Api.Controllers;
using Eras.Application.DTOs.CL;
using Eras.Application.DTOs.Views;
using Eras.Application.Features.Consolidator.Queries;
using Eras.Application.Features.Consolidator.Queries.Polls;
using Eras.Application.Features.Consolidator.Queries.Students;
using Eras.Application.Models.Consolidator;
using Eras.Application.Models.Response.Common;
using Eras.Application.Utils;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using Moq;

using Xunit;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Eras.Api.Tests.Controllers;

public class ReportsControllerTests
{
    private readonly Mock<IMediator> _mediator = new();
    private readonly ReportsController _controller;

    public ReportsControllerTests()
    {
        _controller = new ReportsController(_mediator.Object);
    }

    [Fact]
    public async Task GetHigherRiskStudentsByCohortAsync_ReturnsOk_WhenQuerySucceedsAsync()
    {
        var student = new List<(Student, List<Answer>, decimal)>
        {
            (new Student
            {
                Uuid = "student-1",
                Name = "John"
            },
            new List<Answer>
            {
                new()
                {
                    Id = 1,
                    AnswerText = "Yes",
                    RiskLevel = 2,
                    PollVariableId = 3,
                    PollInstanceId = 4
                }
            },
            0.75m),
        };

        var response = new GetQueryResponse<List<(Student, List<Answer>, decimal)>>(student, "Success", true);
        _mediator
            .Setup(x => x.Send(It.IsAny<GetStudentTopQuery>(), default))
            .ReturnsAsync(response);

        var result = await _controller.GetHigherRiskStudentsByCohortAsync("C1", "Poll", 5);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetHigherRiskStudentsByCohortAsync_ReturnsBadRequest_WhenQueryFailsAsync()
    {
        var response = new GetQueryResponse<List<(Student, List<Answer>, decimal)>>(null, "Error", false);
        _mediator
            .Setup(x => x.Send(It.IsAny<GetStudentTopQuery>(), default))
            .ReturnsAsync(response);

        var result = await _controller.GetHigherRiskStudentsByCohortAsync("C1", "Poll", 5);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetHigherRiskStudentsByCohortAsync_ReturnsNotFound_WhenExceptionOccursAsync()
    {
        _mediator
            .Setup(x => x.Send(It.IsAny<GetStudentTopQuery>(), default))
            .ThrowsAsync(new Exception("boom"));

        var result = await _controller.GetHigherRiskStudentsByCohortAsync("C1", "Poll", 5);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetHigherRiskStudentsByPollAsync_ReturnsOkAsync()
    {
        var response = new PagedResult<ErasCalculationsByPollDTO>(Items: null, Count: 0 );
        _mediator
            .Setup(x => x.Send(It.IsAny<GetPollTopQuery>(), default))
            .ReturnsAsync(response);

        var result = await _controller.GetHigherRiskStudentsByPollAsync(
            Guid.NewGuid().ToString(),
            new Pagination(),
            "1,2,3");

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetHigherRiskStudentsByPollAsync_ReturnsNotFound_WhenExceptionOccursAsync()
    {
        _mediator
            .Setup(x => x.Send(It.IsAny<GetPollTopQuery>(), default))
            .ThrowsAsync(new Exception());

        var result = await _controller.GetHigherRiskStudentsByPollAsync(
            Guid.NewGuid().ToString(),
            new Pagination(),
            "1,2");

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetAvgRiskByPollAsync_ReturnsBadRequest_WhenNoCohortsAsync()
    {
        var result = await _controller.GetAvgRiskByPollAsync(
            Guid.NewGuid().ToString(), "", false, 1);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetAvgRiskByPollAsync_ReturnsOk_WhenSuccessfulAsync()
    {
        var data = new AvgReportResponseVm();
        var response = new GetQueryResponse<AvgReportResponseVm>(data, "Success", true);
        _mediator
            .Setup(x => x.Send(It.IsAny<PollAvgQuery>(), default))
            .ReturnsAsync(response);

        var result = await _controller.GetAvgRiskByPollAsync(Guid.NewGuid().ToString(), "1,2", false, 1);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetAvgRiskByPollAsync_ReturnsBadRequest_WhenQueryFailsAsync()
    {
        var response = new GetQueryResponse<AvgReportResponseVm>(null, "Error", false);
        _mediator
            .Setup(x => x.Send(It.IsAny<PollAvgQuery>(), default))
            .ReturnsAsync(response);

        var result = await _controller.GetAvgRiskByPollAsync(Guid.NewGuid().ToString(), "1", false, 1);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetAvgRiskByPollAsync_Returns500_WhenExceptionOccursAsync()
    {
        _mediator
            .Setup(x => x.Send(It.IsAny<PollAvgQuery>(), default))
            .ThrowsAsync(new Exception());

        var result = await _controller.GetAvgRiskByPollAsync(Guid.NewGuid().ToString(),"1",false,1);

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objectResult.StatusCode);
    }

    [Fact]
    public async Task GetPollResultsCountAsync_ReturnsOk_WhenSuccessfulAsync()
    {
        var data = new CountReportResponseVm();
        var response = new GetQueryResponse<CountReportResponseVm>(data, "Success", true);
        _mediator
            .Setup(x => x.Send(It.IsAny<PollCountQuery>(), default))
            .ReturnsAsync(response);

        var result = await _controller.GetPollResultsCountAsync(
            Guid.NewGuid().ToString(), 1, "1,2", "3,4", false);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetPollResultsCountAsync_ReturnsBadRequest_WhenQueryFailsAsync()
    {
        var response = new GetQueryResponse<CountReportResponseVm>(null, "Error", false);
        _mediator
            .Setup(x => x.Send(It.IsAny<PollCountQuery>(), default))
            .ReturnsAsync(response);

        var result = await _controller.GetPollResultsCountAsync(
            Guid.NewGuid().ToString(), 1, "1", "2", false);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetPollResultsCountAsync_Returns500_WhenExceptionOccursAsync()
    {
        _mediator
            .Setup(x => x.Send(It.IsAny<PollCountQuery>(), default))
            .ThrowsAsync(new Exception());

        var result = await _controller.GetPollResultsCountAsync(
            Guid.NewGuid().ToString(), 1, "1", "2", false);

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objectResult.StatusCode);
    }

    [Fact]
    public async Task GetComponentSummaryByPollAsync_ReturnsOk_WhenSuccessfulAsync()
    {
        var data = new RiskCountResponseVm()
        {
            AverageRisk = 2
        };
        var response = new GetQueryResponse<RiskCountResponseVm>(data, "Success", true);
        _mediator
            .Setup(x => x.Send(It.IsAny<GetRiskCountQuery>(), default))
            .ReturnsAsync(response);

        var result = await _controller.GetComponentSummaryByPollAsync(Guid.NewGuid().ToString());

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetComponentSummaryByPollAsync_ReturnsNotFound_WhenQueryFailsAsync()
    {
        var data = It.IsAny<RiskCountResponseVm>();
        var response = new GetQueryResponse<RiskCountResponseVm>(data, "Error", false);
        _mediator
            .Setup(x => x.Send(It.IsAny<GetRiskCountQuery>(), default))
            .ReturnsAsync(response);

        var result = await _controller.GetComponentSummaryByPollAsync(Guid.NewGuid().ToString());

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetComponentSummaryByPollAsync_ReturnsBadRequest_WhenExceptionOccursAsync()
    {
        _mediator
            .Setup(x => x.Send(It.IsAny<GetRiskCountQuery>(), default))
            .ThrowsAsync(new Exception());

        var result = await _controller.GetComponentSummaryByPollAsync(Guid.NewGuid().ToString());

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetCountSummaryAsync_ReturnsOkAsync()
    {
        var data = It.IsAny<Dictionary<string, int>>();
        var response = new GetQueryResponse<Dictionary<string, int>>(data, "Success", true);
        _mediator
            .Setup(x => x.Send(It.IsAny<GetCountSummaryQuery>(), default))
            .ReturnsAsync(response);

        var result = await _controller.GetCountSummaryAsync();

        Assert.IsType<OkObjectResult>(result);
    }
}