using Eras.Application.Contracts.Persistence;
using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Application.Features.PollInstances.Commands.UpdatePollInstance;
using Eras.Application.Mappers;
using Eras.Application.Models.Enums;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;
using Moq;

namespace Eras.Application.Tests.Features.PollInstances.Commands;
public class UpdatePollInstanceByIdCommandHandlerTest
{
    private readonly Mock<IPollInstanceRepository> _mockPollRepository;
    private readonly Mock<ILogger<UpdatePollInstanceByIdCommandHandler>> _mockLogger;
    private readonly UpdatePollInstanceByIdCommandHandler _handler;

    public UpdatePollInstanceByIdCommandHandlerTest()
    {
        _mockPollRepository = new Mock<IPollInstanceRepository>();
        _mockLogger = new Mock<ILogger<UpdatePollInstanceByIdCommandHandler>>();
        _handler = new UpdatePollInstanceByIdCommandHandler(_mockPollRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handler_ReturnsSuccessResponseWithUpdatedPoll()
    {
        var updatedPollInstanceDTO = new PollInstanceDTO() { Uuid = "Uuid1", Id = 1, LastVersion = 2};
        PollInstance oldPoll = updatedPollInstanceDTO.ToDomain();
        oldPoll.LastVersion = 1;
        PollInstance responsePoll = updatedPollInstanceDTO.ToDomain();

        var command = new UpdatePollInstanceByIdCommand { PollInstanceDTO = updatedPollInstanceDTO };

        _mockPollRepository.Setup(Repo => Repo.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(oldPoll);
        _mockPollRepository.Setup(Repo => Repo.UpdateAsync(It.IsAny<PollInstance>()))
            .ReturnsAsync(responsePoll);

        CreateCommandResponse<PollInstance> result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result.Entity);
        Assert.True(result.Success);
        Assert.Equal(responsePoll, result.Entity);
        Assert.Equal("Updated Poll Instance", result.Message);
    }

    [Fact]
    public async Task Handler_ReturnsErrorResponseWhenNoPollExists()
    {
        var updatedPollInstanceDTO = new PollInstanceDTO() { Uuid = "Uuid1", Id = 1, LastVersion = 2};
        PollInstance responsePoll = updatedPollInstanceDTO.ToDomain();

        var command = new UpdatePollInstanceByIdCommand { PollInstanceDTO = updatedPollInstanceDTO };

        _mockPollRepository.Setup(Repo => Repo.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(value: null);

        CreateCommandResponse<PollInstance> result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result.Entity);
        Assert.False(result.Success);
        Assert.Equal(CommandEnums.CommandResultStatus.NotFound, result.Status);
        Assert.Equal("Poll Instance Not Found", result.Message);
    }
}
