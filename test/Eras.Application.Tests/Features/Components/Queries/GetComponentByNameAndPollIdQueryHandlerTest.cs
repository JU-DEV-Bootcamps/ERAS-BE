using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Components.Queries.GetByName;
using Eras.Application.Features.Components.Queries.GetByNameAndPoll;
using Eras.Application.Features.PollInstances.Queries.GetPollInstancesByCohortAndDays;
using Eras.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Eras.Application.Tests.Features.Components.Queries;
public class GetComponentByNameAndPollIdQueryHandlerTest
{
    private readonly Mock<IComponentRepository> _mockComponentRepository;
    private readonly Mock<ILogger<GetComponentByNameAndPollIdQueryHandler>> _mockLogger;
    private readonly GetComponentByNameAndPollIdQueryHandler _handler;

    public GetComponentByNameAndPollIdQueryHandlerTest()
    {
        _mockComponentRepository = new Mock<IComponentRepository>();
        _mockLogger = new Mock<ILogger<GetComponentByNameAndPollIdQueryHandler>>();
        _handler = new GetComponentByNameAndPollIdQueryHandler(_mockComponentRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Success_ResponseAsync()
    {
        // Arrange
        var query = new GetComponentByNameAndPollIdQuery() { ComponentName = "Component", PollId = 1 };
        var component = new Component()
        {
            Name = "Component"
        };

        _mockComponentRepository
            .Setup(Repo => Repo.GetByNameAndPollIdAsync(
                It.Is<string>(Name => Name == "Component"),
                It.Is<int>(Id => Id == 1)))
            .ReturnsAsync(component);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Component",result.Body.Name);
    }
}
