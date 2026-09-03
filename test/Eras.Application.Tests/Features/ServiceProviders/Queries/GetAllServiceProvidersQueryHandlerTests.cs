using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.ServiceProviders.Queries;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Features.ServiceProviders.Queries;

public class GetAllServiceProvidersQueryHandlerTests
{
    private readonly Mock<IServiceProvidersRepository> _serviceProvidersRepository;
    private readonly Mock<ILogger<GetAllServiceProvidersQueryHandler>> _logger;
    private readonly GetAllServiceProvidersQueryHandler _handler;

    public GetAllServiceProvidersQueryHandlerTests()
    {
        _serviceProvidersRepository = new Mock<IServiceProvidersRepository>();
        _logger = new Mock<ILogger<GetAllServiceProvidersQueryHandler>>();

        _handler = new GetAllServiceProvidersQueryHandler(
            _serviceProvidersRepository.Object,
            _logger.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAllServiceProviders()
    {
        var serviceProviders = new List<Domain.Entities.ServiceProviders>
        {
            new Domain.Entities.ServiceProviders()
            {
                ServiceProviderLogo = "logo",
                ServiceProviderName = "first"
            },
            new Domain.Entities.ServiceProviders()
            {
                ServiceProviderLogo = "logo2",
                ServiceProviderName = "second"
            }
        };

        _serviceProvidersRepository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(serviceProviders);

        var result = await _handler.Handle(new GetAllServiceProvidersQuery(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(serviceProviders, result);

        _serviceProvidersRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }
}
