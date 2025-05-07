using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.Features.PollVersions.Commands.CreatePollVersion;
using Eras.Application.Mappers;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;
using Moq;

namespace Eras.Application.Tests.Features.PollVersions.Commands;
public class CreatePollVersionCommandHandlerTest
{
    private readonly Mock<IPollVersionRepository> _mockIPollVersionRepositoryRepository;
    private readonly Mock<ILogger<CreatePollVersionCommandHandler>> _mockLogger;
    private readonly CreatePollVersionCommandHandler _handler;

    public CreatePollVersionCommandHandlerTest()
    {
        _mockIPollVersionRepositoryRepository = new Mock<IPollVersionRepository>();
        _mockLogger = new Mock<ILogger<CreatePollVersionCommandHandler>>();
        _handler = new CreatePollVersionCommandHandler(_mockIPollVersionRepositoryRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task HandlePollInstanceCreatesNewPollInstanceAsync()
    {
        var newPollVersion = new PollVersionDTO()
        {
            Name = "NewVersion"
        };
        var command = new CreatePollVersionCommand { PollVersionDTO = newPollVersion };
        var pollVersion = newPollVersion.ToDomain;

        _mockIPollVersionRepositoryRepository.Setup(Repo => Repo.AddAsync(It.IsAny<PollVersion>()))
            .ReturnsAsync(pollVersion);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("NewVersion", result.Entity.Name);
    }
}
