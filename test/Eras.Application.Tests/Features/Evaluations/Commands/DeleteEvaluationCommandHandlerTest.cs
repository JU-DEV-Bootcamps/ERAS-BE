using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.DTOs.CL;
using Eras.Application.Features.Evaluations.Commands;
using Eras.Application.Features.Evaluations.Commands.DeleteEvaluation;
using Eras.Application.Mappers;
using Eras.Application.Models.Response;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evaluation = Eras.Domain.Entities.Evaluation;

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
        public async Task HandleComponentDeletesEvaluationNotFoundIdAsync()
        {
            BaseResponse responseExample = new BaseResponse("Evaluation not found", false);

            _mockEvaluationRepository.Setup(Repo => Repo.DeleteAsync(It.IsAny<Evaluation>()))
                .Returns(Task.CompletedTask);

            DeleteEvaluationCommand command = new DeleteEvaluationCommand() { id = 1 };
            BaseResponse response = await _handler.Handle(command, CancellationToken.None);
            Assert.NotNull(response);
            Assert.False(responseExample.Success);
            Assert.Equal(responseExample.Message, response.Message);
        }
        
        [Fact]
        public async Task HandleComponentDeletesEvaluationAsync()
        {
            var newEvaluationDto = new EvaluationDTO() { Name = "newEvaluation", StartDate = DateTime.UtcNow, EndDate = DateTime.Now };
            var newComponent = newEvaluationDto.ToDomain;

            _mockEvaluationRepository
                .Setup(Repo => Repo.GetByIdForUpdateAsync(It.IsAny<int>()))
                .ReturnsAsync(newComponent);

            BaseResponse responseExample = new BaseResponse("Evaluation deleted", true);

            _mockEvaluationRepository
                .Setup(Repo => Repo.DeleteAsync(It.IsAny<Evaluation>()))
                .Returns(Task.CompletedTask);

            DeleteEvaluationCommand command = new DeleteEvaluationCommand() { id = 1 };
            BaseResponse response = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(response);
            Assert.True(response.Success);
            Assert.Equal(responseExample.Message, response.Message);
        }

    }
}