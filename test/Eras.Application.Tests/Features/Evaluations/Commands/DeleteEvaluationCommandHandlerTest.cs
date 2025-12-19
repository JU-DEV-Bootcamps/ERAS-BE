using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.Features.Evaluations.Commands.DeleteEvaluation;
using Eras.Application.Mappers;
using Eras.Application.Models.Response;
using Eras.Error;
using Eras.Error.Bussiness;

using Moq;

using Evaluation = Eras.Domain.Entities.Evaluation;

namespace Eras.Application.Tests.Features.Evaluations.Commands
{
    public class DeleteEvaluationCommandHandlerTest
    {
        private readonly Mock<IEvaluationRepository> _mockEvaluationRepository;
        private readonly DeleteEvaluationCommandHandler _handler;

        public DeleteEvaluationCommandHandlerTest()
        {
            _mockEvaluationRepository = new Mock<IEvaluationRepository>();
            _handler = new DeleteEvaluationCommandHandler(_mockEvaluationRepository.Object);
        }

        [Fact]
        public Task HandleComponentDeletesEvaluationNotFoundIdAsync()
        {
            var evaluationId = 1;
            var expectedMessage = $"Evaluation with ID {evaluationId} not found";
            _mockEvaluationRepository.Setup(Repo => Repo.DeleteAsync(It.IsAny<Evaluation>()))
                .Returns(Task.CompletedTask);

            DeleteEvaluationCommand command = new DeleteEvaluationCommand() { id = 1 };

            var response = Assert.ThrowsAsync<NotFoundException>(async () => await _handler.Handle(command, CancellationToken.None));
            var exception = response.Result as IErasException;

            Assert.NotNull(response);
            Assert.NotNull(exception);
            Assert.Equal(exception.FriendlyMessage, expectedMessage);
            return Task.CompletedTask;
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