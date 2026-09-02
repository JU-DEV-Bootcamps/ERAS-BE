using Eras.Application.Contracts.Persistence;
using Eras.Application.Dtos;
using Eras.Application.Features.Polls.Commands.UpdatePoll;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.Polls.Commands;

public class UpdatePollByIdCommandHandlerTests
{
    private readonly Mock<IPollRepository> _pollRepository;
    private readonly Mock<ILogger<UpdatePollByIdCommandHandler>> _logger;
    private readonly UpdatePollByIdCommandHandler _handler;

    public UpdatePollByIdCommandHandlerTests()
    {
        _pollRepository = new Mock<IPollRepository>();
        _logger = new Mock<ILogger<UpdatePollByIdCommandHandler>>();

        _handler = new UpdatePollByIdCommandHandler(_pollRepository.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenPollDoesNotExist()
    {
        var pollDto = new PollDTO { Id = 1 };
        var request = new UpdatePollByIdCommand { PollDTO = pollDto };

        _pollRepository
            .Setup(x => x.GetByIdAsync(pollDto.Id))
            .ReturnsAsync((Poll)null!);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Poll Not Found", result.Message);
        Assert.True(result.Success);
        Assert.Equal(Models.Enums.CommandEnums.CommandResultStatus.NotFound, result.Status);

        _pollRepository.Verify(x => x.UpdateAsync(It.IsAny<Poll>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldUpdateAndReturnPoll_WhenPollExists()
    {
        var pollDto = new PollDTO { Id = 1 };
        var existingPoll = new Poll { Id = 1 };
        var updatedPoll = new Poll { Id = 1 };

        var request = new UpdatePollByIdCommand { PollDTO = pollDto };

        _pollRepository
            .Setup(x => x.GetByIdAsync(pollDto.Id))
            .ReturnsAsync(existingPoll);

        _pollRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Poll>()))
            .ReturnsAsync(updatedPoll);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(updatedPoll, result.Entity);
        Assert.Equal("Poll Updated", result.Message);
        Assert.True(result.Success);

        _pollRepository.Verify(x => x.GetByIdAsync(pollDto.Id), Times.Once);

        _pollRepository.Verify(x => x.UpdateAsync(It.IsAny<Poll>()), Times.Once);
    }
}
