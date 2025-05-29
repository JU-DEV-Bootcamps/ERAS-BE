using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Application.Features.Answers.Commands.CreateAnswer;
using Eras.Application.Features.Components.Commands.CreateCommand;
using Eras.Application.Features.Polls.Commands.CreatePoll;
using Eras.Application.Mappers;
using Eras.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Eras.Application.Tests.Features.Components.Commands
{
    public class CreateAnswerCommandHandlerTests
    {
        private readonly Mock<IAnswerRepository> _mockAnswerRepository;
        private readonly Mock<ILogger<CreateAnswerCommandHandler>> _mockLogger;
        private readonly CreateAnswerCommandHandler _handler;

        public CreateAnswerCommandHandlerTests()
        {
            _mockAnswerRepository = new Mock<IAnswerRepository>();
            _mockLogger = new Mock<ILogger<CreateAnswerCommandHandler>>();
            _handler = new CreateAnswerCommandHandler(_mockAnswerRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task HandleComponentCreatesNewComponentAsync()
        {
            var newAnswerDto = new AnswerDTO() { Answer= "newAnswer" };
            var command = new CreateAnswerCommand { Answer = newAnswerDto };
            var newPoll = newAnswerDto.ToDomain;

            _mockAnswerRepository.Setup(Repo => Repo.AddAsync(It.IsAny<Answer>()))
                .ReturnsAsync(newPoll);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal("newAnswer", result.Entity?.AnswerText);
        }

    }
}
