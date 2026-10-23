using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.Features.Components.Commands.CreateCommand;
using Eras.Application.Mappers;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Eras.Application.Tests.Features.Components.Commands;
public class CreateComponentCommandHandlerTests
{
    private readonly Mock<IComponentRepository> _mockComponentRepository;
    private readonly Mock<ILogger<CreateComponentCommandHandler>> _mockLogger;
    private readonly CreateComponentCommandHandler _handler;

    public CreateComponentCommandHandlerTests()
    {
        _mockComponentRepository = new Mock<IComponentRepository>();
        _mockLogger = new Mock<ILogger<CreateComponentCommandHandler>>();
        _handler = new CreateComponentCommandHandler(_mockComponentRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnErrorResponseIfComponentIsNull()
    {
        var command = new CreateComponentCommand { Component = null };

        CreateCommandResponse<Component> result = await _handler.Handle(command, CancellationToken.None);

        Assert.Null(result.Entity);
        Assert.Equal(0, result.SuccessfullImports);
        Assert.Equal("Error", result.Message);
        Assert.False(result.Success);

        _mockComponentRepository.Verify(Repo => Repo.GetByNameAsync(It.IsAny<string>()), Times.Never);
        _mockComponentRepository.Verify(Repo => Repo.AddAsync(It.IsAny<Component>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnPersistedComponentIfExists()
    {
        var persistedComponent = new Component() { Name = "existingComponent" };
        var command = new CreateComponentCommand
        {
            Component = new ComponentDTO() { Name = "existingComponent" }
        };

        _mockComponentRepository.Setup(Repo => Repo.GetByNameAsync("existingComponent"))
            .ReturnsAsync(persistedComponent);
        
        CreateCommandResponse<Component> result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result.Entity);
        Assert.Equal("existingComponent", result.Entity.Name);
        Assert.Equal(0, result.SuccessfullImports);
        Assert.Equal("Success", result.Message);
        Assert.True(result.Success);

        _mockComponentRepository.Verify(Repo => Repo.AddAsync(It.IsAny<Component>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldCatchExceptionAndReturnErrorResponse()
    {
        var newComponentDTO = new ComponentDTO() { Name = "exception" };
        var command = new CreateComponentCommand { Component = newComponentDTO };

        _mockComponentRepository.Setup(Repo => Repo.AddAsync(It.IsAny<Component>()))
            .ThrowsAsync(new Exception("Error saving component."));
        
        CreateCommandResponse<Component> result = await _handler.Handle(command, CancellationToken.None);

        Assert.Null(result.Entity);
        Assert.Equal(0, result.SuccessfullImports);
        Assert.Equal("Error", result.Message);
        Assert.False(result.Success);
    }

    [Fact]
    public async Task Handle_CreatesNewComponentAsync()
    {
        var newComponentDTO = new ComponentDTO() { Name = "newComponent" };
        var command = new CreateComponentCommand { Component = newComponentDTO };
        Component createdComponent = newComponentDTO.ToDomain();

        _mockComponentRepository.Setup(
            Repo => Repo.AddAsync(It.IsAny<Component>())
        ).ReturnsAsync(createdComponent);

        CreateCommandResponse<Component> result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result.Entity);
        Assert.Equal("newComponent", result.Entity.Name);
        Assert.Equal(1, result.SuccessfullImports);
        Assert.Equal("Success", result.Message);
        Assert.True(result.Success);
    }
}
