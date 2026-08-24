using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.Features.JUServices.Commands.CreateJUService;
using Eras.Application.Mappers;
using Eras.Application.Models.Enums;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Test.Features.JUServices.Commands;
public class CreateJUServiceCommandHandlerTest
{
    private readonly Mock<IJUServiceRepository> _mockJuServiceRepository;
    private readonly Mock<ILogger<CreateJUServiceCommandHandler>> _mockLogger;
    private readonly CreateJUServiceCommandHandler _handler;

    public CreateJUServiceCommandHandlerTest()
    {
        _mockJuServiceRepository = new Mock<IJUServiceRepository>();
        _mockLogger = new Mock<ILogger<CreateJUServiceCommandHandler>>();
        _handler = new CreateJUServiceCommandHandler(_mockJuServiceRepository.Object, _mockLogger.Object);
    }

    private static JUServiceDTO BuildJUServiceDTO(int Id = 1, string Name = "Test Service") => new() { Id = Id, Name = Name, Audit = new Domain.Common.AuditInfo() };

    [Fact]
    public async Task Handler_ShouldReturnDefaultService_IfEntityAlreadyExists()
    {
        JUServiceDTO serviceDTO = BuildJUServiceDTO();
        JUService serviceEntity = serviceDTO.ToDomain();

        var command = new CreateJUServiceCommand { Service = serviceDTO };

        _mockJuServiceRepository.Setup(Repo => Repo.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(serviceEntity);

        CreateCommandResponse<JUService> response = await _handler.Handle(command, CancellationToken.None);

        _mockJuServiceRepository.Verify(Repo => Repo.AddAsync(It.IsAny<JUService>()), Times.Never);
        Assert.NotNull(response.Entity);
        Assert.IsType<JUService>(response.Entity);
        Assert.Equal(response.Entity.Name, string.Empty);
        Assert.Equal("Entity already exists", response.Message);
        Assert.False(response.Success);
        Assert.Equal(CommandEnums.CommandResultStatus.AlreadyExists, CommandEnums.CommandResultStatus.AlreadyExists);
    }

    [Fact]
    public async Task Handler_ShouldCreateAndReturnNewService_IfItDoesNotExistInDB()
    {
        JUServiceDTO serviceDTO = BuildJUServiceDTO();
        JUService serviceEntity = serviceDTO.ToDomain();

        var command = new CreateJUServiceCommand { Service = serviceDTO };

        _mockJuServiceRepository.Setup(Repo => Repo.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(value: null);
        _mockJuServiceRepository.Setup(Repo => Repo.AddAsync(It.IsAny<JUService>()))
            .ReturnsAsync(serviceEntity);

        CreateCommandResponse<JUService> response = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(response.Entity);
        Assert.IsType<JUService>(response.Entity);
        Assert.Equal(response.Entity, serviceEntity);
        Assert.Equal(1, response.SuccessfullImports);
        Assert.Equal("Success", response.Message);
        Assert.True(response.Success);
    }

    [Fact]
    public async Task Handler_ShouldHandleExceptionAndReturnCommandErrorResponse()
    {
        JUServiceDTO serviceDTO = BuildJUServiceDTO();
        JUService serviceEntity = serviceDTO.ToDomain();

        var command = new CreateJUServiceCommand { Service = serviceDTO };

        _mockJuServiceRepository.Setup(Repo => Repo.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(value: null);
        _mockJuServiceRepository.Setup(Repo => Repo.AddAsync(It.IsAny<JUService>()))
            .ThrowsAsync(new Exception("Error creating service."));

        CreateCommandResponse<JUService> response = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(response.Entity);
        Assert.IsType<JUService>(response.Entity);
        Assert.Equal(response.Entity.Name, string.Empty);
        Assert.Equal("Error", response.Message);
        Assert.False(response.Success);
        Assert.Equal(CommandEnums.CommandResultStatus.Error, response.Status);
    }
}