using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Application.Features.Components.Commands.CreateCommand;
using Eras.Application.Features.Polls.Commands.CreatePoll;
using Eras.Application.Features.Variables.Commands.CreatePollVariable;
using Eras.Application.Features.Variables.Commands.CreateVariable;
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
        public async Task Handle_PollVariable_CreatesNewPollVariable()
        {
            var newVariableDto = new VariableDTO() { Name= "newPollVariable" };
            var command = new CreatePollVariableCommand { Variable = newVariableDto };
            var newPollVariable = newVariableDto.ToDomain;

            _mockPollVariableRepository.Setup(Repo => Repo.AddAsync(It.IsAny<Variable>()))
                .ReturnsAsync(newPollVariable);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal("newPollVariable", result.Entity.Name);
        }

    }
}
