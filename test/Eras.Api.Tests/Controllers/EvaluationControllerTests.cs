using Eras.Api.Controllers;
using Eras.Application.DTOs;
using Eras.Application.Features.Evaluations.Commands;
using Eras.Application.Features.Evaluations.Commands.DeleteEvaluation;
using Eras.Application.Features.Evaluations.Commands.UpdateEvaluation;
using Eras.Application.Features.Evaluations.Queries;
using Eras.Application.Features.Evaluations.Queries.GetAll;
using Eras.Application.Features.Evaluations.Queries.GetByDateRange;
using Eras.Application.Mappers;
using Eras.Application.Models.Response;
using Eras.Application.Models.Response.Common;
using Eras.Application.Utils;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Api.Tests.Controllers
{
    public class EvaluationControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly Mock<ILogger<EvaluationsController>> _mockLogger;
        private readonly EvaluationsController _controller;

        public EvaluationControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _mockLogger = new Mock<ILogger<EvaluationsController>>();
            _controller = new EvaluationsController(_mockMediator.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateEvaluationController_Should_Return_SuccessAsync()
        {
            var evaluationDTO = new EvaluationDTO() { Name = "newEvaluation", StartDate = DateTime.UtcNow, EndDate = DateTime.Now };
            var parentId = "";
            var commandResponse = new CreateCommandResponse<Evaluation>(evaluationDTO.ToDomain(), "Success", true);
            _mockMediator.Setup(M => M.Send(It.IsAny<CreateEvaluationCommand>(), default))
                .ReturnsAsync(commandResponse);
            var result = await _controller.CreateEvaluationAsync(parentId, evaluationDTO) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async Task UpdateEvaluation_Should_Return_Ok_When_Ids_MatchAsync()
        {
            var dto = new EvaluationDTO
            {
                Id = 1,
                Name = "Updated Evaluation",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1)
            };
            var response = new CreateCommandResponse<Evaluation>(
                dto.ToDomain(),
                "Updated",
                true);
            _mockMediator
                .Setup(x => x.Send(It.IsAny<UpdateEvaluationCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
            var result = await _controller.UpdateEvaluationAsync(1, dto);
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, ok.StatusCode);
        }

        [Fact]
        public async Task UpdateEvaluation_Should_Return_BadRequest_WhenIdsDoNotMatchAsync()
        {
            var dto = new EvaluationDTO {
                Id = 2,
                Name = "Evaluation",
                StartDate = DateTime.Now,
                EndDate= DateTime.Now,
            };
            var result = await _controller.UpdateEvaluationAsync(1, dto);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequest.StatusCode);
            _mockMediator.Verify(
                x => x.Send(It.IsAny<UpdateEvaluationCommand>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task DeleteEvaluation_Should_Return_OkAsync()
        {
            _mockMediator
                .Setup(x => x.Send(It.IsAny<DeleteEvaluationCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new BaseResponse("Deleted", true));
            var result = await _controller.DeleteEvaluationAsync(1);
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, ok.StatusCode);
        }

        [Fact]
        public async Task GetEvaluationDetailsAsync_Should_ReturnOk_WhenFoundAsync()
        {
            var response = new GetQueryResponse<Evaluation>(
                new Evaluation(),
                "Success",
                true);
            _mockMediator
                .Setup(X => X.Send(It.IsAny<GetEvaluationSummaryQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
            var result = await _controller.GetEvaluationDetailsAsync(1);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetEvaluationDetails_Should_Return_NotFound_WhenBodyIsNullAsync()
        {
            var response = new GetQueryResponse<Evaluation>(
                null,
                "Not found",
                false);
            _mockMediator
                .Setup(x => x.Send(It.IsAny<GetEvaluationSummaryQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
            var result = await _controller.GetEvaluationDetailsAsync(1);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetAllEvaluation_Should_Return_OkAsync()
        {
            var page = new PagedResult<Evaluation>(0, new List<Evaluation>());
            _mockMediator
                .Setup(x => x.Send(It.IsAny<GetAllEvaluationsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(page);

            var result = await _controller.GetAllEvaluationsAsync(new Pagination());

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(page, ok.Value);
        }

        [Fact]
        public async Task GetAllEvaluationsByDateRange_Should_Return_OkAsync()
        {
            var evaluations = new List<Evaluation>{new Evaluation()};
            _mockMediator
                .Setup(x => x.Send(It.IsAny<GetEvaluationsByDateRangeQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(evaluations);
            var result = await _controller.GetAllEvaluationsByDateRangeAsync(
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(1));
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(evaluations, ok.Value);
        }
    }
}
