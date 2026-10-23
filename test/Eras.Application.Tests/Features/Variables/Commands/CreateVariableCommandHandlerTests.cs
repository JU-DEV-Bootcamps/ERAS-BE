using Eras.Application.Contracts.Persistence;
using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Application.Features.Components.Commands.CreateCommand;
using Eras.Application.Features.Polls.Commands.CreatePoll;
using Eras.Application.Features.Variables.Commands.CreateVariable;
using Eras.Application.Mappers;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.Variables.Commands
{
    public class CreateVariableCommandHandlerTests
    {
        private readonly Mock<IVariableRepository> _mockVariableRepository;
        private readonly Mock<ILogger<CreateVariableCommandHandler>> _mockLogger;
        private readonly CreateVariableCommandHandler _handler;

        public CreateVariableCommandHandlerTests()
        {
            _mockVariableRepository = new Mock<IVariableRepository>();
            _mockLogger = new Mock<ILogger<CreateVariableCommandHandler>>();
            _handler = new CreateVariableCommandHandler(_mockVariableRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task HandleVariableCreatesNewVariableAsync()
        {
            var newVariableDto = new VariableDTO() { Name= "newVariable" };
            var command = new CreateVariableCommand { Variable = newVariableDto };
            var newVariable = newVariableDto.ToDomain;

            _mockVariableRepository.Setup(Repo => Repo.AddAsync(It.IsAny<Variable>()))
                .ReturnsAsync(newVariable);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal("newVariable", result.Entity?.Name);
        }

        [Fact]
        public async Task Handle_VariableCreateWithNullRequestVariable()
        {
            var command = new CreateVariableCommand { Variable = null };

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.Null(result.Entity);
            Assert.False(result.Success);
            Assert.Equal("Error", result!.Message);
            Assert.Equal(0, result.SuccessfullImports);
        }

        [Fact]
        public async Task Handle_VariableTryToCreateVariableAlreadyExistentAsync()
        {
            var variableDto = new VariableDTO() { Name = "variable" };
            var command = new CreateVariableCommand { Variable = variableDto };
            var variable = variableDto.ToDomain;

            _mockVariableRepository.Setup(Repo => Repo.GetByNameAndPositionAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(variable);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(0, result.SuccessfullImports);
            Assert.Equal("variable", result.Entity?.Name);
        }

        [Fact]
        public async Task Handle_VariableTryToCreateVariable_ThrowsException()
        {
            var variableDto = new VariableDTO() { Name = "variable" };
            var command = new CreateVariableCommand { Variable = variableDto };

            _mockVariableRepository
                .Setup(Repo => Repo.GetByNameAndPositionAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("Error db"));

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.Null(result.Entity);
            Assert.False(result.Success);
            Assert.Equal(0, result.SuccessfullImports);
            Assert.Equal("Error", result.Message);
        }
    }
}
