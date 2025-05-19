using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Components.Queries.GetByName;
using Eras.Application.Features.PollInstances.Queries.GetPollInstancesByCohortAndDays;
using Eras.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Eras.Application.Tests.Features.Components.Queries;
public class GetComponentByNameQueryHandlerTest
{
    private readonly Mock<IComponentRepository> _mockComponentRepository;
    private readonly Mock<ILogger<GetComponentByNameQueryHandler>> _mockLogger;
    private readonly GetComponentByNameQueryHandler _handler;

    public GetComponentByNameQueryHandlerTest()
    {
        _mockComponentRepository = new Mock<IComponentRepository>();
        _mockLogger = new Mock<ILogger<GetComponentByNameQueryHandler>>();
        _handler = new GetComponentByNameQueryHandler(_mockComponentRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Success_ResponseAsync()
    {
        // Arrange
        var query = new GetComponentByNameQuery() { componentName = "Component" };
        var component = new Component()
        {
            Name = "Component"
        };

        _mockComponentRepository
            .Setup(Repo => Repo.GetByNameAsync(It.Is<string>(Name => Name == "Component")))
            .ReturnsAsync(component);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Component",result.Body.Name);
    }
}
