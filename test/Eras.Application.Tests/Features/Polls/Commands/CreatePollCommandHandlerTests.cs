using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Dtos;
using Eras.Application.Features.Polls.Commands.CreatePoll;
using Eras.Application.Mappers;
using Eras.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Eras.Application.Tests.Features.Polls.Commands
{
    public class CreatePollCommandHandlerTests
    {
        private readonly Mock<IPollRepository> _mockPollRepository;
        private readonly Mock<ILogger<CreatePollCommandHandler>> _mockLogger;
        private readonly CreatePollCommandHandler _handler;

        public CreatePollCommandHandlerTests()
        {
            _mockPollRepository = new Mock<IPollRepository>();
            _mockLogger = new Mock<ILogger<CreatePollCommandHandler>>();
            _handler = new CreatePollCommandHandler(_mockPollRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_Poll_CreatesNewPoll()
        {
            var newPollDto = new PollDTO() { Name= "newPoll" };
            var command = new CreatePollCommand { Poll = newPollDto };
            var newPoll = newPollDto.ToDomain;

            _mockPollRepository.Setup(Repo => Repo.AddAsync(It.IsAny<Poll>()))
                .ReturnsAsync(newPoll);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal("newPoll",result.Entity.Name);
        }

    }
}
