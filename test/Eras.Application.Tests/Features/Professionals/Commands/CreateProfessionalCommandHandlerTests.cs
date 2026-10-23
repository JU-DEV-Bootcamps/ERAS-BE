using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.Features.Professionals.Commands.CreateProfessional;
using Eras.Application.Models.Enums;
using Eras.Domain.Common;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Features.Professionals.Commands;

public class CreateProfessionalCommandHandlerTests
{
    private readonly Mock<IProfessionalRepository> _repository ;
    private readonly Mock<ILogger<CreateProfessionalCommandHandler>> _logger;
    private readonly CreateProfessionalCommandHandler _handler;

    public CreateProfessionalCommandHandlerTests()
    {
        _repository = new Mock<IProfessionalRepository>();
        _logger = new Mock<ILogger<CreateProfessionalCommandHandler>>();
        _handler = new CreateProfessionalCommandHandler(_repository.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_CreatesProfessional_WhenEntityDoesNotExist()
    {
        // Arrange
        var professional = new JUProfessionalDTO
        {
            Id = 1,
            Name = "Anne",
            Uuid = "123",
            Audit = new AuditInfo(),
        };

        var request = new CreateProfessionalCommand
        {
            Professional = professional
        };

        var createdProfessional = new JUProfessional() 
        { 
            Id = 1,
            Name = "Anne",
            Uuid = "123",
            Audit = new AuditInfo(),
        };

        _repository
            .Setup(x => x.AddAsync(It.IsAny<JUProfessional>()))
            .ReturnsAsync(createdProfessional);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Success", result.Message);
        Assert.Equal(createdProfessional, result.Entity);

        _repository.Verify(x => x.AddAsync(It.IsAny<JUProfessional>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsAlreadyExists_WhenProfessionalWithIdExists()
    {
        // Arrange
        var professional = new JUProfessionalDTO
        {
            Id = 123,
            Name = "Vero"
        };

        var request = new CreateProfessionalCommand
        {
            Professional = professional
        };

        var existingProfessional = new JUProfessional
        {
            Id = 123
        };

        _repository
            .Setup(x => x.GetByIdAsync(123))
            .ReturnsAsync(existingProfessional);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Entity already exists", result.Message);
        Assert.Equal(CommandEnums.CommandResultStatus.AlreadyExists, result.Status);

        _repository.Verify(x => x.GetByIdAsync(123), Times.Once);
        _repository.Verify(x => x.AddAsync(It.IsAny<JUProfessional>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ReturnsError_WhenRepositoryThrows()
    {
        // Arrange
        var professional = new JUProfessionalDTO
        {
            Id = 123,
            Name = "Eve"
        };

        var request = new CreateProfessionalCommand
        {
            Professional = professional
        };

        _repository
            .Setup(x => x.GetByIdAsync(123))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Error", result.Message);
        Assert.Equal(CommandEnums.CommandResultStatus.Error, result.Status);

        _logger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString()!.Contains("An error occurred creating the professional")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
