using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Api.Controllers;
using Eras.Application.DTOs;
using Eras.Application.Features.Evaluations.Commands;
using Eras.Application.Mappers;
using Eras.Application.Models.Response.Common;
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
        public async Task CreateEvaluationController_Should_Return_Success()
        {
            var evaluationDTO = new EvaluationDTO() { Name = "newEvaluation", StartDate = DateTime.UtcNow, EndDate = DateTime.Now };
            var commandResponse = new CreateCommandResponse<Evaluation>(evaluationDTO.ToDomain(), "Success", true);
            _mockMediator.Setup(m => m.Send(It.IsAny<CreateEvaluationCommand>(), default))
                .ReturnsAsync(commandResponse);
            var result = await _controller.CreateEvaluationAsync(evaluationDTO) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }
    }
}
