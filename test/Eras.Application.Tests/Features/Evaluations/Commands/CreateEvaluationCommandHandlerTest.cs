using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.Events;
using Eras.Application.Features.Evaluations.Commands;
using Eras.Application.Mappers;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using Eras.Error.Bussiness;

using MediatR;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.Evaluations.Commands
{
    public class CreateEvaluationCommandHandlerTest
    {
        private readonly Mock<IEvaluationRepository> _mockEvaluationRepository;
        private readonly Mock<IPollRepository> _mockPollRepository;
        private readonly Mock<IMediator> _mockMediator;
        private readonly CreateEvaluationCommandHandler _handler;

        public CreateEvaluationCommandHandlerTest()
        {
            _mockEvaluationRepository = new Mock<IEvaluationRepository>();
            _mockPollRepository = new Mock<IPollRepository>();
            _mockMediator = new Mock<IMediator>();
            _handler = new CreateEvaluationCommandHandler(_mockEvaluationRepository.Object, _mockPollRepository.Object, _mockMediator.Object);
        }

        [Fact]
        public async Task HandleComponentCreatesNewComponentIncompleteAsync()
        {
            var newEvaluationDto = new EvaluationDTO() { Name = "newEvaluation", StartDate = DateTime.UtcNow, EndDate = DateTime.Now };
            var command = new CreateEvaluationCommand { EvaluationDTO = newEvaluationDto };
            var newComponent = newEvaluationDto.ToDomain;

            _mockEvaluationRepository
               .Setup(x => x.GetByNameAsync(newEvaluationDto.Name))
               .ReturnsAsync((Evaluation?)null);
            _mockEvaluationRepository.Setup(Repo => Repo.AddAsync(It.IsAny<Evaluation>()))
                .ReturnsAsync(newComponent);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal("newEvaluation", result.Entity?.Name);

            _mockPollRepository.Verify(x => x.GetByParentIdAsync(It.IsAny<string>()), Times.Never);
            _mockMediator.Verify(
                x => x.Send(It.IsAny<CreateEvaluationPollCommand>(), It.IsAny<CancellationToken>()),
                Times.Never);
            _mockMediator.Verify(
            x => x.Publish(
                It.IsAny<EvaluationCreatedEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenEvaluationWithSameNameAlreadyExists()
        {
            // Arrange
            var dto = new EvaluationDTO
            {
                Name = "existingEvaluation",
                PollName = string.Empty,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow
            };

            var command = new CreateEvaluationCommand
            {
                EvaluationDTO = dto
            };

            var existingEvaluation = new Evaluation
            {
                Name = dto.Name
            };

            _mockEvaluationRepository
                .Setup(x => x.GetByNameAsync(dto.Name))
                .ReturnsAsync(existingEvaluation);

            await Assert.ThrowsAsync<ExistingEvaluationNameException>(
                () => _handler.Handle(command, CancellationToken.None));

            _mockEvaluationRepository.Verify(
                x => x.AddAsync(It.IsAny<Evaluation>()),
                Times.Never);

            _mockPollRepository.Verify(
                x => x.GetByParentIdAsync(It.IsAny<string>()),
                Times.Never);

            _mockMediator.Verify(
                x => x.Send(
                    It.IsAny<CreateEvaluationPollCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            _mockMediator.Verify(
                x => x.Publish(
                    It.IsAny<EvaluationCreatedEvent>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldCreateReadyEvaluationAndPublishEvent_WhenPollExists()
        {
            // Arrange
            var dto = new EvaluationDTO
            {
                Name = "evaluationWithPoll",
                PollName = "ExistingPoll",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow
            };

            var command = new CreateEvaluationCommand
            {
                ParentId = "123",
                EvaluationDTO = dto
            };

            var poll = new Poll
            {
                Id = 456
            };

            var createdEvaluation = dto.ToDomain();
            createdEvaluation.Id = 789;
            _mockEvaluationRepository
            .Setup(x => x.GetByNameAsync(dto.Name))
            .ReturnsAsync((Evaluation?)null);

            _mockPollRepository
                .Setup(x => x.GetByParentIdAsync(command.ParentId))
                .ReturnsAsync(poll);

            _mockEvaluationRepository
                .Setup(x => x.AddAsync(It.IsAny<Evaluation>()))
                .ReturnsAsync(createdEvaluation);

            _mockMediator
                .Setup(x => x.Send(
                    It.IsAny<CreateEvaluationPollCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(It.IsAny<CreateCommandResponse<Evaluation>>());

            _mockMediator
                .Setup(x => x.Publish(
                    It.IsAny<EvaluationCreatedEvent>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _handler.Handle(
            command,
            CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("evaluationWithPoll", result.Entity?.Name);

            _mockEvaluationRepository.Verify(
                x => x.AddAsync(
                    It.Is<Evaluation>(e =>
                        e.PollId == poll.Id &&
                        e.Status ==
                        EvaluationConstants.EvaluationStatus.Ready.ToString())),
                Times.Once);

            _mockPollRepository.Verify(
                x => x.GetByParentIdAsync(command.ParentId),
                Times.Once);

            _mockMediator.Verify(
                x => x.Send(
                    It.Is<CreateEvaluationPollCommand>(c =>
                        c.EvaluationDTO.PollId == poll.Id &&
                        c.EvaluationDTO.Id == createdEvaluation.Id),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            //_mockMediator.Verify(
            //x => x.Publish(
            //    It.Is<EvaluationCreatedEvent>(e =>
            //        e.Id == createdEvaluation.Id),
            //    It.IsAny<CancellationToken>()),
            //Times.Once);
        }
    }
}
