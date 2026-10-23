using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.Features.ServiceProviders.Command;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Features.ServiceProviders.Command;

public class CreateServiceProviderCommandHandlerTests
{
    private readonly Mock<IServiceProvidersRepository> _serviceProvidersRepository;
    private readonly Mock<ILogger<CreateServiceProviderCommandHandler>> _logger;
    private readonly CreateServiceProviderCommandHandler _handler;

    public CreateServiceProviderCommandHandlerTests()
    {
        _serviceProvidersRepository = new Mock<IServiceProvidersRepository>();
        _logger = new Mock<ILogger<CreateServiceProviderCommandHandler>>();

        _handler = new CreateServiceProviderCommandHandler(
            _serviceProvidersRepository.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenServiceProviderIsNull()
    {
        var request = new CreateServiceProviderCommand
        {
            ServiceProviders = null
        };

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.Null(result.Entity);
        Assert.Equal("Error", result.Message);
        Assert.False(result.Success);

        _serviceProvidersRepository.Verify(x => x.GetByNameAsync(It.IsAny<string>()), Times.Never);
        _serviceProvidersRepository.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.ServiceProviders>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnExistingServiceProvider_WhenNameAlreadyExists()
    {
        var serviceProviderDto = new ServiceProvidersDTO
        {
            ServiceProviderName = "Provider 1",
            ServiceProviderLogo = "Logo"
        };

        var existingServiceProvider = new Domain.Entities.ServiceProviders()
        {
            ServiceProviderName = "Provider 1",
            ServiceProviderLogo = "Logo"
        };

        var request = new CreateServiceProviderCommand
        {
            ServiceProviders = serviceProviderDto
        };

        _serviceProvidersRepository
            .Setup(x => x.GetByNameAsync(serviceProviderDto.ServiceProviderName))
            .ReturnsAsync(existingServiceProvider);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.Equal(existingServiceProvider, result.Entity);
        Assert.Equal("Success", result.Message);
        Assert.True(result.Success);

        _serviceProvidersRepository.Verify(
            x => x.GetByNameAsync(serviceProviderDto.ServiceProviderName), Times.Once);

        _serviceProvidersRepository.Verify(
            x => x.AddAsync(It.IsAny<Domain.Entities.ServiceProviders>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldCreateServiceProvider_WhenNameDoesNotExist()
    {
        var serviceProviderDto = new ServiceProvidersDTO
        {
            ServiceProviderName = "Provider 1",
            ServiceProviderLogo = "logo"  
        };

        var createdServiceProvider = new Domain.Entities.ServiceProviders()
        {
            ServiceProviderName = "Provider 1",
            ServiceProviderLogo = "logo"
        };

        var request = new CreateServiceProviderCommand
        {
            ServiceProviders = serviceProviderDto
        };

        _serviceProvidersRepository
            .Setup(x => x.GetByNameAsync(serviceProviderDto.ServiceProviderName))
            .ReturnsAsync((Domain.Entities.ServiceProviders)null!);

        _serviceProvidersRepository
            .Setup(x => x.AddAsync(It.IsAny<Domain.Entities.ServiceProviders>()))
            .ReturnsAsync(createdServiceProvider);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.Equal(createdServiceProvider, result.Entity);
        Assert.Equal("Success", result.Message);
        Assert.True(result.Success);

        _serviceProvidersRepository.Verify(
            x => x.GetByNameAsync(serviceProviderDto.ServiceProviderName), Times.Once);

        _serviceProvidersRepository.Verify(
            x => x.AddAsync(It.IsAny<Domain.Entities.ServiceProviders>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenRepositoryThrowsException()
    {
        var serviceProviderDto = new ServiceProvidersDTO
        {
            ServiceProviderName = "Provider 1",
            ServiceProviderLogo = "avatar"
        };

        var request = new CreateServiceProviderCommand
        {
            ServiceProviders = serviceProviderDto
        };

        _serviceProvidersRepository
            .Setup(x => x.GetByNameAsync(serviceProviderDto.ServiceProviderName))
            .ThrowsAsync(new Exception());

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.Null(result.Entity);
        Assert.Equal("Error", result.Message);
        Assert.False(result.Success);

        _serviceProvidersRepository.Verify(
            x => x.GetByNameAsync(serviceProviderDto.ServiceProviderName), Times.Once);
        _serviceProvidersRepository.Verify(
            x => x.AddAsync(It.IsAny<Domain.Entities.ServiceProviders>()), Times.Never);
    }
}

