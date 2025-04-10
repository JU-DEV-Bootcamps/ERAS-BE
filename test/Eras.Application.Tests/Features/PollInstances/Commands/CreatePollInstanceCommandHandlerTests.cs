using Eras.Application.Contracts.Persistence;
using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Application.Features.PollInstances.Commands.CreatePollInstance;
using Eras.Application.Mappers;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.PollInstances.Commands
{
    public class CreatePollInstanceCommandHandlerTests
    {
        private readonly Mock<IPollInstanceRepository> _mockPollInstanceRepository;
        private readonly Mock<ILogger<CreatePollInstanceCommandHandler>> _mockLogger;
        private readonly CreatePollInstanceCommandHandler _handler;

        public CreatePollInstanceCommandHandlerTests()
        {
            _mockPollInstanceRepository = new Mock<IPollInstanceRepository>();
            _mockLogger = new Mock<ILogger<CreatePollInstanceCommandHandler>>();
            _handler = new CreatePollInstanceCommandHandler(_mockPollInstanceRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_PollInstance_CreatesNewPollInstance()
        {
            var newStudent = new StudentDTO
            {
                Id = 1,
            };
            var newPollIsntanceDto = new PollInstanceDTO() { Uuid = "Uuid1", Student = newStudent };
            var command = new CreatePollInstanceCommand { PollInstance = newPollIsntanceDto };
            var newPoll = newPollIsntanceDto.ToDomain;

            _mockPollInstanceRepository.Setup(repo => repo.AddAsync(It.IsAny<PollInstance>()))
                .ReturnsAsync(newPoll);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal("Uuid1", result.Entity.Uuid);
        }

    }
}
