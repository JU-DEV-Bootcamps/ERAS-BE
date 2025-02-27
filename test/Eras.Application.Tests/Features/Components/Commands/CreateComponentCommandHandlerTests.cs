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
using Eras.Application.Mappers;
using Eras.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Eras.Application.Tests.Features.Answers.Commands
{
    public class CreateAnswerCommandHandlerTests
    {
        private readonly Mock<IComponentRepository> _mockComponentRepository;
        private readonly Mock<ILogger<CreateComponentCommandHandler>> _mockLogger;
        private readonly CreateComponentCommandHandler _handler;

        public CreateAnswerCommandHandlerTests()
        {
            _mockComponentRepository = new Mock<IComponentRepository>();
            _mockLogger = new Mock<ILogger<CreateComponentCommandHandler>>();
            _handler = new CreateComponentCommandHandler(_mockComponentRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_Component_CreatesNewComponent()
        {
            var newComponentDto = new ComponentDTO() { Name= "newComponent" };
            var command = new CreateComponentCommand { Component = newComponentDto };
            var newPoll = newComponentDto.ToDomain;

            _mockComponentRepository.Setup(repo => repo.AddAsync(It.IsAny<Component>()))
                .ReturnsAsync(newPoll);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal("newComponent", result.Entity.Name);
        }

    }
}
