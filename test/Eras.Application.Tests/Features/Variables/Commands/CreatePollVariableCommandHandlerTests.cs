
using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.Features.Variables.Commands.CreatePollVariable;
using Eras.Application.Mappers;
using Eras.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Eras.Application.Tests.Features.Variables.Commands
{
    public class CreatePollVariableCommandHandlerTests
    {
        private readonly Mock<IPollVariableRepository> _mockPollVariableRepository;
        private readonly Mock<ILogger<CreatePollVariableCommandHandler>> _mockLogger;
        private readonly CreatePollVariableCommandHandler _handler;

        public CreatePollVariableCommandHandlerTests()
        {
            _mockPollVariableRepository = new Mock<IPollVariableRepository>();
            _mockLogger = new Mock<ILogger<CreatePollVariableCommandHandler>>();
            _handler = new CreatePollVariableCommandHandler(_mockPollVariableRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task HandlePollVariableCreatesNewPollVariableAsync()
        {
            var newVariableDto = new VariableDTO() { Name= "newPollVariable" };
            var command = new CreatePollVariableCommand { Variable = newVariableDto };
            var newPollVariable = newVariableDto.ToDomain;

            _mockPollVariableRepository.Setup(Repo => Repo.AddAsync(It.IsAny<Variable>()))
                .ReturnsAsync(newPollVariable);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal("newPollVariable", result.Entity?.Name);
        }

        [Fact]
        public async Task Handle_CreatePollVariableThrowsAnExceptionAsync()
        {
            _mockPollVariableRepository
                .Setup(x => x.GetByPollIdAndVariableIdAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("Some error"));

            var command = new CreatePollVariableCommand { Variable = new() };

            _mockPollVariableRepository
                .Setup(x => x.AddAsync(It.IsAny<Variable>()))
                .ThrowsAsync(new Exception("Some error"));

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.Success);
            Assert.Null(result.Entity);
            Assert.Equal("Error", result.Message);
        }

        [Fact]
        public async Task Handle_CreatePollVariableErrorInRequestVariable()
        {
            var newVariableDto = new VariableDTO() { Name = "newPollVariable" };
            var command = new CreatePollVariableCommand { Variable = null };
            var newPollVariable = newVariableDto.ToDomain;

            _mockPollVariableRepository.Setup(Repo => Repo.AddAsync(It.IsAny<Variable>()))
                .ReturnsAsync(newPollVariable);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.Success);
            Assert.Null(result.Entity);
            Assert.Equal("Error", result.Message);
        }
    }
}
