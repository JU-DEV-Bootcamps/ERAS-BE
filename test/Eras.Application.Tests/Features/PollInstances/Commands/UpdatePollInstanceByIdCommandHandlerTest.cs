using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Application.Features.PollInstances.Commands.CreatePollInstance;
using Eras.Application.Features.Polls.Commands.UpdatePoll;
using Eras.Application.Mappers;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;
using Moq;

namespace Eras.Application.Tests.Features.PollInstances.Commands;
public class UpdatePollInstanceByIdCommandHandlerTest
{
    private readonly Mock<IPollRepository> _mockPollRepository;
    private readonly Mock<ILogger<UpdatePollByIdCommandHandler>> _mockLogger;
    private readonly UpdatePollByIdCommandHandler _handler;

    public UpdatePollInstanceByIdCommandHandlerTest()
    {
        _mockPollRepository = new Mock<IPollRepository>();
        _mockLogger = new Mock<ILogger<UpdatePollByIdCommandHandler>>();
        _handler = new UpdatePollByIdCommandHandler(_mockPollRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task HandlePollUpdatesPollAsync()
    {
        var updatedPollDto = new PollDTO() { Uuid = "Uuid1", Id = 1, LastVersion = 2};
        var command = new UpdatePollByIdCommand { PollDTO = updatedPollDto };
        var responsePoll = updatedPollDto.ToDomain;
        var oldPoll = updatedPollDto.ToDomain();
        oldPoll.LastVersion = 1;

        _mockPollRepository.Setup(Repo => Repo.UpdateAsync(It.IsAny<Poll>()))
            .ReturnsAsync(responsePoll);

        _mockPollRepository.Setup(Repo => Repo.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(oldPoll);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Uuid1", result.Entity.Uuid);
    }
}
