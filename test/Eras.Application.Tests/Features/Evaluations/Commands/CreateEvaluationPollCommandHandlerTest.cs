using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.Features.Evaluations.Commands;
using Microsoft.Extensions.Logging;
using Eras.Application.Mappers;
using Moq;
using Eras.Domain.Entities;

namespace Eras.Application.Tests.Features.Evaluations.Commands
{
    public class CreateEvaluationPollCommandHandlerTest
    {
        private readonly Mock<IEvaluationPollRepository> _mockEvaluationPollRepository;
        private readonly Mock<ILogger<CreateEvaluationPollCommandHandler>> _mockLogger;
        private readonly CreateEvaluationPollCommandHandler _handler;

        public CreateEvaluationPollCommandHandlerTest()
        {
            _mockEvaluationPollRepository = new Mock<IEvaluationPollRepository>();
            _mockLogger = new Mock<ILogger<CreateEvaluationPollCommandHandler>>();
            _handler = new CreateEvaluationPollCommandHandler(_mockEvaluationPollRepository.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task HandleComponentCreatesNewComponentIncompleteAsync()
        {
            var newEvaluationDto = new EvaluationDTO() { Name = "newEvaluation", StartDate = DateTime.UtcNow, EndDate = DateTime.Now, EvaluationPollId = 1, PollId = 1 };
            var command = new CreateEvaluationPollCommand { EvaluationDTO = newEvaluationDto };
            var newComponent = newEvaluationDto.ToDomain;

            _mockEvaluationPollRepository.Setup(Repo => Repo.AddAsync(It.IsAny<Evaluation>()))
                .ReturnsAsync(newComponent);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(1, result.Entity?.EvaluationPollId);
        }

        [Fact]
        public async Task HandleDoesNotDuplicateWhenPollIsAlreadyLinkedAsync()
        {
            var evaluationDto = new EvaluationDTO() { Id = 5, Name = "eval", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow, PollId = 7 };
            var command = new CreateEvaluationPollCommand { EvaluationDTO = evaluationDto };
            _mockEvaluationPollRepository.Setup(Repo => Repo.ExistsAsync(5, 7)).ReturnsAsync(true);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result.Success);
            _mockEvaluationPollRepository.Verify(Repo => Repo.AddAsync(It.IsAny<Evaluation>()), Times.Never);
        }

        [Fact]
        public async Task HandleLinksAnotherPollToTheSameEvaluationAsync()
        {
            var evaluationDto = new EvaluationDTO() { Id = 5, Name = "eval", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow, PollId = 8 };
            var command = new CreateEvaluationPollCommand { EvaluationDTO = evaluationDto };
            _mockEvaluationPollRepository.Setup(Repo => Repo.ExistsAsync(5, 7)).ReturnsAsync(true);
            _mockEvaluationPollRepository.Setup(Repo => Repo.ExistsAsync(5, 8)).ReturnsAsync(false);
            _mockEvaluationPollRepository.Setup(Repo => Repo.AddAsync(It.IsAny<Evaluation>()))
                .ReturnsAsync((Evaluation E) => E);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result.Success);
            _mockEvaluationPollRepository.Verify(Repo => Repo.AddAsync(It.Is<Evaluation>(E => E.Id == 5 && E.PollId == 8)), Times.Once);
        }
    }
}
