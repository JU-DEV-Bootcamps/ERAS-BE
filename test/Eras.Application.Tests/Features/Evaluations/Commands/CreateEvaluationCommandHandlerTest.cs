using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.Features.Evaluations.Commands;
using MediatR;
using Microsoft.Extensions.Logging;
using Eras.Application.Mappers;
using Moq;
using Eras.Domain.Entities;

namespace Eras.Application.Tests.Features.Evaluations.Commands
{
    public class CreateEvaluationCommandHandlerTest
    {
        private readonly Mock<IEvaluationRepository> _mockEvaluationRepository;
        private readonly Mock<IPollRepository> _mockPollRepository;
        private readonly Mock<ILogger<CreateEvaluationCommandHandler>> _mockLogger;
        private readonly Mock<IMediator> _mockMediator;
        private readonly CreateEvaluationCommandHandler _handler;

        public CreateEvaluationCommandHandlerTest()
        {
            _mockEvaluationRepository = new Mock<IEvaluationRepository>();
            _mockPollRepository = new Mock<IPollRepository>();
            _mockLogger = new Mock<ILogger<CreateEvaluationCommandHandler>>();
            _mockMediator = new Mock<IMediator>();
            _handler = new CreateEvaluationCommandHandler(_mockEvaluationRepository.Object, _mockPollRepository.Object,
                _mockLogger.Object,_mockMediator.Object);
        }

        [Fact]
        public async Task HandleComponentCreatesNewComponentIncompleteAsync()
        {
            var newEvaluationDto = new EvaluationDTO() { Name = "newEvaluation", StartDate= DateTime.UtcNow, EndDate = DateTime.Now };
            var command = new CreateEvaluationCommand { EvaluationDTO = newEvaluationDto };
            var newComponent = newEvaluationDto.ToDomain;

            _mockEvaluationRepository.Setup(Repo => Repo.AddAsync(It.IsAny<Evaluation>()))
                .ReturnsAsync(newComponent);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal("newEvaluation", result.Entity.Name);
        }
    }
}
