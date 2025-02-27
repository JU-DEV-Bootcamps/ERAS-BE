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
using Eras.Application.Features.Variables.Commands.CreateVariable;
using Eras.Application.Mappers;
using Eras.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Eras.Application.Tests.Features.Variables.Commands
{
    public class CreateStudentCommandHandlerTests
    {
        private readonly Mock<IVariableRepository> _mockVariableRepository;
        private readonly Mock<ILogger<CreateVariableCommandHandler>> _mockLogger;
        private readonly CreateVariableCommandHandler _handler;

        public CreateStudentCommandHandlerTests()
        {
            _mockVariableRepository = new Mock<IVariableRepository>();
            _mockLogger = new Mock<ILogger<CreateVariableCommandHandler>>();
            _handler = new CreateVariableCommandHandler(_mockVariableRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_Variable_CreatesNewVariable()
        {
            var newVariableDto = new VariableDTO() { Name= "newVariable" };
            var command = new CreateVariableCommand { Variable = newVariableDto };
            var newVariable = newVariableDto.ToDomain;

            _mockVariableRepository.Setup(repo => repo.AddAsync(It.IsAny<Variable>()))
                .ReturnsAsync(newVariable);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal("newVariable", result.Entity.Name);
        }

    }
}
