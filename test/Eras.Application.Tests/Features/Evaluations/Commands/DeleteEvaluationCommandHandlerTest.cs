using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.Features.Evaluations.Commands;
using Eras.Application.Features.Evaluations.Commands.DeleteEvaluation;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Tests.Features.Evaluations.Commands
{
    public class DeleteEvaluationCommandHandlerTest
    {
        private readonly Mock<IEvaluationRepository> _mockEvaluationRepository;
        private readonly Mock<IPollRepository> _mockPollRepository;
        private readonly Mock<ILogger<DeleteEvaluationCommandHandler>> _mockLogger;
        private readonly Mock<IMediator> _mockMediator;
        private readonly DeleteEvaluationCommandHandler _handler;

        public DeleteEvaluationCommandHandlerTest()
        {
            _mockEvaluationRepository = new Mock<IEvaluationRepository>();
            _mockPollRepository = new Mock<IPollRepository>();
            _mockLogger = new Mock<ILogger<DeleteEvaluationCommandHandler>>();
            _mockMediator = new Mock<IMediator>();
            _handler = new DeleteEvaluationCommandHandler(_mockEvaluationRepository.Object, _mockPollRepository.Object,
                _mockLogger.Object, _mockMediator.Object);
        }

        [Fact]
        public async Task Handle_Component_Delete_Poll_By_Id()
        {
            /*
            var newEvaluationDto = new EvaluationDTO() { Name = "newEvaluation", StartDate = DateTime.UtcNow, EndDate = DateTime.Now };
            var command = new CreateEvaluationCommand { EvaluationDTO = newEvaluationDto };
            var newComponent = newEvaluationDto.ToDomain;

            _mockEvaluationRepository.Setup(repo => repo.AddAsync(It.IsAny<Evaluation>()))
                .ReturnsAsync(newComponent);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal("newEvaluation", result.Entity.Name);
            */
        }
    }
}