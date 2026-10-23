using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.Features.PollInstances.Commands.CreatePollInstance;
using Eras.Application.Mappers;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Eras.Application.Tests.Features.PollInstances.Commands;
public class CreatePollInstanceCommandHandlerTests
{
    private readonly Mock<IPollInstanceRepository> _mockPollInstanceRepository;
    private readonly Mock<ILogger<CreatePollInstanceCommandHandler>> _mockLogger;
    private readonly CreatePollInstanceCommandHandler _handler;

    public CreatePollInstanceCommandHandlerTests()
    {
        _mockPollInstanceRepository = new Mock<IPollInstanceRepository>();
        _mockLogger = new Mock<ILogger<CreatePollInstanceCommandHandler>>();
        _handler = new CreatePollInstanceCommandHandler(_mockPollInstanceRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handler_CreatesNewPollInstanceAsync()
    {
        var newStudent = new StudentDTO
        {
            Id = 1,
        };
        var newPollInstanceDTO = new PollInstanceDTO() { Uuid= "Uuid1" , Student = newStudent};
        var command = new CreatePollInstanceCommand { PollInstance = newPollInstanceDTO };
        PollInstance newPoll = newPollInstanceDTO.ToDomain();

        _mockPollInstanceRepository.Setup(Repo => Repo.AddAsync(It.IsAny<PollInstance>()))
            .ReturnsAsync(newPoll);

        CreateCommandResponse<PollInstance> result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result.Entity);
        Assert.Equal(newPoll, result.Entity);
        Assert.True(result.Success);
        Assert.Equal(1, result.SuccessfullImports);
        Assert.Equal("Success", result.Message);
    }

    [Fact]
    public async Task Handler_ReturnsExistingEntityId()
    {
        var newStudent = new StudentDTO
        {
            Id = 1,
        };
        var newPollInstanceDTO = new PollInstanceDTO() { Uuid= "Uuid1" , Student = newStudent};
        var command = new CreatePollInstanceCommand { PollInstance = newPollInstanceDTO };
        PollInstance existingEntity = newPollInstanceDTO.ToDomain();

        _mockPollInstanceRepository.Setup(Repo => Repo.GetByUuidAndStudentIdAsync(
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<int>())
        ).ReturnsAsync(existingEntity);

        CreateCommandResponse<PollInstance> result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result.Entity);
        Assert.Equal(existingEntity, result.Entity);
        Assert.True(result.Success);
        Assert.Equal(0, result.SuccessfullImports);
        Assert.Equal("Success", result.Message);
    }

    [Fact]
    public async Task Handler_ShouldReturnFailureResponse_WhenNoPollInstanceIsProvided()
    {
        var command = new CreatePollInstanceCommand { PollInstance = null };

        CreateCommandResponse<PollInstance> result = await _handler.Handle(command, CancellationToken.None);

        Assert.Null(result.Entity);
        Assert.False(result.Success);
        Assert.Equal(0, result.SuccessfullImports);
        Assert.Equal("Error", result.Message);
    }

    [Fact]
    public async Task Handler_ShouldReturnFailureResponse_WhenExceptionIsThrown()
    {
        var newStudent = new StudentDTO
        {
            Id = 1,
        };
        var newPollInstanceDTO = new PollInstanceDTO() { Uuid= "Uuid1" , Student = newStudent};
        var command = new CreatePollInstanceCommand { PollInstance = newPollInstanceDTO };

        _mockPollInstanceRepository.Setup(Repo => Repo.AddAsync(It.IsAny<PollInstance>()))
            .ThrowsAsync(new Exception("DB Error"));

        CreateCommandResponse<PollInstance> result = await _handler.Handle(command, CancellationToken.None);

        Assert.Null(result.Entity);
        Assert.False(result.Success);
        Assert.Equal(0, result.SuccessfullImports);
        Assert.Equal("Error", result.Message);
    }
}
